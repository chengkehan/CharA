using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoryboardCore;
using System;

namespace GameScript.Cutscene
{
    public class SoliloquyThread
    {
        private Storyboard storyboard = null;

        private StoryThread storyThread = null;

        public SoliloquyThread(string storyboardName)
        {
            AssetsManager.GetInstance().LoadAsset<Storyboard>(AssetsManager.STORYBOARD_ASSET_PREFIX + storyboardName, (obj)=>
            {
                storyboard = obj;
                storyThread = new StoryThread(obj, talkAsyncCallback, choiceAsyncCallback, endAsyncCallback);
            });
        }

        private void talkAsyncCallback(StoryThread.AsyncHandler<TalkNode> asyncHandler)
        {
            var talkNode = asyncHandler.nodes[0];
            Utils.Log(talkNode.GetLangauge(talkNode.wordsId));
            asyncHandler.Complete();
        }

        private void choiceAsyncCallback(StoryThread.AsyncHandler<ChoiceNode> asyncHandler)
        {
            Utils.Assert(false, "Unexpected choise node in SoliloquyThread");
        }

        private void endAsyncCallback(StoryThread.AsyncHandler<EndNode> asyncHandler)
        {
            asyncHandler.Complete();

            if (storyboard != null)
            {
                AssetsManager.GetInstance().UnloadAsset(storyboard);
                storyboard = null;
            }
        }
    }
}
