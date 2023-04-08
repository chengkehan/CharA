using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoryboardCore;

namespace GameScript.UI.Talking
{
    public class TalkingController : MonoBehaviour
    {
        private Talking talkingUI = null;

        private Storyboard storyboard = null;

        private StoryThread storyThread = null;

        private StoryThread.AsyncHandler<TalkNode> storyThreadAsyncHandler_talk = null;

        private StoryThread.AsyncHandler<ChoiceNode> storyThreadAsyncHandler_choice = null;

        private StoryThread.AsyncHandler<EndNode> storyThreadAsyncHandler_end = null;

        // articlePid must be validated before pass to this
        public bool Initialize(Talking talkingUI, string storyboardName)
        {
            if (this.talkingUI == null)
            {
                this.talkingUI = talkingUI;
                this.talkingUI.AdjustPivotPosition();

                AssetsManager.GetInstance().LoadAsset<Storyboard>(AssetsManager.STORYBOARD_ASSET_PREFIX + storyboardName, (obj) =>
                {
                    storyboard = obj;
                    storyThread = new StoryThread(storyboard, TalkAsyncCallback, ChoiceAsyncCallback, EndAsyncCallback);
                });

                return true;
            }
            else
            {
                return false;
            }
        }

        private void TalkAsyncCallback(StoryThread.AsyncHandler<TalkNode> asyncHandler)
        {
            storyThreadAsyncHandler_talk = asyncHandler;

            var nameStr = string.Empty;
            var talkNode = storyThreadAsyncHandler_talk.nodes[0];
            var roleId = talkNode.roleId;

            if (DataCenter.query.IsHeroRoleIdSimplified(roleId))
            {
                roleId = DataCenter.define.HeroRoleID;
            }

            if (string.IsNullOrWhiteSpace(roleId) == false)
            {
                if (DataCenter.query.IsHeroRoleID(roleId))
                {
                    nameStr = talkingUI.GetLanguage("me");
                }
                else
                {
                    RoleConfig roleConfig = DataCenter.GetInstance().GetRoleConfig(roleId);
                    nameStr = roleConfig.name;
                }
            }

            talkingUI.SetAllItemsAsGray();
            talkingUI.AddWords(nameStr, talkNode.GetLangauge(talkNode.wordsId), false, roleId);
            talkingUI.AddNextStep(TalkCompleteCB);

            talkingUI.ScrollToBottom();
        }

        private void TalkCompleteCB()
        {
            talkingUI.RemoveTheLastOne();
            talkingUI.ScrollToBottom();

            storyThreadAsyncHandler_talk.Complete();
        }

        private void ChoiceAsyncCallback(StoryThread.AsyncHandler<ChoiceNode> asyncHandler)
        {
            storyThreadAsyncHandler_choice = asyncHandler;

            var choiceNodes = storyThreadAsyncHandler_choice.nodes;
            for (int choiceI = 0; choiceI < choiceNodes.Count; choiceI++)
            {
                var choiceNode = choiceNodes[choiceI];
                talkingUI.AddChoice(choiceI + 1, choiceNode.GetLangauge(choiceNode.wordsId), ChoiceCompleteCB);
            }

            talkingUI.ScrollToBottom();
        }

        private void ChoiceCompleteCB(int index)
        {
            var choiceNodes = storyThreadAsyncHandler_choice.nodes;
            for (int choiceI = 0; choiceI < choiceNodes.Count; choiceI++)
            {
                talkingUI.RemoveTheLastOne();
            }

            var choiceNode = choiceNodes[index - 1];
            talkingUI.AddWords(talkingUI.GetLanguage("me"), choiceNode.GetLangauge(choiceNode.wordsId), true, null);
            talkingUI.ScrollToBottom();

            storyThreadAsyncHandler_choice.Complete(choiceNode);
        }

        private void EndAsyncCallback(StoryThread.AsyncHandler<EndNode> asyncHandler)
        {
            storyThreadAsyncHandler_end = asyncHandler;

            talkingUI.AddButton("End", EndCompleteCB);
            talkingUI.ScrollToBottom();
        }

        private void EndCompleteCB()
        {
            storyThreadAsyncHandler_end.Complete();

            UIManager.GetInstance().CloseUI(UIManager.UIName.Talking);
        }

        private void OnDestroy()
        {
            if (storyboard != null)
            {
                AssetsManager.GetInstance().UnloadAsset(storyboard);
                storyboard = null;
            }
        }
    }
}
