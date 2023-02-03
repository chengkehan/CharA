using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class ClickHeroOperation : BaseOperation
    {
        public override bool ExecuteOperation()
        {
            UIManager.GetInstance().OpenUI(UIManager.UIName.CentraPlan, null);
            return true;
        }

        public override void OnSecondOperationTheEnd()
        {
            // Do nothing
        }

        public override void OnSecondOperationUpdate()
        {
            // Do nothing
        }
    }
}
