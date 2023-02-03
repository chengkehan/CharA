using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

namespace GameScript.UI.HUD
{
    public class TalkingBubblesHUD : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text = null;
        private TMP_Text text
        {
            get
            {
                return _text;
            }
        }

        private string _actorGUID = null;
        public string actorGUID
        {
            private set
            {
                _actorGUID = value;
            }
            get
            {
                return _actorGUID;
            }
        }

        private float duration = 0;

        private List<string> recentText = new List<string>();

        public void Show(string actorGUID, string text, float duration)
        {
            this.duration = duration;
            this.actorGUID = actorGUID;

            if (recentText.Count >= 3/*display recent 3 items*/)
            {
                recentText.RemoveAt(0);
            }
            recentText.Add(text == null ? string.Empty : text);

            string txt = recentText[recentText.Count - 1];
            for (int i = recentText.Count - 2; i >= 0; i--)
            {
                txt = "<#888888>" + recentText[i] + "</color>" + "\n" + txt;
            }

            this.text.text = txt;

            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (transform != null && transform.parent != null &&
                CameraManager.GetInstance() != null &&
                CameraManager.GetInstance().GetMainCamera() != null &&
                CameraManager.GetInstance().GetUICamera() != null)
            {
                var actorPD = DataCenter.GetInstance().playerData.GetSerializableMonoBehaviourPD<ActorPD>(actorGUID);
                var actor = ActorsManager.GetInstance().GetActorByGUID(actorGUID);
                var container = transform.parent.GetComponent<RectTransform>();
                if (container != null)
                {
                    var footPosition = actorPD.position;
                    var headPosition = actor.GetHeadPointPosition();
                    var wPos = footPosition + new Vector3(0, headPosition.y - footPosition.y, 0);

                    Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
                        CameraManager.GetInstance().GetMainCamera(), wPos
                    );

                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(container, screenPoint, CameraManager.GetInstance().GetUICamera(), out Vector2 localPoint))
                    {
                        GetComponent<RectTransform>().anchoredPosition = localPoint;
                    }
                }
            }
        }

        private void Update()
        {
            UpdatePosition();

            if (duration < 0)
            {
                Destroy(gameObject);
            }
            duration -= Time.deltaTime;
        }
    }
}
