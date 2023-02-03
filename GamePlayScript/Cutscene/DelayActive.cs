using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript.Cutscene
{
    public class DelayActive : MonoBehaviour
    {
        public float delaySecond = 1;

        public GameObject[] targets = null;

        private float time = 0;

        private void Awake()
        {
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.SetActive(false);
                }
            }
        }

        private void Start()
        {
            time = 0;    
        }

        private void FixedUpdate()
        {
            time += Time.fixedDeltaTime;
            if (time >= delaySecond)
            {
                if (targets != null)
                {
                    foreach (var target in targets)
                    {
                        if (target != null)
                        {
                            target.SetActive(true);
                        }
                    }
                }
                enabled = false;
            }
        }
    }
}
