#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using StoryboardCore;

namespace StoryboardEditor
{
    [CustomNodeEditor(typeof(StartNode))]
    public class StartNodeEditor : StoryNodeBaseEditor<StartNode>
    {
        public override Color GetTint()
        {
            return GetTintByState(new Color(0.03737985f, 0.3773585f, 0.04202573f, 1));
        }

        public override Texture IndividualIcon()
        {
            return EditorTextures.storyboard_start;
        }

        public override Color IndividualIconColor()
        {
            return new Color(0, 0, 0, 0.3f);
        }

        public override void OnValidate()
        {
            base.OnValidate();

            Validate(false,
                IsPortConnected(StoryNodeBase.NEXT_PORT_NAME), WARNING_TEXT_LINK_NEXT_PORT
            );
        }
    }
}
#endif
