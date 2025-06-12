using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using _1toOneConverterOnline.Server;
using Microsoft.AspNetCore.CookiePolicy;
using AspNet.Security.OAuth.Discord;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
   .AddCookie(options =>
   {
       options.LoginPath = "/login";
       options.AccessDeniedPath = "/access-denied";
   })
   .AddDiscord(options =>
   {
       options.ClientId = builder.Configuration["Discord:ClientId"] ?? throw new InvalidOperationException("Discord ClientId is missing");
       options.ClientSecret = builder.Configuration["Discord:ClientSecret"] ?? throw new InvalidOperationException("Discord ClientSecret is missing");
       options.AccessDeniedPath = "/access-denied";
       options.ClaimActions.MapJsonKey(DiscordAdditionalClaims.GlobalName, "global_name");
   });

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen, applyThemeToRedirectedOutput: true)
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        options.Protocol = builder.Configuration["OTEL_EXPORTER_OTLP_PROTOCOL"]?.ToLowerInvariant() switch
        {
            "grpc" => Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc,
            "http/protobuf" or null or "" => Serilog.Sinks.OpenTelemetry.OtlpProtocol.HttpProtobuf,
            _ => throw new NotSupportedException($"OTLP protocol {builder.Configuration["OTEL_EXPORTER_OTLP_PROTOCOL"]} is not supported")
        };
        options.Headers = builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split('=', 2, StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(x => x[0], x => x[1]) ?? [];
    })
    .CreateLogger();

builder.Services.AddSerilog();

builder.Services.AddOpenTelemetry()
    .WithMetrics(options =>
    {
        options
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddOtlpExporter();

        options.AddMeter("System.Net.Http");
    })
    .WithTracing(options =>
    {
        if (builder.Environment.IsDevelopment())
        {
            options.SetSampler<AlwaysOnSampler>();
        }

        options
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    Secure = CookieSecurePolicy.Always,
    HttpOnly = HttpOnlyPolicy.Always
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

var allowedDiscordUserIds = builder.Configuration.GetSection("AllowedDiscordUserIds").Get<List<string>>() ?? [];

app.Use(async (context, next) =>
{
    if (!context.User.Identity?.IsAuthenticated ?? true)
    {
        await context.ChallengeAsync(DiscordAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
        return;
    }

    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId is null || !allowedDiscordUserIds.Contains(userId))
    {
        var globalName = context.User.FindFirst(DiscordAdditionalClaims.GlobalName)?.Value;
        var username = context.User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

        logger.LogWarning("Access denied for user: {GlobalName} ({Username})", globalName ?? username, username);
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Access denied.");
        return;
    }

    await next();
});

app.MapFallbackToFile("index.html");

app.Run();
