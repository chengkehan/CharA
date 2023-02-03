
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class LanguageConfig
    {

        [SerializeField]
        private string _key = string.Empty;
        public string key
        {
            get
            {
                return _key;
            }
        }

        [SerializeField]
        private string _chs = string.Empty;
        public string chs
        {
            get
            {
                return _chs;
            }
        }

        public LanguageConfig (string key, string chs)
        {
            _key = key;
            _chs = chs;
        }

        public LanguageConfig ()
        {
            _key = string.Empty;
            _chs = string.Empty;
        }
        private string[] blocks = null;

        private int blockIndex = 0;

        private int blockIndexStep = 0;

        public string Selector()
        {
            string language = chs;
            if (blockIndex == -1)
            {
                return language;
            }
            else
            {
                if (blocks == null)
                {
                    if (language.Contains("|"))
                    {
                        blocks = language.Split('|');
                        blockIndex = UnityEngine.Random.Range(0, blocks.Length);
                        blockIndexStep = UnityEngine.Random.Range(1, blocks.Length);
                    }
                    else
                    {
                        blockIndex = -1;
                        return language;
                    }
                }

                var txt = blocks[blockIndex];
                blockIndex += blockIndexStep;
                blockIndex %= blocks.Length;
                return txt;
            }
        }

    }
}
