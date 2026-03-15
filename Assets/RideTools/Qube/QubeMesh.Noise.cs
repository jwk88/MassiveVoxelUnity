using UnityEngine;
using UnsafeCollections.Collections.Unsafe;

namespace RideTools.Qube
{
    public static partial class QubeMesh
    {
        static unsafe UnsafeArray* _colorBuffer;
        static unsafe int _width;

        public static unsafe void SetColorBuffer(UnsafeArray* colorBuffer, int width)
        {
            _colorBuffer = colorBuffer;
            _width = width;
        }

        public static unsafe void DisposeColorBuffer()
        {
            UnsafeArray.Free(_colorBuffer);
        }

        public static unsafe float GetNoiseValue(int x, int y)
        {
            var n = y * _width + x;
            if (n < 0 || n >= UnsafeArray.GetLength(_colorBuffer))
            {
                return 0;
            }

            Color32 color = UnsafeArray.Get<Color32>(_colorBuffer, n);
            return RedToFloat(color);
        }

        static float RedToFloat(Color32 color) => color.r * (1f / 255f);

        public static unsafe void FilterByNoise(QubePoint* points, QubeData* data, int width, int height, int length, Vector2Int heightRange, int scale)
        {
            var yCutOff = 0;

            var yMax = height - 1;
            var totalSize = width * height * length;

            for (int n = 0; n < totalSize; n++)
            {
                var x = n % width;
                var y = (n / width) % height;
                var z = n / (width * height);

                if (y <= yCutOff) continue;

                var xPos = points->WorldX[n];
                var yPos = points->WorldY[n];
                var zPos = points->WorldZ[n];

                if (y < heightRange.x)
                {
                    var noiseValue = GetNoiseValue(xPos, zPos);
                    var surface = heightRange.x - (noiseValue * scale);
                    if (yPos > surface)
                    {
                        data->Visible[n] = 1;
                    }
                    else
                    {
                        data->Visible[n] = 0;
                    }
                }

                var heightOffset = yMax - heightRange.y;
                if (y > heightOffset)
                {
                    var noiseValue = GetNoiseValue(xPos, zPos);
                    var surface = heightOffset + (noiseValue * scale);
                    if (yPos < surface)
                    {
                        data->Visible[n] = 1;
                    }
                    else
                    {
                        data->Visible[n] = 0;
                    }
                }
            }
        }
    }
}