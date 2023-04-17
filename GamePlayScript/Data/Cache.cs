using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    public class Cache
    {
        private SoloCompleteND _soloCompleteND = new SoloCompleteND();
        public SoloCompleteND soloCompleteND
        {
            get
            {
                return _soloCompleteND;
            }
        }

        private UpBodyAnimationCompleteND _upBodyAnimationCompleteND = new UpBodyAnimationCompleteND();
        public UpBodyAnimationCompleteND upBodyAnimationCompleteND
        {
            get
            {
                return _upBodyAnimationCompleteND;
            }
        }
    }
}
