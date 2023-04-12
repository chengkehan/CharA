using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class SoloSM : ActionStateMachine
    {
        public enum Transition
        {
            Undefined = 0,
            StandUp = 1,
            StandingToCrouched = 2,
            CrouchedToStanding = 3,
            SittingGroundDown = 4,
            SittingGround = 5,
            SittingGroundUp = 6,
            Dynamic = 7
        }

        protected override int InitializeActionNameId()
        {
            return Animator.StringToHash("Solo");
        }

        protected override int GetAction(string clipName)
        {
            return Utils.EnumToValue(Utils.StringToEnum<Transition>(clipName));
        }

        protected override void InitializeSequenceActions()
        {
            base.InitializeSequenceActions();

            AddSequenceAction(Transition.SittingGroundDown, Transition.SittingGround);
        }

        protected override void InitializeCompleteTimeOfActions()
        {
            base.InitializeCompleteTimeOfActions();

            AddCompleteTimeOfAction(Transition.SittingGround, 0.25f);
        }

        protected override void InitializedEqualActions()
        {
            base.InitializedEqualActions();

            AddEuqalActions(Transition.SittingGroundDown, Transition.SittingGround);
        }

        protected override void LoopTypeActionCompleteFirstCircle(int action)
        {
            base.LoopTypeActionCompleteFirstCircle(action);

            var transition = (Transition)action;
            var roleId = GetRoleAnimation().actor.o.GetId();

            var notificationData = new LoopTypeSoloCompleteND();
            notificationData.roleId = roleId;
            notificationData.transition = transition;
            EventSystem.GetInstance().Notify(EventID.LoopTypeSoloComplete, notificationData);
        }
    }
}
