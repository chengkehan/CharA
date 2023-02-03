using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript.Cutscene
{
    public class RoleSpawn : MonoBehaviour
    {
        [SerializeField]
        private string _roleID = null;
        private string roleID
        {
            get
            {
                return _roleID;
            }
        }

        public void Spawn()
        {
            ActorsManager.GetInstance().LoadRole(roleID, LoadRoleCompleteCB);
        }

        private void LoadRoleCompleteCB()
        {
            Vector3 rolePosition = Vector3.zero;
            Vector3 roleFaceTo = Vector3.zero;

            var actor = ActorsManager.GetInstance().GetActor(roleID);
            if (actor.pd.initialized.o == false)
            {
                actor.pd.initialized.o = true;

                rolePosition = transform.position;
                roleFaceTo = transform.forward;

                if (DataCenter.query.IsHeroRoleID(roleID))
                {
                    actor.roleAnimation.GetMotionAnimator().SetSoloState(SoloSM.Transition.StandUp);
                    StartCoroutine(HeadacheAnimationCoroutine(actor));
                }
            }
            else
            {
                rolePosition = actor.pd.position;
                if (Random.value < 0.5f)
                {
                    roleFaceTo = Vector3.right;
                }
                else
                {
                    roleFaceTo = Vector3.left;
                }
            }
            rolePosition = AdjustInitialPosition(rolePosition);

            actor.roleAnimation.GetMotionAnimator().SetPosition(rolePosition);
            actor.roleAnimation.GetMotionAnimator().SetForward(roleFaceTo);

            if (actor.pd.inHandItem.IsEmpty() == false)
            {
                actor.SetInHandItem(actor.pd.inHandItem);
            }
        }

        private IEnumerator HeadacheAnimationCoroutine(Actor actor)
        {
            yield return new WaitForSeconds(7.0f);
            actor.roleAnimation.GetMotionAnimator().SetUpBodyAnimationLayer2(MotionAnimator.UpBodyAnimationLayer2.Headache);
            yield return new WaitForSeconds(5.0f);
            actor.roleAnimation.GetMotionAnimator().SetUpBodyAnimationLayer2(MotionAnimator.UpBodyAnimationLayer2.None);
        }

        // when a role is created, role should be stand on a pure moving type waypoint,
        // so we must adjust initial position.
        private Vector3 AdjustInitialPosition(Vector3 position)
        {
            Waypoint eligibleWaypoint = null;
            float distance = 999999;
            for (int i = 0; i < Waypoint.NumberWaypoints(); i++)
            {
                var waypoint = Waypoint.GetWaypoint(i);
                if (waypoint != null)
                {
                    var d = Vector3.Distance(position, waypoint.GetPosition());
                    if (d < distance &&
                        waypoint.type == Waypoint.Type.Moving &&
                        waypoint.IsDoor() == false && waypoint.IsPortal() == false)
                    {
                        distance = d;
                        eligibleWaypoint = waypoint;
                    }
                }
            }
            if (eligibleWaypoint == null)
            {
                return position;
            }
            else
            {
                return eligibleWaypoint.GetPosition();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Color gizmosColor = Gizmos.color;
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, 0.25f);
            }
            Gizmos.color = gizmosColor;
        }
#endif
    }
}
