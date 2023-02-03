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
    public class GameNodeEditorBase<T> : StoryNodeBaseEditor<T>
        where T : StoryNodeBase
    {
        public override Color GetTint()
        {
            return GetTintByState(new Color(0.2f, 0.2f, 0.2f, 1));
        }

        protected void DrawRuntimeOnlyTips(string appendTooltip = null)
        {
            var tooltip =
                "This node is working only at runtime, output is default value when game is not playing.\n" +
                "   default text is empty\n   default number is 0\n   default boolean is false";
            if (string.IsNullOrWhiteSpace(appendTooltip) == false)
            {
                tooltip += "\n" + appendTooltip;
            }
            DrawTipsLabel("Runtime only", tooltip);
        }
    }
}
#endif
