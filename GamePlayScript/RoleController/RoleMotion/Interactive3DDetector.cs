using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.Cutscene;

namespace GameScript
{
    public class Interactive3DDetector
    {
        #region Outline object

        private static OutlineObject _recentOutlineObject = null;

        private static void RecentOutlineObjectShow()
        {
            if (_recentOutlineObject != null)
            {
                _recentOutlineObject.ShowOutline();
            }
        }

        private static void RecentOutlineObjectHide()
        {
            if (_recentOutlineObject != null)
            {
                _recentOutlineObject.HideOutline();
                _recentOutlineObject = null;
            }
        }

        public static void DetectOutlineObject(int layerMask = (int)Define.LayersMask.Interactive3D, Camera camera = null, bool selectNearest = false)
        {
            if (CameraManager.GetInstance() != null && CameraManager.GetInstance().GetMainCamera() != null)
            {
                Camera cam = camera == null ? CameraManager.GetInstance().GetMainCamera() : camera;
                if (cam != null)
                {
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    if (Detect(ray, layerMask))
                    {
                        bool isSelected = false;
                        OutlineObject outline = null;
                        if (selectNearest)
                        {
                            isSelected = SelectNearest(out outline);
                        }
                        else
                        {
                            isSelected = Select(out outline);
                        }
                        if (isSelected && outline != null)
                        {
                            if (outline.enabled && outline != _recentOutlineObject)
                            {
                                RecentOutlineObjectHide();

                                _recentOutlineObject = outline;
                                RecentOutlineObjectShow();
                            }
                        }
                        else
                        {
                            RecentOutlineObjectHide();
                        }
                    }
                    else
                    {
                        RecentOutlineObjectHide();
                    }
                }
            }
        }

        #endregion

        // Detecting all hitpoints in 3D with a ray.
        public static bool Detect(Ray ray, int layerMask = (int)Define.LayersMask.Interactive3D)
        {
            hitInfoSelected = -1;
            int numHits = Physics.RaycastNonAlloc(ray, GetHitInfoBuffer(), 999, layerMask);
            if (numHits > 0)
            {
                hitInfoBufferSize = numHits;
                System.Array.Sort(GetHitInfoBuffer(), 0, numHits, GetHitInfoSortingComparer());
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SelectNearest<T>(out T o) where T : Component
        {
            o = null;
            if (hitInfoBufferSize > 0)
            {
                if (SelectCheckHitInfo(out o, GetHitInfoBuffer()[0], 0))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Select<T>(out T o) where T : Component
        {
            o = null;
            for (int i = 0; i < hitInfoBufferSize; i++)
            {
                if (SelectCheckHitInfo(out o, GetHitInfoBuffer()[i], i)) 
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SelectCheckHitInfo<T>(out T o, RaycastHit hitInfo, int hitInfoIndex) where T : Component
        {
            o = null;
            if (hitInfo.transform != null && hitInfo.transform.GetComponent<T>() != null)
            {
                o = hitInfo.transform.GetComponent<T>();
                hitInfoSelected = hitInfoIndex;
                return true;
            }
            if (hitInfo.transform != null && hitInfo.transform.parent != null && hitInfo.transform.parent.GetComponent<T>() != null)
            {
                o = hitInfo.transform.parent.GetComponent<T>();
                hitInfoSelected = hitInfoIndex;
                return true;
            }
            return false;
        }

        public static Vector3 GetHitPointOfRay()
        {
            if (hitInfoSelected != -1)
            {
                return GetHitInfoBuffer()[hitInfoSelected].point;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public static Transform GetHitTargetTransform()
        {
            if (hitInfoSelected != -1)
            {
                return GetHitInfoBuffer()[hitInfoSelected].transform;
            }
            else
            {
                return null;
            }
        }

        public static BoxCollider GetHitBoxCollider()
        {
            if (hitInfoSelected != -1)
            {
                return GetHitInfoBuffer()[hitInfoSelected].collider as BoxCollider;
            }
            else
            {
                return null;
            }
        }

        public static bool TryGetHitBoxColliderBounds(out Bounds localBounds, out Matrix4x4 worldToLocalBounds)
        {
            var hitBoxCollider = GetHitBoxCollider();
            if (hitBoxCollider == null)
            {
                localBounds = new Bounds(Vector3.zero, Vector3.one);
                worldToLocalBounds = Matrix4x4.identity;
                return false;
            }
            else
            {
                var hitBoxColliderTransform = hitBoxCollider.transform;
                localBounds = new Bounds(Vector3.zero, hitBoxCollider.size);
                worldToLocalBounds = hitBoxColliderTransform.worldToLocalMatrix;
                return true;
            }
        }

        #region HitInfoBuffer

        private static RaycastHit[] _hitInfoBuffer = null;

        private static int _hitInfoBufferSize = 0;

        private static int _hitInfoSelected = -1;

        private static HitInfoSortingComparer _hitInfoSortingComparer = null;

        private static int hitInfoSelected
        {
            set
            {
                _hitInfoSelected = value;
            }
            get
            {
                return _hitInfoSelected;
            }
        }

        private static int hitInfoBufferSize
        {
            set
            {
                _hitInfoBufferSize = value;
            }
            get
            {
                return _hitInfoBufferSize;
            }
        }

        private static RaycastHit[] GetHitInfoBuffer()
        {
            if (_hitInfoBuffer == null)
            {
                _hitInfoBuffer = new RaycastHit[5];
            }
            return _hitInfoBuffer;
        }

        private static HitInfoSortingComparer GetHitInfoSortingComparer()
        {
            if (_hitInfoSortingComparer == null)
            {
                _hitInfoSortingComparer = new HitInfoSortingComparer();
            }
            return _hitInfoSortingComparer;
        }

        public class HitInfoSortingComparer : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit x, RaycastHit y)
            {
                return x.distance > y.distance ? 1 : -1;
            }
        }

        #endregion
    }
}
