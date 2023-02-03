using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameScript
{
    [Serializable]
    public class GlobalEffectVolume
    {
        public bool overwrite = false;

        public float skyboxFogBlendStart = 0;
        public float skyboxFogBlendEnd = 500;
        [Range(0.1f, 10)]
        public float skyboxFogPow = 0.5f;

        public float heightFogStart = 10;
        public float heightFogEnd = 40;
        public float heightFogTop = 2;
        public float heightFogBottom = -2;
        public float heightFogSpeed = 0.3f;
        public float heightFogScale = 0.25f;
        [Min(0)]
        public float heightFogIntensity = 2;
        [Min(0)]
        public float heightFogNoiseBlendSpeed = 0.2f;
        public Texture2D heightFogNoise = null;
        [Min(0)]
        public float heightFogMinAtNight = 0.1f;
        public bool heightFogCoord = false;

        [Min(1)]
        public int blurDownsample = 2;
        [Min(1)]
        public int blurPasses = 6;
        [Min(1)]
        public float blurOffset = 3;

        [Min(0.1f)]
        public float radius = 0.6f;
        [Min(1)]
        public int radiusIteration = 10;
    }
}
