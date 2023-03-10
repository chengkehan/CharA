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
        public static OutlineObject recentOutlineObject
        {
            private set
            {
                _recentOutlineObject = value;
            }
            get
            {
                return _recentOutlineObject;
            }
        }

        private static void RecentOutlineObjectShow()
        {
            if (recentOutlineObject != null)
            {
                recentOutlineObject.ShowOutline();
            }
        }

        public static void RecentOutlineObjectHide()
        {
            if (recentOutlineObject != null)
            {
                recentOutlineObject.HideOutline();
                recentOutlineObject = null;
            }
        }

        public static void DetectOutlineObject(int layerMask = (int)Define.LayersMask.Interactive3D, Camera camera = null, bool depthOcclusion = false)
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
                        if (depthOcclusion == false)
                        {
                            isSelected = Select(out outline);
                        }
                        else
                        {
                            for (int i = 0; i < hitInfoBufferSize; i++)
                            {
                                if (Select(out outline, i))
                                {
                                    if (outline == null)
                                    {       
                                        break;
                                    }
                                    else
                                    {
                                        if (outline.enabled)
                                        {
                                            isSelected = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (isSelected && outline != null)
                        {
                            if (outline.enabled)
                            {
                                if (outline != recentOutlineObject)
                                {
                                    RecentOutlineObjectHide();

                                    recentOutlineObject = outline;
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

        private static bool Select<T>(out T o, int index) where T : MonoBehaviour
        {
            o = null;
            if (index >= 0 && index < hitInfoBufferSize)
            {
                if (SelectCheckHitInfo(out o, GetHitInfoBuffer()[index], index))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Select<T>(out T o) where T : MonoBehaviour
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

        private static bool SelectCheckHitInfo<T>(out T o, RaycastHit hitInfo, int hitInfoIndex) where T : MonoBehaviour
        {
            o = null;
            if (hitInfo.transform != null)
            {
                var mono = hitInfo.transform.GetComponent<T>();
                if (mono != null)
                {
                    o = mono;
                    hitInfoSelected = hitInfoIndex;
                    return true;
                }
            }
            if (hitInfo.transform != null && hitInfo.transform.parent != null)
            {
                var mono = hitInfo.transform.parent.GetComponent<T>();
                if (mono != null)
                {
                    o = mono;
                    hitInfoSelected = hitInfoIndex;
                    return true;
                }
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
