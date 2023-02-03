#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using GameScript;

namespace GameScriptEditor
{
    public class TextureGammaToLinear
    {
        [MenuItem("Assets/Texture Gamma To Linear/Convert Selected Texture (RGB)")]
        private static void ConvertTextureGammaToLinearRGB()
        {
            Object[] objects = Selection.objects;
            foreach (var obj in objects)
            {
                ConvertOneTexture(obj as Texture2D, true, false, 2.2f);
            }

            Utils.Log("Convert Texture Gamma To Linear RGB Complete.");
        }

        [MenuItem("Assets/Texture Gamma To Linear/Convert Selected Texture (RGB)", true)]
        private static bool ConvertTextureGammaToLinearRGBValidation()
        {
            return ConvertTextureGammaToLinearValidation();
        }

        [MenuItem("Assets/Texture Gamma To Linear/Convert Selected Texture (Alpha)")]
        private static void ConvertTextureGammeToLinearAlpha()
        {
            Object[] objects = Selection.objects;
            foreach (var obj in objects)
            { 
                ConvertOneTexture(obj as Texture2D, false, true, 0.5f);
            }

            Utils.Log("Convert Texture Gamma To Linear Alpha Complete.");
        }

        [MenuItem("Assets/Texture Gamma To Linear/Convert Selected Texture (Alpha)", true)]
        private static bool ConvertTextureGammeToLinearAlphaValidation()
        {
            return ConvertTextureGammaToLinearValidation();
        }

        private static bool ConvertTextureGammaToLinearValidation()
        {
            Object[] objects = Selection.objects;
            foreach (var obj in objects)
            {
                if (obj != null && obj is Texture2D == false)
                {
                    return false;
                }
            }
            return true;
        }

        private static void ConvertOneTexture(Texture2D texture2D, bool isRGB, bool isAlpha, float gamma)
        {
            if (texture2D == null)
            {
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(texture2D);

            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (textureImporter.isReadable == false)
            {
                textureImporter.isReadable = true;
                textureImporter.SaveAndReimport();
            }

            texture2D = AssetDatabase.LoadMainAssetAtPath(assetPath) as Texture2D;

            Color[] pixels = texture2D.GetPixels();
            int numPixels = pixels.Length;
            for (int pixelI = 0; pixelI < numPixels; pixelI++)
            {
                Color c = pixels[pixelI];
                if (isRGB)
                {
                    c.r = Mathf.Pow(c.r, gamma);
                    c.g = Mathf.Pow(c.g, gamma);
                    c.b = Mathf.Pow(c.b, gamma);
                }
                if (isAlpha)
                {
                    c.a = Mathf.Pow(c.a, gamma);
                }
                pixels[pixelI] = c; 
            }

            if (textureImporter.isReadable)
            {
                textureImporter.isReadable = false;
                textureImporter.SaveAndReimport();
            }

            Texture2D newTexture2D = new Texture2D(texture2D.width, texture2D.height);
            newTexture2D.SetPixels(pixels);
            byte[] pngBytes = newTexture2D.EncodeToPNG();
            Texture2D.DestroyImmediate(newTexture2D);

            FileInfo assetFileInfo = new FileInfo(assetPath);
            string newAssetPath = assetPath.Substring(0, assetPath.Length - assetFileInfo.Extension.Length) + "_GTL.png";
            File.WriteAllBytes(newAssetPath, pngBytes);
            AssetDatabase.ImportAsset(newAssetPath);

            TextureImporter newTextureImporter = AssetImporter.GetAtPath(newAssetPath) as TextureImporter;
            newTextureImporter.textureType = textureImporter.textureType;
            newTextureImporter.SaveAndReimport();
        }
    }
}
#endif
