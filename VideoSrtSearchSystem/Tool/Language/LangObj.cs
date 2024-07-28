using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Text;
using VideoSrtSearchSystem.Config;

namespace VideoSrtSearchSystem.Tool.Language
{
    public static class LangObj
    {
        public static ConcurrentDictionary<string, ConcurrentDictionary<string, string>> langDict = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        public static string lang = Lang.EN;

        public static void LoadLangData(string? langInput)
        {
            string text = lang;
            if (langInput != null)
            {
                text = langInput;
            }

            if (langDict.ContainsKey(text))
            {
                return;
            }

            string format = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource", "Language", "{0}.json");
            string path = string.Format(format, text);
            if (!File.Exists(path))
            {
                path = string.Format(format, Lang.EN);
            }

            JObject jObject = JObject.Parse(File.ReadAllText(path, Encoding.GetEncoding("utf-8")));
            langDict.TryAdd(text, new ConcurrentDictionary<string, string>());
            foreach (KeyValuePair<string, JToken> item in jObject)
            {
                string key = item.Key.ToString();
                if (!langDict[text].ContainsKey(key))
                {
                    try
                    {
                        langDict[text].TryAdd(key, item.Value.ToString());
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }
        }
    }
}
