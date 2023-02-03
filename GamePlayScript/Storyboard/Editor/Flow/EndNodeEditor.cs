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
    [CustomNodeEditor(typeof(EndNode))]
    public class EndNodeEditor : StoryNodeBaseEditor<EndNode>
    {
        public override Texture IndividualIcon()
        {
            return EditorTextures.storyboard_end;
        }

        public override Color IndividualIconColor()
        {
            return new Color(0, 0, 0, 0.3f);
        }

        public override void OnValidate()
        {
            base.OnValidate();

            Validate(false, 
                IsPortConnected(StoryNodeBase.PREV_PORT_NAME), WARNING_TEXT_LINK_PREV_PORT
            );
        }
    }
}
#endif
