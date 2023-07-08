# JSONSchemaToCSharp

## Table of Contents
1. [What is JSONSchemaToCSharp](#What-is-JSONSchemaToCSharp)
2. [Prerequisites](#Prerequisites)
3. [Installation](#Installation)
4. [How it works](#How-it-works)
4. [License](#License)

## What is JSONSchemaToCSharp
JSONSchemaToCSharp generate C# code from JSON schema files (https://json-schema.org/). Although Visual Studio is capable of generating C# classes from JSON data files, we still need to have a tool to generate C# code from JSON schema files.

The following is part of a Walmrt API schema file:
```
{
    "$schema": "http://json-schema.org/draft-04/schema#", 
    "type": "object", 
    "title": "MPItemFeed", 
    "properties": {
        "MPItemFeedHeader": {
            "$schema": "http://json-schema.org/draft-04/schema#", 
            "type": "object", 
            "properties": {
                "requestId": {
                    "type": "string", 
                    "title": "Request ID", 
                    "minLength": 1, 
                    "maxLength": 64
                }, 
                "requestBatchId": {
                    "type": "string", 
                    "title": "Request Batch ID", 
                    "minLength": 1, 
                    "maxLength": 64
                }, 
                "feedDate": {
                    "type": "string", 
                    "title": "Feed Date", 
                    "format": "date-time"
                }, 
                "processMode": {
                    "type": "string", 
                    "title": "Process Mode", 
                    "enum": [
                        "REPLACE"
                    ]
                }, 
                "subset": {
                    "type": "string", 
                    "title": "Subset", 
                    "enum": [
                        "EXTERNAL"
                    ]
                }, 
                "mart": {
                    "type": "string", 
                    "title": "Mart", 
                    "enum": [
                        "WALMART_CA", 
                        "WALMART_US", 
                        "ASDA_GM"
                    ]
                }, 
                "sellingChannel": {
                    "type": "string", 
                    "title": "Selling Channel", 
                    "enum": [
                        "marketplace"
                    ]
                }, 
                "version": {
                    "type": "string", 
                    "title": "Spec Version", 
                    "enum": [
                        "4.8"
                    ]
                }, 
                "subCategory": {
                    "type": "string", 
                    "title": "Sub Category", 
                    "enum": [
                        "cases_and_bags", 
                        "building_supply", 
                        "tires", 
                        "computer_components", 
                        "health_and_beauty_electronics", 
                        "furniture_other", 
                        "decorations_and_favors", 
                        "hardware", 
                        "child_car_seats", 
                        "food_and_beverage_other", 
                        "electronics_other", 
                        "electronics_cables", 
                        "plumbing_and_hvac", 
                        "video_games", 
                        "other_other", 
                        "safety_and_emergency", 
                        "jewelry_other", 
                        "books_and_magazines", 
                        "tools", 
                        "sport_and_recreation_other", 
                        "carriers_and_accessories_other", 
                        "animal_food", 
                        "baby_toys", 
                        "cleaning_and_chemical", 
                        "ceremonial_clothing_and_accessories", 
                        "music_cases_and_bags", 
                        "computers", 
                        "grills_and_outdoor_cooking", 
                        "personal_care", 
                        "bedding", 
                        "storage", 
                        "animal_accessories", 
                        "baby_food", 
                        "electrical", 
                        "medical_aids", 
                        "music", 
                        "art_and_craft_other", 
                        "medicine_and_supplements", 
                        "toys_other", 
                        "wheels_and_wheel_components", 
                        "footwear_other", 
                        "tv_shows", 
                        "animal_health_and_grooming", 
                        "video_projectors", 
                        "cameras_and_lenses", 
                        "sound_and_recording", 
                        "watercraft", 
                        "funeral", 
                        "watches_other", 
                        "large_appliances", 
                        "baby_furniture", 
                        "costumes", 
                        "instrument_accessories", 
                        "optical", 
                        "home_other", 
                        "cycling", 
                        "gift_supply_and_awards", 
                        "fuels_and_lubricants", 
                        "baby_other", 
                        "vehicle_other", 
                        "animal_other", 
                        "optics", 
                        "garden_and_patio_other", 
                        "cell_phones", 
                        "musical_instruments", 
                        "printers_scanners_and_imaging", 
                        "movies", 
                        "office_other", 
                        "tvs_and_video_displays", 
                        "tools_and_hardware_other", 
                        "electronics_accessories", 
                        "vehicle_parts_and_accessories", 
                        "land_vehicles", 
                        "clothing_other", 
                        "photo_accessories", 
                        "software"
                    ]
                }, 
                "locale": {
                    "type": "string", 
                    "title": "Locale", 
                    "enum": [
                        "en"
                    ]
                }
            }, 
            "required": [
                "subset", 
                "locale", 
                "sellingChannel", 
                "processMode", 
                "version"
            ], 
            "additionalProperties": false
        }
    }
}
```
Here is the generated code:
```
/// <summary>
/// MPItemFeed.
/// </summary>
[DataContract(Namespace = "http://json-schema.org/draft-04/schema#")]
public class MPItemFeed
{
	/// <summary>
	/// MPItemFeedHeader.
	/// </summary>
	[Required]
	public MPItemFeedHeader? MPItemFeedHeader { get; set; }
};

/// <summary>
/// MPItemFeedHeader.
/// </summary>
[DataContract(Namespace = "http://json-schema.org/draft-04/schema#")]
public class MPItemFeedHeader
{
	/// <summary>
	/// Request ID.
	/// </summary>
	[JsonPropertyName("requestId")]
	[StringLength(64, MinimumLength = 1)]
	public string? RequestId { get; set; }

	/// <summary>
	/// Request Batch ID.
	/// </summary>
	[JsonPropertyName("requestBatchId")]
	[StringLength(64, MinimumLength = 1)]
	public string? RequestBatchId { get; set; }

	/// <summary>
	/// Feed Date.
	/// </summary>
	[JsonPropertyName("feedDate")]
	public DateTimeOffset FeedDate { get; set; }

	/// <summary>
	/// Process Mode.
	/// </summary>
	[JsonPropertyName("processMode")]
	[Required]
	public ProcessMode ProcessMode { get; set; }

	/// <summary>
	/// Subset.
	/// </summary>
	[JsonPropertyName("subset")]
	[Required]
	public Subset Subset { get; set; }

	/// <summary>
	/// Mart.
	/// </summary>
	[JsonPropertyName("mart")]
	public Mart Mart { get; set; }

	/// <summary>
	/// Selling Channel.
	/// </summary>
	[JsonPropertyName("sellingChannel")]
	[Required]
	public SellingChannel SellingChannel { get; set; }

	/// <summary>
	/// Spec Version.
	/// </summary>
	[JsonPropertyName("version")]
	[Required]
	public SpecVersion Version { get; set; }

	/// <summary>
	/// Sub Category.
	/// </summary>
	[JsonPropertyName("subCategory")]
	public SubCategory SubCategory { get; set; }

	/// <summary>
	/// Locale.
	/// </summary>
	[JsonPropertyName("locale")]
	[Required]
	public Locale Locale { get; set; }
};

...
```

## Prerequisites
1. **.NET Framework 4.8.1**. The initial version of this tool is a command line application. A Visual Studio extension will be added. Because Visual Studio Extension projects do not support .NET Core versions such as .NET 5, .NET 6, and .NET 7, .NET Framework 4.8.1 is required.
2. **.NET 7**. In order to support all the latest C# syntax, we use .NET 7 for processing the C# type definitions.
3. **Visual Studio 2022**. .NET 7 is only supported in Visual Studio 2022 and later.

## Installation
For now, you will need to build everything from the source code in Visual Studio 2022.

## How it works
JSONSchemaToCSharp needs to be executed on Windows command line:
```
JSONSchemaToCSharp <JSON scheme file path> <C# code file path> <Optional namespace name>
```
If you would like to customize the output, you may create a JSON configuration file that is named as follows. Let's say your JSON schema file path is
```
C:\MySchemaFiles\MyJSONSchema.json
```
You can configure your customizations by creating a companion file to the JSON schema file with the path
```
C:\MySchemaFiles\MyJSONSchema.Common.json
```
Inside `C:\MySchemaFiles\MyJSONSchema.Common.json`, structure definitions are identified by signatures: 
```
{
  "(Measure(decimal), Unit([kg, lb, oz, g]))": {
    "name": "Weight",
    "isValueType": true
  },
  "(Measure(decimal), Unit([ft, in]))": {
    "name": "EnglishLength",
    "isValueType": true
  },
  "(Measure(decimal), Unit([ºF, ºK, ºC]))": {
    "name": "Temperature"
  }
}
```
1. The signature of an enum definition is an array of its values listed inside a pair of square brackets, such as `[kg, lb, oz, g]`
2. A structure member is identified by its name, followed by the signature of its type in parentheses. For example, `Unit([kg, lb, oz, g])` identifies a member named `Unit` that is an enumerated value from the list `[kg, lb, oz, g]`.
3. A structure is a list of its members inside a pair of parentheses. For eample, `(Measure(decimal), Unit([kg, lb, oz, g]))` is a structure with two members named `Measure` and `Unit`. `Measure` is a number, and `Unit` is an enumerated value.
Here whenever a structure of type `(Measure(decimal), Unit([kg, lb, oz, g]))` is encountered, JSONSchemaToCSharp will use the definition named `Weight` as its type:

```
public class Weight
{
	/// <summary>
	/// Measure.
	/// </summary>
	[JsonPropertyName("measure")]
	[Range(typeof(decimal), "0", "1000000000")]
	public decimal Measure { get; set; }

	/// <summary>
	/// Unit.
	/// </summary>
	[JsonPropertyName("unit")]
	public Unit Unit { get; set; }
};
```
By adding `"isValueType": true` in `C:\MySchemaFiles\MyJSONSchema.Common.json` as shown, the output will become
```
public struct Weight
{
	/// <summary>
	/// Measure.
	/// </summary>
	[JsonPropertyName("measure")]
	[Range(typeof(decimal), "0", "1000000000")]
	public decimal Measure { get; set; }

	/// <summary>
	/// Unit.
	/// </summary>
	[JsonPropertyName("unit")]
	public Unit Unit { get; set; }
};
```
The reference of the type `Weight` would look like this
```
    ...
	/// <summary>
	/// Weight.
	/// </summary>
	[JsonPropertyName("minimumWeight")]
	public Weight MinimumWeight { get; set; }
    ...
```
if it is defined as a value type, or
```
    ...
	/// <summary>
	/// Weight.
	/// </summary>
	[JsonPropertyName("minimumWeight")]
	public Weight? MinimumWeight { get; set; }
    ...
```
if it is defined as a class.
## License
JSONSchemaToCSharp is under the <a href="https://opensource.org/license/mit/">MIT License</a>.

