using VideoSrtSearchSystem.Config;

namespace VideoSrtSearchSystem.Tool.Language
{
    public static class LangTool
    {
        private static readonly object lockMe = new object();

        public static void SetLang(string lang)
        {
            LangObj.lang = lang;
            LangObj.LoadLangData(lang);
        }

        public static string GetLang()
        {
            return LangObj.lang;
        }

        public static string GetTranslation(string keyStr, string? lang = null)
        {
            try
            {
                if (keyStr.Length <= 0)
                {
                    return string.Empty;
                }

                lock (lockMe)
                {
                    if (lang == null)
                    {
                        lang = LangObj.lang;
                    }

                    if (!LangObj.langDict.ContainsKey(lang))
                    {
                        LangObj.LoadLangData(lang);
                    }

                    if (LangObj.langDict[lang].ContainsKey(keyStr))
                    {
                        return LangObj.langDict[lang][keyStr];
                    }
                }

                return string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool CheckLang(string lang)
        {
            return Lang.GetPropertys().Contains(lang);
        }
    }
}
