using System;
using System.Collections.Generic;
using UnityEngine;
using UnsafeCollections.Collections.Unsafe;

namespace RideTools
{
    public class NoiseTexturer : MonoBehaviour
    {
        [SerializeField] int _width;
        [SerializeField] int _height;
        [SerializeField] NoiseData _noiseData;
        public Texture2D TexturePreview { get; private set; }
        FastNoise _noise;

        public void SetDimensions(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public unsafe UnsafeArray* Generate(int width, int height)
        {
            _width = width;
            _height = height;

            return Generate();
        }

        public unsafe void GenerateTexture()
        {
            _noise = new FastNoise(_noiseData);
            var colorBuffer = _noise.GetNoiseTex2D(_width, _height);
            var tex2D = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
            tex2D.filterMode = FilterMode.Point;
            
            var length = UnsafeArray.GetLength(colorBuffer);
            var buffer = new Color32[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = UnsafeArray.Get<Color32>(colorBuffer, i);
            }

            tex2D.SetPixels32(buffer);
            tex2D.Apply();
            tex2D.hideFlags = HideFlags.None;

            TexturePreview = tex2D;
            UnsafeArray.Free(colorBuffer);
        }

        public unsafe UnsafeArray* Generate()
        {
            _noise = new FastNoise(_noiseData);
            var colorBuffer = _noise.GetNoiseTex2D(_width, _height);
            
            var length = UnsafeArray.GetLength(colorBuffer);
            var buffer = new Color32[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = UnsafeArray.Get<Color32>(colorBuffer, i);
            }

            return colorBuffer;
        }

        public void FlushTexture()
        {
            GameObject.DestroyImmediate(TexturePreview);
        }
    }
}