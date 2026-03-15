using System;
using UnityEngine;

namespace RideTools
{
    [Serializable]
    public class NoiseData
    {
        [Range(-100, 100)]
        public int Seed = -58;
        [Range(0.001f, 0.1f)]
        public float Frequency = 0.017f;
        [Range(0.01f, 1f)]
        public float Gain = 0.358f;
        [Range(0.01f, 1f)]
        public float Lacunarity = 0.5f;
        [Range(1, 50)]
        public int Octaves = 4;
        
        public FastNoiseLite.NoiseType Type = FastNoiseLite.NoiseType.Cellular;
        public FastNoiseLite.FractalType FractalType = FastNoiseLite.FractalType.Ridged;
    }
}