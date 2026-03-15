using System;
using UnityEngine.UIElements;

namespace RideTools.Qube
{
    [Serializable]
    public class QubeConfig
    {
        [ReadOnlyRuntime] public int Width = 16;
        [ReadOnlyRuntime] public int Length = 16;
        [ReadOnlyRuntime] public int ChunkWidth = 16;
        [ReadOnlyRuntime] public int ChunkHeight = 64;
        [ReadOnlyRuntime] public int ChunkLength = 16;
        [ReadOnlyRuntime] public bool UseNoiseTexture = false;
        
        [ReadOnly] public int Step = 2;

        public int HeightNoiseMin = 0;
        public int HeightNoiseMax = 63;
        public int NoiseScale = 150;

        public int ChunkCount => Width * Length;
        public int ChunkSize => ChunkWidth * ChunkHeight * ChunkLength;
        public int FullWidth => (Width * ChunkWidth) * Step;
        public int FullLength => (Length * ChunkLength) * Step;
    }
}