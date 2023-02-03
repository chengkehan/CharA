#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace GameScriptEditor
{
    public class UpgradeLegacyCubemap : ScriptableWizard
    {
        public Cubemap legacyCubemap = null;

        public Cubemap cubemap = null;

        void OnWizardUpdate()
        {
            if (legacyCubemap != null && cubemap != null)
            {
                if (legacyCubemap.isReadable == false)
                {
                    helpString = "Legacy cubemap should be marked as isReadable.";
                    isValid = false;
                }
                else
                {
                    helpString = string.Empty;
                    isValid = true;
                }
            }
            else
            {
                helpString = "Please set legacy cubemap asset and cubemap asset.";
                isValid = false;
            }
        }

        [MenuItem("GameTools/Cubemap Lighting/Upgrade Legacy Cubemap")]
        private static void UpgradeLegacyCubemapMenu()
        {
            ScriptableWizard.DisplayWizard("Upgrade Legacy Cubemap", typeof(UpgradeLegacyCubemap), "Upgrade");
        }

        void OnWizardCreate()
        {
            string cubemapAssetPath = AssetDatabase.GetAssetPath(cubemap);
            int cubemapSize = Mathf.Max(legacyCubemap.width, legacyCubemap.height);
            Texture2D nx = CreateCubemapFace(legacyCubemap, CubemapFace.NegativeX, cubemapSize);
            Texture2D ny = CreateCubemapFace(legacyCubemap, CubemapFace.NegativeY, cubemapSize);
            Texture2D nz = CreateCubemapFace(legacyCubemap, CubemapFace.NegativeZ, cubemapSize);
            Texture2D px = CreateCubemapFace(legacyCubemap, CubemapFace.PositiveX, cubemapSize);
            Texture2D py = CreateCubemapFace(legacyCubemap, CubemapFace.PositiveY, cubemapSize);
            Texture2D pz = CreateCubemapFace(legacyCubemap, CubemapFace.PositiveZ, cubemapSize);

            Texture2D atlas = new Texture2D(cubemapSize * 6, cubemapSize, TextureFormat.RGBA32, true, true);
            atlas.hideFlags = HideFlags.HideAndDontSave;
            PackTexture(atlas, px, 0, cubemapSize);
            PackTexture(atlas, nx, cubemapSize, cubemapSize);
            PackTexture(atlas, py, cubemapSize * 2, cubemapSize);
            PackTexture(atlas, ny, cubemapSize * 3, cubemapSize);
            PackTexture(atlas, pz, cubemapSize * 4, cubemapSize);
            PackTexture(atlas, nz, cubemapSize * 5, cubemapSize);

            byte[] atlasBytes = atlas.EncodeToPNG();
            File.WriteAllBytes(cubemapAssetPath, atlasBytes);
            AssetDatabase.ImportAsset(cubemapAssetPath);

            TextureImporter atlasImporter = AssetImporter.GetAtPath(cubemapAssetPath) as TextureImporter;
            TextureImporterSettings atlasSettings = new TextureImporterSettings();
            atlasImporter.ReadTextureSettings(atlasSettings);
            atlasSettings.textureShape = TextureImporterShape.TextureCube;
            atlasImporter.SetTextureSettings(atlasSettings);
            AssetDatabase.ImportAsset(cubemapAssetPath);

            DestroyImmediate(nx, true);
            DestroyImmediate(ny, true);
            DestroyImmediate(nz, true);
            DestroyImmediate(px, true);
            DestroyImmediate(py, true);
            DestroyImmediate(pz, true);
            DestroyImmediate(atlas, true); 
        }

        private void PackTexture(Texture2D atlas, Texture2D tex, int offset, int spriteSize)
        {
            atlas.SetPixels(offset, 0, spriteSize, spriteSize, tex.GetPixels());
            atlas.Apply();
        }

        private Texture2D CreateCubemapFace(Cubemap cubemap, CubemapFace face, int cubemapSize)
        {
            Color[] nxc = cubemap.GetPixels(face);
            if (PlayerSettings.colorSpace == ColorSpace.Gamma)
            {
                EncodeGamme(nxc);
            }
            FlipPixels(nxc, cubemapSize);
            Texture2D nxct = new Texture2D(cubemapSize, cubemapSize, TextureFormat.RGBAFloat, true, true);
            nxct.hideFlags = HideFlags.HideAndDontSave;
            nxct.SetPixels(nxc);
            nxct.Apply(true);

            return nxct;
        }

        private Color[] EncodeGamme(Color[] colors)
        {
            float gamma = 2.2f;
            int count = colors.Length;
            for (int i = 0; i < count; ++i)
            {
                Color c = colors[i];
                c.r = Mathf.Pow(c.r, gamma);
                c.g = Mathf.Pow(c.g, gamma);
                c.b = Mathf.Pow(c.b, gamma);
                colors[i] = c;
            }
            return colors;
        }

        private void FlipPixels(Color[] colors, int size)
        {
            Color[] buffer = new Color[size];
            for (int i = 0; i < size / 2; ++i)
            {
                CopyColors(colors, i * size, buffer, 0, size);
                //MirrorColors(buffer, 0, size);
                CopyColors(colors, (size - 1 - i) * size, colors, i * size, size);
                //MirrorColors(colors, i * size, size);
                CopyColors(buffer, 0, colors, (size - 1 - i) * size, size);
            }
        }

        private void CopyColors(Color[] src, int srcIndex, Color[] dest, int destIndex, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                dest[destIndex + i] = src[srcIndex + i];
            }
        }
    }
}
#endif