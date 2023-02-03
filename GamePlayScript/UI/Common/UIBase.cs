using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.UI.Common
{
    public class UIBase : MonoBehaviour
    {
        #region Language

        [SerializeField]
        private TextAsset _languageConfig = null;
        private TextAsset languageConfig
        {
            get
            {
                return _languageConfig;
            }
        }

        [SerializeField]
        private TextAsset _languageConfigFallback = null;
        private TextAsset languageConfigFallback
        {
            get
            {
                return _languageConfigFallback;
            }
        }

        private SerializableDictionaryReadOnly<string, LanguageConfig> _languages = null;
        private SerializableDictionaryReadOnly<string, LanguageConfig> languages
        {
            get
            {
                if (_languages == null)
                {
                    _languages = Utils.Deserialize<SerializableDictionary<string, LanguageConfig>>(languageConfig.text);
                }
                return _languages;
            }
        }

        private SerializableDictionaryReadOnly<string, LanguageConfig> _languagesFallback = null;
        private SerializableDictionaryReadOnly<string, LanguageConfig> languagesFallback
        {
            get
            {
                if (_languagesFallback == null)
                {
                    _languagesFallback = Utils.Deserialize<SerializableDictionary<string, LanguageConfig>>(languageConfigFallback.text);
                }
                return _languagesFallback;
            }
        }

        public string GetLanguage(string key)
        {
            if (ContainsLanguage(key))
            {
                return languages[key].Selector();
            }
            else
            {
                if (ContainsFallbackLanguage(key))
                {
                    return languagesFallback[key].Selector();
                }
                else
                {
                    return DataCenter.GetInstance().GetLanguage(key);
                }
            }
        }

        public bool ContainsLanguage(string key)
        {
            return languageConfig != null && languages != null && string.IsNullOrWhiteSpace(key) == false && languages.ContainsKey(key);
        }

        public bool ContainsFallbackLanguage(string key)
        {
            return languageConfigFallback != null && languagesFallback != null && string.IsNullOrWhiteSpace(key) == false && languagesFallback.ContainsKey(key);
        }

        #endregion
    }
}
