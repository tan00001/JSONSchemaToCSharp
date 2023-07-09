// See https://aka.ms/new-console-template for more information
using System.Reflection;
using Microsoft.Extensions.Configuration;
using JSONSchemaToCSharp;

Console.WriteLine("JSON Schema to C# class definition. Version " + Assembly.GetExecutingAssembly().GetName().Version);
Console.WriteLine();

if (args.Length < 2)
{
    Console.WriteLine("Usage: JSONSchemaToCSharp <JSONSchema File Path> <C# Output File Path> <Optional Name Space Name>");
    return;
}

var schemaFilePath = args[0];

if (!File.Exists(schemaFilePath))
{
    Console.WriteLine("File \"" + schemaFilePath + "\" does not exist.");
    return;
}

InitiSettings();

try
{
    System.Text.Json.JsonDocument jsonDoc;
    using (var inputStream = new FileStream(schemaFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
    {
        jsonDoc = System.Text.Json.JsonDocument.Parse(inputStream);
    }

    var outputFilePath = args[1];

    var namespaceDefinition = args.Length >= 3 ? new NamespaceDefinition(schemaFilePath, args[2])
        : new NamespaceDefinition(schemaFilePath);

    namespaceDefinition.AddRootClass(jsonDoc.RootElement);

    using (var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
    using (var sw = new StreamWriter(outputStream))
    {
        namespaceDefinition.Write(sw);
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

static void InitiSettings()
{
    try
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);

        IConfiguration configuration = builder.Build();

        var namespaceSection = configuration.GetSection("namespace");
        NamespaceDefinition.UsingNamespaces = namespaceSection.GetSection("using").Get<string[]>();

        var enumSection = configuration.GetSection("enum");
        EnumDefinition.StringConverter = enumSection.GetValue<string>("stringConverter");
        EnumDefinition.StringCollectionConverter = enumSection.GetValue<string>("stringCollectionConverter");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}