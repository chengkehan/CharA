using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.UI.HUD
{
    public class HexagonCoordinate : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas = null;
        private Canvas canvas
        {
            get
            {
                return _canvas;
            }
        }

        private RectTransform _canvasRectTransform = null;
        private RectTransform canvasRectTransform
        {
            get
            {
                if (_canvasRectTransform == null)
                {
                    _canvasRectTransform = canvas == null ? null : canvas.GetComponent<RectTransform>();
                }
                return _canvasRectTransform;
            }
        }

        private Vector2[,] points = new Vector2[5, 5];

        private readonly float xO = -200;
        private readonly float yO = -100;
        private readonly float pointSize = 50;

        private Vector3 AnchoredPositionToWorldPosition(Vector2 anchoredPosition)
        {
            Vector2 canvasSize = canvasRectTransform.sizeDelta;

            Vector2 viewportPosition = new Vector2(
                (anchoredPosition.x + (canvasSize.x * 0.5f)) / canvasSize.x,
                (anchoredPosition.y + (canvasSize.y * 0.5f)) / canvasSize.y
            );
            Vector3 worldPosition = CameraManager.GetInstance().GetUICamera().ViewportToWorldPoint(viewportPosition);
            worldPosition.z = canvasRectTransform.position.z;

            return worldPosition;
        }

        private void Awake()
        {
            // Initialize points
            {
                float x = xO;
                float y = yO;
                for (int i = 0; i < points.GetLength(0); i++)
                {
                    for (int j = 0; j < points.GetLength(1); j++)
                    {
                        points[i, j] = new Vector2(x, y);
                        x += pointSize * 2;
                    }
                    y += pointSize * 2;
                    x = xO;
                }
            }
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                if (CameraManager.GetInstance() != null && CameraManager.GetInstance().GetUICamera() != null &&
                    canvasRectTransform != null)
                {
                    // Draw Points
                    {
                        for (int i = 0; i < points.GetLength(0); i++)
                        {
                            for (int j = 0; j < points.GetLength(1); j++)
                            {
                                Gizmos.DrawWireSphere(
                                    AnchoredPositionToWorldPosition(points[i, j]), 0.4f
                                );
                            }
                        }
                    }
                }
            }
        }
#endif
    }
}
