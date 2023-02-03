using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class MeetNpcOperation : MovingOperation
    {
        private string npcId = null;

        public MeetNpcOperation(string npcId)
        {
            this.npcId = npcId;
        }

        public override void OnSecondOperationUpdate()
        {
            if (ActorsManager.GetInstance() != null)
            {
                Actor hero = ActorsManager.GetInstance().GetHeroActor();
                Actor npc = ActorsManager.GetInstance().GetActor(npcId);
                if (hero != null && npc != null)
                {
                    if (IsVeryClosed(hero, npc) && IsHeroMovingOrIdle())
                    {
                        isSecondOperationInterrputed = true;
                        SendMeetNpcEvent(true);
                    }
                }
                else
                {
                    isSecondOperationInterrputed = true;
                }
            }
        }

        public override void OnSecondOperationTheEnd()
        {
            if (ActorsManager.GetInstance() != null)
            {
                Actor hero = ActorsManager.GetInstance().GetHeroActor();
                Actor npc = ActorsManager.GetInstance().GetActor(npcId);
                if (hero != null && npc != null)
                {
                    if (npc.IsVisible() && IsHeroMovingOrIdle())
                    {
                        if (IsVeryClosed(hero, npc))
                        {
                            SendMeetNpcEvent(true);
                        }
                        else
                        {
                            SendMeetNpcEvent(false);
                        }
                    }
                }
            }
        }

        private bool IsVeryClosed(Actor hero, Actor npc)
        {
            if (hero == null || npc == null)
            {
                return false;
            }

            Vector3 heroPos = hero.roleAnimation.GetMotionAnimator().GetPosition();
            Vector3 npcPos = npc.roleAnimation.GetMotionAnimator().GetPosition();
            return Vector3.Distance(heroPos, npcPos) < 1.5f;
        }

        private bool IsHeroMovingOrIdle()
        {
            return GetRoleAnimation().GetMotionAnimator().ContainsState(MotionAnimator.State.Moving) ||
                    GetRoleAnimation().GetMotionAnimator().ContainsState(MotionAnimator.State.Idle);
        }

        private void SendMeetNpcEvent(bool isSuccessful)
        {
            EventSystem.GetInstance().Notify(EventID.MeetNpc, new MeetNpcND() { isSuccessful = isSuccessful, npcId = npcId });
        }
    }
}