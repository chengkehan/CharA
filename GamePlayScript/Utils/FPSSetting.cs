using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class FPSSetting : MonoBehaviour
    {
        private void Start()
        {
            Application.targetFrameRate = 30;
        }
    }
}
