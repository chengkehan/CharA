using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameScript.Cutscene;

namespace GameScript
{
    public class SceneManager
    {
        public enum SceneNames
        {
            Undefined, 
            AmisterNo3,
            RuinsPlan,
            BludgerSide,
            TwoFloors,
            SafeHouse
        }

        public static SceneManager s_instance = null;

        public static SceneManager GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new SceneManager();
            }
            return s_instance;
        }

        private SceneNames _sceneName = SceneNames.Undefined;
        public SceneNames sceneName
        {
            private set
            {
                _sceneName = value;
            }
            get
            {
                return _sceneName;
            }
        }

        private SceneNode[] _allSceneNode = null;
        private SceneNode[] allSceneNode
        {
            get
            {
                if (_allSceneNode == null)
                {
                    _allSceneNode = GameObject.FindObjectsOfType<SceneNode>(true);
                }
                return _allSceneNode;
            }
        }

        public void LoadAIO(Action completeCB)
        {
            AssetsManager.GetInstance().LoadScene("AIO", completeCB);
        }

        public SceneNode CurrentSceneNode()
        {
            foreach (var sceneNode in allSceneNode)
            {
                if (sceneNode != null)
                {
                    if (sceneNode.sceneName == sceneName)
                    {
                        return sceneNode;
                    }
                }
            }
            return null;
        }

        public void LoadScene(SceneNames sceneName, Action completeCB)
        {
            Utils.Assert(this.sceneName != sceneName);

            this.sceneName = sceneName;
            AssetsManager.GetInstance().LoadGameObject(AssetsManager.BUILDING_PREFAB_PREFIX + sceneName.ToString(), (obj)=>
            {
                foreach (var sceneNode in allSceneNode)
                {
                    if (sceneNode != null)
                    {
                        if (sceneNode.sceneName == sceneName)
                        {
                            sceneNode.SetSceneGameObject(obj);
                        }
                        else
                        {
                            AssetsManager.GetInstance().UnloadGameObject(sceneNode.GetSceneGameObject());
                        }
                    }
                }

                SceneRenderer.GetInstance().RefreshCurrentSceneMaterials();
                completeCB?.Invoke();

                var notificationData = new SceneLoadedND();
                notificationData.loadedSceneName = this.sceneName;
                EventSystem.GetInstance().Notify(EventID.SceneLoaded, notificationData);
            });
        }
    }
}
