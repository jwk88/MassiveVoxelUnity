using UnityEngine;

namespace RideTools.Qube
{
    public class QubeRuntime
    {
        public Vector2Int HeightRange;
        public int NoiseScale;

        public QubeRuntime(QubeConfig config)
        {
            HeightRange = new Vector2Int(config.HeightNoiseMin, config.HeightNoiseMax);
            NoiseScale = config.NoiseScale;
        }
    }
}