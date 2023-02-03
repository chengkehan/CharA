using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.Cutscene;

namespace GameScript
{
    public class Main : MonoBehaviour
    {
        public Color testColor;

        private void Awake()
        {
            AssetsManager.GetInstance().Initialize(AssetsManagerInitializedCompleteCB);
        }

        private void AssetsManagerInitializedCompleteCB()
        {
            Utils.Log("Assets Initialized");

            UIManager.GetInstance().Initialize();

            DataCenter.GetInstance().Initialize(() =>
            {
                SceneManager.GetInstance().LoadAIO(() =>
                {
                    UIManager.GetInstance().OpenUI(UIManager.UIName.HUD, () =>
                    {
                        Scene.GetInstance().SpawnItemsAndRoleOnStartup();
                    });
                });
            });
            
        }

        private void OnApplicationQuit()
        {
            EventSystem.GetInstance().Notify(EventID.ApplicationQuit);
        }
    }
}
