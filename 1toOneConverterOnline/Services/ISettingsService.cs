using _1toOneConverterOnline.Models.Settings;
using _1toOneConverterOnline.Models.Settings.Conversion;

namespace _1toOneConverterOnline.Services;

public interface ISettingsService
{
    MainSettings? MainSettings { get; }
    Dictionary<string, ComplexConversion>? Conversions { get; }

    Task LoadConversionSettingsAsync(Func<string, Task> progress, CancellationToken cancellationToken = default);
    Task LoadMainSettingsAsync(CancellationToken cancellationToken = default);
}