#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

public class LocalizationSyncWindow : EditorWindow
{
    [Header("Google Sheet (Publish to Web) - One sheet with many language columns")]
    [SerializeField] private string fileUrl = "https://.../pub?output=csv";
    [SerializeField] private bool useTsv = false;
    [SerializeField] private string outFolder = "Assets/Resources/Localization";
    
    private static readonly Dictionary<string,string> HeaderToIso = new(StringComparer.OrdinalIgnoreCase)
    {
        { "en", "en" }, { "english", "en" },
        { "es", "es" }, { "spanish", "es" }, { "español", "es" },
        { "it", "it" }, { "italian", "it" }, { "italiano", "it" },
        { "fr", "fr" }, { "french", "fr" }, { "français", "fr" },
        { "ru", "ru" }, { "russian", "ru" }, { "русский", "ru" },
        { "de", "de" }, { "german", "de" }, { "deutsch", "de" },
        { "pt", "pt" }, { "portuguese", "pt" }, { "português", "pt" },
        { "ja", "ja" }, { "japanese", "ja" }, { "日本語", "ja" },
        { "ko", "ko" }, { "korean", "ko" }, { "한국어", "ko" },
        { "zh-hans", "zh-Hans" }, { "zh_cn", "zh-Hans" }, { "chinese simplified", "zh-Hans" },
        { "zh-hant", "zh-Hant" }, { "zh_tw", "zh-Hant" }, { "chinese traditional", "zh-Hant" },
        { "ar", "ar" }, { "arabic", "ar" }
    };

    [MenuItem("Localization/Sync From Google Sheet (One Sheet Multi-Column)")]
    public static void Open() => GetWindow<LocalizationSyncWindow>("Loc Sync (One Sheet)");

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("The sheet must have columns: key, en, es, it, fr, ...", MessageType.Info);
        fileUrl  = EditorGUILayout.TextField("Publish URL (CSV/TSV)", fileUrl);
        useTsv   = EditorGUILayout.Toggle("Use TSV (safer for commas)", useTsv);
        outFolder = EditorGUILayout.TextField("Output Folder", outFolder);

        if (GUILayout.Button("Sync"))
        {
            try
            {
                Sync();
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Localization", "Sync completed.", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                EditorUtility.DisplayDialog("Localization", "Sync failed. Check console.", "OK");
            }
        }
    }

    private void Sync()
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new Exception("Publish URL is empty");

        Directory.CreateDirectory(outFolder);

        string data;
        using (var wc = new WebClient())
        {
            wc.Encoding = Encoding.UTF8;
            data = wc.DownloadString(fileUrl);
        }
        
        var lines = SplitLines(data);
        if (lines.Count < 2)
            throw new Exception("CSV/TSV empty.");

        var header = SplitRow(lines[0], useTsv ? '\t' : ',');
        if (header.Count < 2 || !header[0].Equals("key", StringComparison.OrdinalIgnoreCase))
            throw new Exception("The first column must be named 'key'.");

        var langCols = new List<(int colIndex, string iso)>();
        for (int c = 1; c < header.Count; c++)
        {
            var h = header[c].Trim();
            if (string.IsNullOrEmpty(h)) continue;

            var key = h.ToLowerInvariant();
            var iso = HeaderToIso.TryGetValue(key, out var mapped)
                ? mapped
                : h;

            langCols.Add((c, iso));
        }

        if (langCols.Count == 0)
            throw new Exception("No language columns found. Expected header: key,en,es,...");

        var perLang = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var (_, iso) in langCols)
            perLang[iso] = new Dictionary<string, string>();

        for (int r = 1; r < lines.Count; r++)
        {
            var row = SplitRow(lines[r], useTsv ? '\t' : ',');
            if (row.Count == 0) continue;

            var key = row[0].Trim();
            if (string.IsNullOrEmpty(key)) continue;

            foreach (var (col, iso) in langCols)
            {
                if (col < row.Count)
                {
                    var val = row[col].Trim();
                    if (!perLang.TryGetValue(iso, out var dict))
                        perLang[iso] = dict = new Dictionary<string, string>();
                    dict[key] = val;
                }
            }
        }

        foreach (var kv in perLang)
        {
            var json = JsonConvert.SerializeObject(kv.Value, Formatting.Indented);
            var path = Path.Combine(outFolder, kv.Key + ".json");
            File.WriteAllText(path, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            Debug.Log($"[Localization] Wrote {path} ({kv.Value.Count} entries)");
        }
    }

    private static List<string> SplitLines(string text)
    {
        var lines = new List<string>();
        using (var sr = new StringReader(text))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
                lines.Add(line);
        }
        return lines;
    }

    private static List<string> SplitRow(string row, char delimiter)
    {
        if (delimiter == '\t')
            return new List<string>(row.Split('\t'));

        var result = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < row.Length; i++)
        {
            char ch = row[i];

            if (ch == '\"')
            {
                if (inQuotes && i + 1 < row.Length && row[i + 1] == '\"')
                {
                    sb.Append('\"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (ch == delimiter && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Length = 0;
            }
            else
            {
                sb.Append(ch);
            }
        }
        result.Add(sb.ToString());
        return result;
    }
}
#endif