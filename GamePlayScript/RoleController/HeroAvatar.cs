using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class HeroAvatar : MonoBehaviour
    {
        public GameObject body = null;

        public GameObject topNormal = null;
        public Texture2D topNormalBodyMask = null;
        public GameObject topPolice = null;
        public Texture2D topPoliceBodyMask = null;
        public GameObject topDoctor = null;
        public Texture2D topDoctorBodyMask = null;
        public GameObject topDoughBoy = null;
        public Texture2D topDoughBoyBodyMask = null;
        public GameObject topHoody = null;
        public Texture2D topHoodyBodyMask = null;

        public GameObject bottomNormal = null;
        public Texture2D bottomNormalBodyMask = null;
        public GameObject bottomPolice = null;
        public Texture2D bottomPoliceBodyMask = null;
        public Texture2D bottomPoliceSelfMask = null;
        public GameObject bottomDoctor = null;
        public Texture2D bottomDoctorBodyMask = null;
        public GameObject bottomDoughBoy = null;
        public Texture2D bottomDoughBoyBodyMask = null;
        public GameObject bottomHoody = null;
        public Texture2D bottomHoodyBodyMask = null;

        public GameObject shoesNormal = null;
        public Texture2D shoesNormalBodyMask = null;
        public GameObject shoesPolice = null;
        public Texture2D shoesPoliceBodyMask = null;
        public GameObject shoesDoctor = null;
        public Texture2D shoesDoctorBodyMask = null;
        public GameObject shoesDoughBoy = null;
        public Texture2D shoesDoughBoyBodyMask = null;
        public Texture2D shoesDoughBoySelfMask = null;
        public GameObject shoesHoody = null;
        public Texture2D shoesHoodyBodyMask = null;

        public GameObject[] hat1 = null;
        public GameObject[] hat2 = null;
        public GameObject[] hat3 = null;
        private GameObject[] currentHat = null;

        public GameObject hair1 = null;
        public GameObject hair2 = null;
        public GameObject hair3 = null;
        public GameObject hair4 = null;
        private int currentHairIndex = 0;

        public GameObject eyewear1 = null;

        public GameObject beard1 = null;
        public GameObject beard2 = null;

        private Texture2D bodyClipMask = null;

        private Texture2D currentTopClipMask = null;
        private Texture2D currentBottomClipMask = null;
        private Texture2D currentShoesClipMask = null;

        private Mesh bodyMesh = null;
        private Mesh bodyMeshCloned = null;

        public const int CLIP_MASK_SIZE = 128;

        private void Start()
        {
            currentTopClipMask = topNormalBodyMask;
            currentBottomClipMask = bottomNormalBodyMask;
            currentShoesClipMask = shoesNormalBodyMask;
            UpdateBodyClipMask();
            SetHair(hair1);
        }

        private void UpdateBodyClipMask()
        {
            if (bodyClipMask == null)
            {
                bodyClipMask = new Texture2D(CLIP_MASK_SIZE, CLIP_MASK_SIZE, TextureFormat.RGB24, false);
            }

            Color[] colors = bodyClipMask.GetPixels();
            int numPixels = colors.Length;
            for (int i = 0; i < numPixels; i++)
            {
                colors[i] = Color.white;
            }

            SetBodyClipMaskPixels(colors, currentTopClipMask);
            SetBodyClipMaskPixels(colors, currentBottomClipMask);
            SetBodyClipMaskPixels(colors, currentShoesClipMask);

            bodyClipMask.SetPixels(colors);
            bodyClipMask.Apply();

            body.GetComponent<SkinnedMeshRenderer>().material.SetTexture("_AlphaTestMaskTex", bodyClipMask);
        }

        private void SetBodyClipMaskPixels(Color[] colors, Texture2D clipMask)
        {
            if (clipMask != null && clipMask.isReadable && clipMask.width == CLIP_MASK_SIZE && clipMask.height == CLIP_MASK_SIZE)
            {
                int numPixels = colors.Length;
                Color[] clipMaskColors = clipMask.GetPixels();
                for (int i = 0; i < numPixels; i++)
                {
                    colors[i] *= clipMaskColors[i];
                }
            }
        }
        /*
        private void OnGUI()
        {
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(0.65f, 0.65f, 0.65f)); ;

            if (GUILayout.Button("红色上衣"))
            {
                SetTop(topNormal, topNormalBodyMask);
            }
            if (GUILayout.Button("警服"))
            {
                SetTop(topPolice, topPoliceBodyMask);
            }
            if (GUILayout.Button("手术服"))
            {
                SetTop(topDoctor, topDoctorBodyMask);
            }
            if (GUILayout.Button("大兵服"))
            {
                SetTop(topDoughBoy, topDoughBoyBodyMask);
            }
            if (GUILayout.Button("小混混服"))
            {
                SetTop(topHoody, topHoodyBodyMask);
            }
            GUILayout.Label("---------------------");
            if (GUILayout.Button("休闲裤"))
            {
                SetBottom(bottomNormal, bottomNormalBodyMask);
            }
            if (GUILayout.Button("警裤"))
            {
                SetBottom(bottomPolice, bottomPoliceBodyMask);
            }
            if (GUILayout.Button("手术裤"))
            {
                SetBottom(bottomDoctor, bottomDoctorBodyMask);
            }
            if (GUILayout.Button("大兵裤"))
            {
                SetBottom(bottomDoughBoy, bottomDoughBoyBodyMask);
            }
            if (GUILayout.Button("小混混裤"))
            {
                SetBottom(bottomHoody, bottomHoodyBodyMask);
            }
            GUILayout.Label("---------------------");
            if (GUILayout.Button("休闲鞋"))
            {
                SetShoes(shoesNormal, shoesNormalBodyMask);
            }
            if (GUILayout.Button("皮鞋"))
            {
                SetShoes(shoesPolice, shoesPoliceBodyMask);
            }
            if (GUILayout.Button("休闲鞋2"))
            {
                SetShoes(shoesDoctor, shoesDoctorBodyMask);
            }
            if (GUILayout.Button("大兵鞋"))
            {
                SetShoes(shoesDoughBoy, shoesDoughBoyBodyMask);
            }
            if (GUILayout.Button("小混混鞋"))
            {
                SetShoes(shoesHoody, shoesHoodyBodyMask);
            }
            GUILayout.Label("---------------------");
            if (GUILayout.Button("光头"))
            {
                SetHair(null);
                RefreshHat();
            }
            if (GUILayout.Button("头发1"))
            {
                SetHair(hair1);
                RefreshHat();
            }
            if (GUILayout.Button("头发2"))
            {
                SetHair(hair2);
                RefreshHat();
            }
            if (GUILayout.Button("头发3"))
            {
                SetHair(hair3);
                RefreshHat();
            }
            if (GUILayout.Button("头发4"))
            {
                SetHair(hair4);
                RefreshHat();
            }
            GUILayout.Label("---------------------");
            if (GUILayout.Button("无帽子"))
            {
                SetHat(null);
            }
            if (GUILayout.Button("警帽"))
            {
                SetHat(hat1);
            }
            if (GUILayout.Button("手术帽子"))
            {
                SetHat(hat2);
            }
            if (GUILayout.Button("大兵帽"))
            {
                SetHat(hat3);
            }
            GUILayout.Label("---------------------");
            if (GUILayout.Button("无眼镜"))
            {
                eyewear1.gameObject.SetActive(false);
            }
            if (GUILayout.Button("蛤蟆镜"))
            {
                eyewear1.gameObject.SetActive(true);
            }
            GUILayout.Label("---------------------");
            if (GUILayout.Button("无胡须"))
            {
                SetBeard(null);
            }
            if (GUILayout.Button("胡须1"))
            {
                SetBeard(beard1);
            }
            if (GUILayout.Button("口罩"))
            {
                SetBeard(beard2);
            }

            GUI.matrix = Matrix4x4.identity;
        }
        */

        private void UpdateBottomPolice()
        {
            UpdateSelfMask(bottomPolice, topPolice, bottomPoliceSelfMask);
        }

        private void UpdateShoesDoughBoy()
        {
            UpdateSelfMask(shoesDoughBoy, bottomDoughBoy, shoesDoughBoySelfMask);
        }

        private void UpdateSelfMask(GameObject a, GameObject b, Texture2D selfMask)
        {
            if (a.activeSelf)
            {
                Material mtrl = a.GetComponent<SkinnedMeshRenderer>().material;
                if (b.activeSelf)
                {
                    mtrl.EnableKeyword("_ALPHA_TEST_OFF");
                    mtrl.DisableKeyword("_ALPHA_TEST_BASE");
                    mtrl.DisableKeyword("_ALPHA_TEST_TEX");
                }
                else
                {
                    mtrl.DisableKeyword("_ALPHA_TEST_OFF");
                    mtrl.DisableKeyword("_ALPHA_TEST_BASE");
                    mtrl.EnableKeyword("_ALPHA_TEST_TEX");
                    mtrl.SetTexture("_AlphaTestMaskTex", selfMask);
                }
            }
        }

        private void SetShoes(GameObject shoes, Texture2D bodyMask)
        {
            shoesNormal.gameObject.SetActive(shoesNormal == shoes);
            shoesPolice.gameObject.SetActive(shoesPolice == shoes);
            shoesDoctor.gameObject.SetActive(shoesDoctor == shoes);
            shoesDoughBoy.gameObject.SetActive(shoesDoughBoy == shoes);
            shoesHoody.gameObject.SetActive(shoesHoody == shoes);
            currentShoesClipMask = bodyMask;
            UpdateBodyClipMask();
            UpdateShoesDoughBoy();
        }

        private void SetTop(GameObject top, Texture2D bodyMask)
        {
            topNormal.gameObject.SetActive(topNormal == top);
            topPolice.gameObject.SetActive(topPolice == top);
            topDoctor.gameObject.SetActive(topDoctor == top);
            topDoughBoy.gameObject.SetActive(topDoughBoy == top);
            topHoody.gameObject.SetActive(topHoody == top);
            currentTopClipMask = bodyMask;
            UpdateBodyClipMask();
            UpdateBottomPolice();
        }

        private void SetBottom(GameObject bottom, Texture2D bodyMask)
        {
            bottomNormal.gameObject.SetActive(bottomNormal == bottom);
            bottomPolice.gameObject.SetActive(bottomPolice == bottom);
            bottomDoctor.gameObject.SetActive(bottomDoctor == bottom);
            bottomDoughBoy.gameObject.SetActive(bottomDoughBoy == bottom);
            bottomHoody.gameObject.SetActive(bottomHoody == bottom);
            currentBottomClipMask = bodyMask;
            UpdateBodyClipMask();
            UpdateBottomPolice();
            UpdateShoesDoughBoy();
        }

        private void SetBeard(GameObject beard)
        {
            beard1.SetActive(beard1 == beard);
            beard2.SetActive(beard2 == beard);
        }

        private void RefreshHat()
        {
            SetHat(currentHat);
        }
        private void SetHat(GameObject[] hat)
        {
            currentHat = hat;
            SetHat_Internal(hat1, hat1 == hat);
            SetHat_Internal(hat2, hat2 == hat);
            SetHat_Internal(hat3, hat3 == hat);
        }
        private void SetHat_Internal(GameObject[] hat, bool isVisible)
        {
            if (hat != null)
            {
                for (int i = 0; i < hat.Length; i++)
                {
                    hat[i].SetActive(i == currentHairIndex && isVisible);
                }
            }
        }

        private void SetHair(GameObject hair)
        {
            hair1.SetActive(hair1 == hair);
            hair2.SetActive(hair2 == hair);
            hair3.SetActive(hair3 == hair);
            hair4.SetActive(hair4 == hair);
            currentHairIndex = hair == hair1 ? 1 :
                                hair == hair2 ? 2 :
                                hair == hair3 ? 3 :
                                hair == hair4 ? 4 : 0;
        }

        private void OnDestroy()
        {
            if (bodyClipMask != null)
            {
                DestroyImmediate(bodyClipMask, true);
                bodyClipMask = null;
            }
        }
    }
}
