using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using XNode;
using GameScript;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StoryboardCore
{
    [Serializable, CreateAssetMenu(fileName = "NewStoryboard", menuName = "Storyboard")]
    public class Storyboard : NodeGraph
    {
        [ReadOnly]
        public string guid = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (string.IsNullOrWhiteSpace(guid))
            {
                guid = Guid.NewGuid().ToString();
            }
        }

        // Runtime context.
        // This Storyboard working in which StoryThread at runtime.
        // When game is not playing, it's null
        private StoryThread _storyThread = null;
        public StoryThread storyThread
        {
            set
            {
                _storyThread = value;
            }
            get
            {
                return _storyThread;
            }
        }

        #region Language

        public TextAsset languageData = null;

        public string GetLanguage(string key)
        {
            if (ContainsLanguage(key))
            {
                return languages[key].Selector();
            }
            else
            {
                return string.Empty;
            }
        }

        public bool ContainsLanguage(string key)
        {
            return languages != null && string.IsNullOrWhiteSpace(key) == false && languages.ContainsKey(key);
        }

#if UNITY_EDITOR
        private DateTime languageDataLastWriteTime = DateTime.Now;
#endif

        private SerializableDictionary<string, LanguageConfig> _languages = null;
        private SerializableDictionary<string, LanguageConfig> languages
        {
            get
            {
                if (languageData == null)
                {
                    return null;
                }
                else
                {
                    // Reload language if source file is modified.
                    // Just check this in editor, it's impossible to be modified in player.
                    bool isModifiedInEditor = false;
#if UNITY_EDITOR
                    var languageDataAssetPath = AssetDatabase.GetAssetPath(languageData);
                    var fileInfo = new FileInfo(languageDataAssetPath);
                    if (fileInfo.LastWriteTime != languageDataLastWriteTime)
                    {
                        isModifiedInEditor = true;
                        languageDataLastWriteTime = fileInfo.LastWriteTime;
                    }
#endif

                    if (_languages == null || isModifiedInEditor)
                    {
                        _languages = Utils.Deserialize<SerializableDictionary<string, LanguageConfig>>(languageData.text);
                    }

                    return _languages;
                }
            }
        }

        #endregion
    }
}
