using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [DisallowMultipleComponent]
    public class CameraMotion : MonoBehaviour
    {
        [SerializeField]
        private Transform _cameraTransform = null;
        private Transform cameraTransform
        {
            get
            {
                return _cameraTransform;
            }
        }

        private bool initialized = false;

        private void Awake()
        {
            EventSystem.GetInstance().AddListener(EventID.SceneLoaded, SceneLoadedHandler);
        }

        private void OnDestroy()
        {
            EventSystem.GetInstance().RemoveListener(EventID.SceneLoaded, SceneLoadedHandler);
        }

        private void SceneLoadedHandler(NotificationData _data)
        {
            var data = _data as SceneLoadedND;
            if (data != null)
            {
                initialized = false;

                var sceneNode = SceneManager.GetInstance().CurrentSceneNode();
                cameraTransform.eulerAngles = sceneNode.cameraEulerAngles;
            }
        }

        private void FixedUpdate()
        {
            if (ActorsManager.GetInstance() != null && ActorsManager.GetInstance().GetHeroActor() != null)
            {
                Vector3 myPosition = cameraTransform.position;
                Vector3 targetPosition = ActorsManager.GetInstance().GetHeroActor().roleAnimation.GetMotionAnimator().GetPosition();

                bool xRestricted = true;
                if (UIManager.GetInstance().ContainsUI(UIManager.UIName.CardboardBox))
                {
                    xRestricted = false;
                    targetPosition.x += 2;
                }

                targetPosition.z = myPosition.z;
                targetPosition.y += 1;

                if (initialized)
                {
                    Vector3 v = Vector3.zero;
                    Vector3 tweenPosition = Vector3.SmoothDamp(myPosition, targetPosition, ref v, 0.025f, 10);

                    if (SceneManager.GetInstance() != null)
                    {
                        var sceneNode = SceneManager.GetInstance().CurrentSceneNode();
                        if (sceneNode != null)
                        {
                            var camAreaBounds = sceneNode.cameraAreaBounds;
                            var min = camAreaBounds.min;
                            var max = camAreaBounds.max;
                            if (xRestricted)
                            {
                                tweenPosition.x = Mathf.Clamp(tweenPosition.x, min.x, max.x);
                            }
                            tweenPosition.y = Mathf.Clamp(tweenPosition.y, min.y, max.y);
                            //tweenPosition.z = Mathf.Clamp(tweenPosition.z, min.z, max.z);
                        }
                    }

                    cameraTransform.position = tweenPosition;
                }
                else
                {
                    initialized = true;
                    cameraTransform.position = targetPosition;
                }
            }
        }
    }
}
