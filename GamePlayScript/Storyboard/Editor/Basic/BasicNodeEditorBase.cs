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
    public class BasicNodeEditorBase<T> : StoryNodeBaseEditor<T>
        where T : StoryNodeBase
    {
        public override Color GetTint()
        {
            return new Color(0.2f, 0.2f, 0.2f, 1);
        }
    }
}
#endif
