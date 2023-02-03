using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [DisallowMultipleComponent]
    public class ViewField : MonoBehaviour
    {
        // all points must be in the same plane
        // The amount must be four
        // Four points form a quadrilateral
        [SerializeField]
        private Transform[] _fourPoints = null;
        private Transform[] fourPoints
        {
            get
            {
                return _fourPoints;
            }
        }

        private Plane plane; 

        private Vector3[] crossProd = new Vector3[4];

        public bool IsEnabled()
        {
            return this.enabled;
        }

        public bool CanBeSeenExtendedViewField(Vector3 testPos)
        {
            if (IsValid() == false)
            {
                return false;
            }

            if (ActorsManager.GetInstance() != null && ActorsManager.GetInstance().GetHeroActor() != null)
            {
                Vector3 eyePos = ActorsManager.GetInstance().GetHeroActor().GetHeadPosition(); 
                if (plane.SameSide(eyePos, testPos) == false)
                {
                    Vector3 rayDir = testPos - eyePos;
                    rayDir.Normalize();

                    //Vector3 headDirection = heroActor.GetHeadDirection();
                    // Ignore blind area of back side
                    //if (Vector3.Dot(headDirection, rayDir) > -0.45f/*blind area*/)
                    {
                        Ray ray = new Ray(eyePos, rayDir);
                        if (plane.Raycast(ray, out float enter))
                        {
                            Vector3 hitPos = eyePos + rayDir * enter;
                            for (int i = 0; i < fourPoints.Length; i++)
                            {
                                Vector3 v1 = hitPos - fourPoints[i].position;
                                Vector3 v2;
                                if (i == fourPoints.Length - 1)
                                {
                                    v2 = fourPoints[0].position - fourPoints[i].position;
                                }
                                else
                                {
                                    v2 = fourPoints[i + 1].position - fourPoints[i].position;
                                }
                                crossProd[i] = Vector3.Cross(v1, v2);
                            }

                            bool isAtTheSameSide = true;
                            Vector3 refPos = hitPos + crossProd[0];
                            for (int i = 1; i < crossProd.Length; i++)
                            {
                                if (plane.SameSide(refPos, hitPos + crossProd[i]) == false)
                                {
                                    isAtTheSameSide = false;
                                    break;
                                }
                            }

                            if (isAtTheSameSide)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void OnValidate()
        {
            CreatePlane();
        }

        private void Awake()
        {
            CreatePlane();
        }

        private void CreatePlane()
        {
            if (IsValid())
            {
                plane = new Plane(fourPoints[0].position, fourPoints[1].position, fourPoints[2].position);
            }
        }

        private bool IsValid()
        {
            return fourPoints != null && fourPoints.Length == 4 && fourPoints[0] != null && fourPoints[1] != null && fourPoints[2] != null && fourPoints[3] != null;
        }

        private void OnDrawGizmosSelected()
        {
            if (IsValid() == false)
            {
                return;
            }

            if (ActorsManager.GetInstance() != null && ActorsManager.GetInstance().GetHeroActor() != null)
            {
                float scale = 4;
                Vector3 eyePos = ActorsManager.GetInstance().GetHeroActor().GetHeadPosition();
                if (fourPoints != null)
                {
                    for (int i = 0; i < fourPoints.Length; i++)
                    {
                        var point = fourPoints[i];
                        if (point != null)
                        {
                            Vector3 dir = (point.position - eyePos) * scale;
                            Vector3 pos = eyePos + dir;
                            Gizmos.DrawLine(eyePos, pos);

                            var nextPoint = i == fourPoints.Length - 1 ? fourPoints[0] : fourPoints[i + 1];
                            if (nextPoint != null)
                            {
                                Vector3 dir2 = (nextPoint.position - eyePos) * scale;
                                Vector3 pos2 = eyePos + dir2;
                                Gizmos.DrawLine(pos, pos2);
                            }
                        }
                    }
                }
            }
        }
    }
}