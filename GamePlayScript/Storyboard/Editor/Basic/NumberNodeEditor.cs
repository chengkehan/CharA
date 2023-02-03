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
    [CustomNodeEditor(typeof(NumberNode))]
    public class NumberNodeEditor : BasicNodeEditorBase<NumberNode>
    {

    }
}
#endif
