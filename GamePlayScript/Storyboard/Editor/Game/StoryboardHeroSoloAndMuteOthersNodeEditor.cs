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
    [CustomNodeEditor(typeof(StoryboardHeroSoloAndMuteOthersNode))]
    public class StoryboardHeroSoloAndMuteOthersNodeEditor : GameNodeEditorBase<StoryboardHeroSoloAndMuteOthersNode>
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            DrawRuntimeOnlyTips();
        }

        protected override string HeaderText()
        {
            return "HeroSoloAndMuteOthers";
        }
    }
}
#endif
