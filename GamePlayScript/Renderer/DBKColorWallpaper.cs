using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    [ExecuteInEditMode]
    public class DBKColorWallpaper : MonoBehaviour
    {
        [Range(0, 64)]
        public int color = 1;

        [Range(1, 16)]
        public int wallpaperNumberRow = 1;

        public bool setValueManually = false;

        public int wallpaperNumber = 0;

        public int wallpaperRow = 0;

        private void Awake()
        {
            UpdateValues();
        }

        private void OnValidate()
        {
            UpdateValues();
        }

        private void OnDestroy()
        {
            if (Application.isPlaying == false)
            {
                MeshRenderer mr = GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.realtimeLightmapIndex = -1;
                }
            }
        }

        private void UpdateValues()
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr != null)
            {
                int wallpaperNumber = setValueManually ? this.wallpaperNumber : ReturnOrnamentRow(wallpaperNumberRow);
                int wallpaperRow = setValueManually ? this.wallpaperRow : ReturnOrnamentColumn(wallpaperNumberRow);

                mr.realtimeLightmapIndex = 0;
                mr.realtimeLightmapScaleOffset = new Vector4(color, wallpaperNumber, wallpaperRow, 0);
            }
        }

        private int ReturnOrnamentRow(int index)
        {
            int row = Mathf.CeilToInt(index / 4) + 1;
            return row;
        }

        private int ReturnOrnamentColumn(int index)
        {
            int column = index % 4;
            return column;
        }
    }
}
