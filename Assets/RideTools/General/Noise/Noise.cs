using UnityEngine;
using UnsafeCollections.Collections.Unsafe;

namespace RideTools
{
    public class FastNoise
    {
        FastNoiseLite _fastNoiseLite;

        public FastNoise(NoiseData data)
        {
            _fastNoiseLite = new FastNoiseLite(data.Seed);
            _fastNoiseLite.SetNoiseType(data.Type);
            _fastNoiseLite.SetFractalType(data.FractalType);
            _fastNoiseLite.SetFrequency(data.Frequency);
            _fastNoiseLite.SetFractalLacunarity(data.Lacunarity);
            _fastNoiseLite.SetFractalOctaves(data.Octaves);
            _fastNoiseLite.SetFractalGain(data.Gain);
        }

        public unsafe UnsafeArray* GetNoiseTex2D(int width, int height)
        {
            var size = width * height;
            var colors = UnsafeArray.Allocate<Color32>(size);
            for (int n = 0; n < size; n++)
            {
                int x = n % width;
                int y = n / width;
                var noise = GetNoise2D(x, y);
                var sample = Remap(noise);
                Color32 color = new Color(sample, sample, sample, sample);
                UnsafeArray.Set(colors, n, color);
            }

            return colors;
        }

        float Remap(float value) => (value + 1f) * 0.5f;

        public float GetNoise3D(int x, int y, int z)
        {
            return _fastNoiseLite.GetNoise(x, y, z);
        }

        public float GetNoise2D(float x, float y)
        {
            return _fastNoiseLite.GetNoise(x, y);
        }
    }
}