using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class SceneNode : MonoBehaviour
    {
        [SerializeField]
        private SceneManager.SceneNames _sceneName = SceneManager.SceneNames.Undefined;
        public SceneManager.SceneNames sceneName
        {
            get
            {
                return _sceneName;
            }
        }

        [SerializeField]
        private GameObject _staticNode = null;
        private GameObject staticNode
        {
            get
            {
                return _staticNode;
            }
        }

        // using for preview building model in editor only
        // don't access at runtime
        [HideInInspector]
        [SerializeField]
        private string _buildingAssetPath = null;

        [SerializeField]
        private Vector3 _cameraEulerAngles = Vector3.zero;
        public Vector3 cameraEulerAngles
        {
            get
            {
                return _cameraEulerAngles;
            }
        }

        [SerializeField]
        private GlobalEffectVolume _globalEffectVolume = new GlobalEffectVolume();
        public GlobalEffectVolume globalEffectVolume
        {
            get
            {
                return _globalEffectVolume;
            }
        }

        [SerializeField]
        private BoundsComponent cameraArea = new BoundsComponent();
        public Bounds cameraAreaBounds
        {
            get
            {
                return cameraArea.bounds;
            }
        }

        [SerializeField]
        private BoundsComponent sceneBox = new BoundsComponent();
        public Bounds sceneBoxBounds
        {
            get
            {
                return sceneBox.bounds;
            }
        }

        private void Awake()
        {
            Utils.Assert(HasSceneGameObject() == false, "Static scene node is not empty");
        }

        private void Update()
        {
            if (ActorsManager.GetInstance() != null)
            {
                var heroActor = ActorsManager.GetInstance().GetHeroActor();
                if (heroActor != null)
                {
                    if (sceneBox.bounds.Contains(heroActor.roleAnimation.GetMotionAnimator().GetPosition()))
                    {
                        if (SceneManager.GetInstance().CurrentSceneNode() != this)
                        {
                            SceneManager.GetInstance().LoadScene(sceneName, null); 
                        }
                    }
                }
            }
        }

        public bool HasSceneGameObject()
        {
            return GetSceneGameObject() != null;
        }

        public GameObject GetSceneGameObject()
        {
            if (staticNode == null && staticNode.transform != null || staticNode.transform.childCount == 0)
            {
                return null;
            }
            else
            {
                return staticNode.transform.GetChild(0).gameObject;
            }
        }

        public void SetSceneGameObject(GameObject sceneGo)
        {
            if (staticNode != null && staticNode.transform != null && sceneGo != null && sceneGo.transform != null)
            {
                sceneGo.transform.SetParent(staticNode.transform, false);
            }
        }
    }
}
