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
    [CustomNodeEditor(typeof(TriggerNode))]
    public class TriggerNodeEditor : StoryNodeBaseEditor<TriggerNode>
    {
        public override Color GetTint()
        {
            return GetTintByState(new Color(0f, 0.4705882f, 0.3279017f, 1));
        }

        public override Texture IndividualIcon()
        {
            return EditorTextures.storyboard_trigger;
        }

        public override Color IndividualIconColor()
        {
            return new Color(0, 0, 0, 0.3f);
        }

        public override void OnValidate()
        {
            base.OnValidate();

            Validate(false,
                IsPortConnected(StoryNodeBase.NEXT_PORT_NAME), WARNING_TEXT_LINK_NEXT_PORT,
                IsPortConnected(StoryNodeBase.PREV_PORT_NAME), WARNING_TEXT_LINK_PREV_PORT
            );
        }
    }
}
#endif
