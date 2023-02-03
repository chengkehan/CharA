#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using StoryboardCore;
using GameScript;

namespace StoryboardEditor
{
    [CustomNodeEditor(typeof(ChoiceNode))]
    public class ChoiceNodeEditor : StoryNodeBaseEditor<ChoiceNode>
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            DrawInfoLabel(node.GetLangauge(node.wordsId));
        }

        public override Color GetTint()
        {
            return GetTintByState(new Color(0.3584906f, 0f, 0.2339623f, 1));
        }

        public override Texture IndividualIcon()
        {
            return EditorTextures.storyboard_choice;
        }

        public override Color IndividualIconColor()
        {
            return new Color(0, 0, 0, 0.3f);
        }

        public override void OnValidate()
        {
            base.OnValidate();
            
            Validate(false, 
                !string.IsNullOrWhiteSpace(node.wordsId), "words id is required",
                node.ContainsLanguage(node.wordsId), "words not existed", 
                IsPortConnected(StoryNodeBase.NEXT_PORT_NAME), WARNING_TEXT_LINK_NEXT_PORT,
                IsPortConnected(StoryNodeBase.PREV_PORT_NAME), WARNING_TEXT_LINK_PREV_PORT
            );
        }
    }
}
#endif
