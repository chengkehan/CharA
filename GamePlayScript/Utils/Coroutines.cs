using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class Coroutines : MonoBehaviour
    {
        private static Coroutines s_instance = null;

        public static Coroutines GetInstance()
        {
            return s_instance;
        }

        public void Execute(IEnumerator enumerator)
        {
            base.StartCoroutine(enumerator);
        }

        public new Coroutine StartCoroutine(IEnumerator routine)
        {
            throw new System.NotSupportedException();
        }

        private void Awake()
        {
            s_instance = this;
        }

        private void OnDestroy()
        {
            s_instance = null;
        }
    }
}
