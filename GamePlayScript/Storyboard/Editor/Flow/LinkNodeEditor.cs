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
    [CustomNodeEditor(typeof(LinkNode))]
    public class LinkNodeEditor : StoryNodeBaseEditor<LinkNode>
    {
        public override Color GetTint()
        {
            return GetTintByState(new Color(0.3819272f, 0.3819272f, 0f, 1));
        }

        public override Texture IndividualIcon()
        {
            if (node.Connected())
            {
                return EditorTextures.storyboard_link;
            }
            else
            {
                return EditorTextures.storyboard_break;
            }
        }

        public override Color IndividualIconColor()
        {
            if (node.Connected())
            {
                return new Color(0, 0, 0, 0.3f);
            }
            else
            {
                return new Color(1, 0.92f, 0.016f, 0.3f);
            }
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
