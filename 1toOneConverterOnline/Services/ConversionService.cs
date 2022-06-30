using _1toOneConverterOnline.Models;

namespace _1toOneConverterOnline.Services;

public class ConversionService : IConversionService
{
    private readonly ISettingsService _settings;

    public ConversionService(ISettingsService settings)
    {
        _settings = settings;
    }

    public void Convert(Map map)
    {
        if (_settings.Conversions is null)
        {
            throw new Exception("No conversions are available.");
        }

        var env = map.Challenge.Collection.ToString();

        if (!_settings.Conversions.TryGetValue(env, out var conversion))
        {
            throw new Exception($"{env} conversion is not available.");
        }

        conversion.Convert(map);
    }
}
