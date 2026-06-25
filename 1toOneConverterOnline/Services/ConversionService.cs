using _1toOneConverterOnline.Models;

namespace _1toOneConverterOnline.Services;

internal interface IConversionService
{
    void Convert(Map map);
}

internal sealed class ConversionService : IConversionService
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

        var env = map.Challenge.Collection.ToString() ?? throw new Exception("Collection is null.");

        if (!_settings.Conversions.TryGetValue(env, out var conversion))
        {
            throw new Exception($"{env} conversion is not available.");
        }

        if (map.Challenge.CreatedWithSimpleEditor)
        {
            throw new Exception("The map was created with the simple editor, which is not supported.");
        }

        map.Environment = env;
        conversion.Convert(map);
    }
}
