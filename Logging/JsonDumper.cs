using Newtonsoft.Json;
using System.IO;

/// <summary>
/// Stellt Routinen zum Speichern von Objekten als JSON bereit.
/// </summary>
public static class JsonDumper
{
    /// <summary>
    /// Serialisiert ein Objekt in ein JSON-String.
    /// Serialisiert auch rekursive Objekte.
    /// </summary>
    /// <typeparam name="T">Generischer Typparameter.</typeparam>
    /// <param name="obj">Das zu serialisierende Objekt.</param>
    /// <returns>Json-String mit Objektinformationen.</returns>
    public static string DumpToJson<T>(T obj)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };

        return JsonConvert.SerializeObject(obj, settings);
    }

    /// <summary>
    /// Serialisiert ein Objekt in ein JSON-String und speichert es in einer Datei.
    /// </summary>
    /// <typeparam name="T">Generischer Typparameter.</typeparam>
    /// <param name="obj">Das zu serialisierende Objekt.</param>
    /// <param name="filePath">Dateipfad der zu erstellenden Json-Datei.</param>
    public static void DumpToFile<T>(T obj, string filePath)
    {
        string json = DumpToJson(obj);
        File.WriteAllText(filePath, json);
    }
}