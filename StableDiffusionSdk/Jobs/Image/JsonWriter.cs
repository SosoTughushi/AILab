using Newtonsoft.Json;

namespace StableDiffusionSdk.Jobs.Image;

public class JsonWriter
{
    private readonly string _outputFolder;
    private int _savedFileCount;

    public JsonWriter(string baseOutputFolder)
    {
        _outputFolder = Path.Combine(baseOutputFolder, "Logs");
        Directory.CreateDirectory(_outputFolder);
    }

    public async Task Write<T>(T obj)
    {
        _savedFileCount++;

        var fileName = Path.Combine(_outputFolder, $"{_savedFileCount:D5}.json");
        await using var file = File.CreateText(fileName);
        var serializer = new JsonSerializer()
        {
            Formatting = Formatting.Indented
        };
        await Task.Run(() => serializer.Serialize(file, obj));
    }
}