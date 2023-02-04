using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using MoonSharp.Interpreter;

namespace GameScript
{
    public class DataCenter
    {
        private const string SAVE_NAME = "DataCenter.json";

        public static readonly string SAVE_DIRECTORY = Application.persistentDataPath;

        public static readonly string SAVE_PATH = SAVE_DIRECTORY + "/" + SAVE_NAME;

        private static Define _define = null;
        public static Define define
        {
            get
            {
                if (_define == null)
                {
                    _define = new Define();
                }
                return _define;
            }
        }

        private static Query _query = null;
        public static Query query
        {
            get
            {
                if (_query == null)
                {
                    _query = new Query();
                }
                return _query;
            }
        }

        private static DataCenter s_instance = null;

        public static DataCenter GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new DataCenter();
            }
            return s_instance;
        }

        private SerializableDictionaryReadOnly<string, RoleConfig> _roleConfigs = null;
        private SerializableDictionaryReadOnly<string, RoleConfig> roleConfigs
        {
            set
            {
                _roleConfigs = value;
            }
            get
            {
                return _roleConfigs;
            }
        }

        private SerializableDictionaryReadOnly<string, LanguageConfig> _languageConfigs = null;
        private SerializableDictionaryReadOnly<string, LanguageConfig> languageConfigs
        {
            set
            {
                _languageConfigs = value;
            }
            get
            {
                return _languageConfigs;
            }
        }

        private SerializableDictionaryReadOnly<string, ItemConfig> _itemConfigs = null;
        private SerializableDictionaryReadOnly<string, ItemConfig> itemConfigs
        {
            set
            {
                _itemConfigs = value;
            }
            get
            {
                return _itemConfigs;
            }
        }

        private PlayerData _playerData = null;
        public PlayerData playerData
        {
            private set
            {
                _playerData = value;
            }
            get
            {
                return _playerData;
            }
        }

        private bool _isInitialized = false;
        public bool isInitialized
        {
            private set
            {
                _isInitialized = value;
            }
            get
            {
                return _isInitialized;
            }
        }

        public void Save()
        {
            playerData.OnSave();
            string json = Utils.Serialize(playerData);
            File.WriteAllText(SAVE_PATH, json);
        }

        public bool Initialize(Action completeCB)
        {
            if (isInitialized)
            {
                return false;
            }
            isInitialized = true;

            AssetsManager.GetInstance().LoadAsset<TextAsset>(AssetsManager.ROLE_CONFIG, (obj) =>
            {
                roleConfigs = Utils.Deserialize<SerializableDictionaryReadOnly<string, RoleConfig>>(obj.text);

                AssetsManager.GetInstance().LoadAsset<TextAsset>(AssetsManager.LANGUAGE_CONFIG, (obj) =>
                {
                    languageConfigs = Utils.Deserialize<SerializableDictionaryReadOnly<string, LanguageConfig>>(obj.text);

                    AssetsManager.GetInstance().LoadAsset<TextAsset>(AssetsManager.ITEM_CONFIG, (obj) =>
                    {
                        itemConfigs = Utils.Deserialize<SerializableDictionaryReadOnly<string, ItemConfig>>(obj.text);

                        Utils.Log("Data Center Initialized");
                        completeCB?.Invoke();
                    });
                });
            });

            return true;
        }

        // This is a test for dynamic instruction
        public bool IsHeroObserving()
        {
            return true;
        }

        private DataCenter()
        {
            if (File.Exists(SAVE_PATH))
            {
                string json = File.ReadAllText(SAVE_PATH);
                playerData = Utils.Deserialize<PlayerData>(json);
            }
            else
            {
                playerData = new PlayerData();
            }
            playerData.OnLoad();
        }

        #region Get RoleConfig

        private RoleConfig _nullRoleConfig = new RoleConfig();

        public RoleConfig GetRoleConfig(string id)
        {
            if (query.IsHeroRoleIdSimplified(id))
            {
                id = AssetsManager.HERO_ROLE_ID;
            }
            if (roleConfigs.TryGetValue(id, out var roleConfig))
            {
                return roleConfig;
            }
            else
            {
                return _nullRoleConfig;
            }
        }

        #endregion

        private bool ContainsLanguage(string key)
        {
            return languageConfigs != null && string.IsNullOrWhiteSpace(key) == false && languageConfigs.ContainsKey(key);
        }

        public string GetLanguage(string key)
        {
            if (ContainsLanguage(key))
            {
                return languageConfigs[key].Selector();
            }
            else
            {
                return string.Empty;
            }
        }

        #region Get ItemConfig

        private ItemConfig _nullItemConfig = new ItemConfig();

        public bool ContainsItemConfig(string id)
        {
            return itemConfigs.ContainsKey(id);
        }

        public ItemConfig GetItemConfig(string id)
        {
            if (itemConfigs.TryGetValue(id, out var itemConfig))
            {
                return itemConfig;
            }
            else
            {
                return _nullItemConfig;
            }
        }

        #endregion
    }
}