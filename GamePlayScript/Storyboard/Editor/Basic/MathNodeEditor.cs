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
    [CustomNodeEditor(typeof(MathNode))]
    public class MathNodeEditor : BasicNodeEditorBase<MathNode>
    {

    }
}
#endif
