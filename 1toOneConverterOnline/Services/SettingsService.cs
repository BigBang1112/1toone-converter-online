using _1toOneConverterOnline.Models.Settings;
using _1toOneConverterOnline.Models.Settings.Conversion;
using System.Text.Json;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Services;

internal interface ISettingsService
{
    MainSettings? MainSettings { get; }
    Dictionary<string, ComplexConversion>? Conversions { get; }

    Task LoadConversionSettingsAsync(Func<string, Task> progress, CancellationToken cancellationToken = default);
    Task LoadMainSettingsAsync(CancellationToken cancellationToken = default);
}

internal sealed class SettingsService : ISettingsService
{
    private readonly HttpClient _http;

    public MainSettings? MainSettings { get; private set; }
    public Dictionary<string, ComplexConversion>? Conversions { get; private set; }

    public SettingsService(HttpClient http)
    {
        _http = http;
    }

    public async Task LoadMainSettingsAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync("appsettings.json", cancellationToken);
        
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        
        MainSettings = await JsonSerializer.DeserializeAsync<MainSettings>(stream, cancellationToken: cancellationToken);
    }

    public async Task LoadConversionSettingsAsync(Func<string, Task> progress, CancellationToken cancellationToken = default)
    {
        if (MainSettings?.Environments is null)
        {
            return;
        }

        Conversions = [];

        var responses = new Dictionary<string, Task<HttpResponseMessage>>();

        foreach (var environment in MainSettings.Environments)
        {
            responses.Add(environment, _http.GetAsync($"{environment}Conversion.xml", cancellationToken));
        }

        foreach (var (environment, responseTask) in responses)
        {
            await progress(environment);

            using var response = await responseTask;

            if (!response.IsSuccessStatusCode)
            {
                continue;
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            var serializer = new XmlSerializer(typeof(ComplexConversion));

            if (serializer.Deserialize(stream) is not ComplexConversion conversion)
            {
                continue;
            }

            Conversions.Add(environment, conversion);
        }
    }
}
