# 1toOne Converter Online

1toOne Converter Online is a browser-based map converter for the TMOne title packs. Drop one or more compatible `.Challenge.Gbx` maps into the app, let it translate the supported blocks into the TMOne items, then download each result or all successful conversions in a ZIP file.

The conversion happens locally in your browser. Your maps are not uploaded to a server, and you can use the site in offline mode.

## Using the converter

Open the site, add your map files, select a converted map, and download it. With several maps, use **Download all conversions** to receive a ZIP archive.

The website is also installable as a progressive web app. After the first successful load, its application files and conversion definitions are cached.

## Script metadata (traits)

Every converted map includes script metadata traits. Gamemodes and map editor plugins can use these to identify converted maps and access information about their source maps.

| Trait | Type | Value | Description |
| --- | --- | --- | --- |
| `MadeWith1toOneConverterOnline` | Boolean | `true` | The map was converted with 1toOne Converter Online. |
| `Unbitn_1toOne_IsConverted` | Boolean | `true` | The map was converted with 1toOne Converter Online. |
| `Unbitn_1toOne_ConvertedAt` | Text | `2026-09-12T14:30:00` | Date and time of conversion in ISO 8601 format, always UTC. |
| `Unbitn_1toOne_Phase` | Text | `Stable` | Current release phase of the converter. |
| `Unbitn_1toOne_Environment` | Text | `Alpine` | Environment of the original map. |
| `Unbitn_1toOne_OriginalAuthorLogin` | Text | `examplelogin` | Login of the original map author. |
| `Unbitn_1toOne_OriginalMapUid` | Text | `JtPkBe105hUkDkQOoieBVFZTDK9` | UID of the original map, captured before a new UID is generated for the converted map. |

### Usage in ManiaScript

```maniascript
declare metadata Boolean MadeWith1toOneConverterOnline for Map;
```

## Disclaimer

The code had been ported from [Luk's 1toOne-Converter](https://github.com/LuksTrackmaniaCorner/1toOne-Converter) to be compatible with GBX.NET over the last 4 years.
Due to tighten Gbx processing with the conversion process, many compromises had to be made, therefore, you should NOT look into the code if at all possible.

It is possible that this converter will be remade to run on a Convengine in a couple of years, which I still consider low priority; otherwise, don't expect the code to become any better in a reasonable time.
