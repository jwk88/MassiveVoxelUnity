namespace RideTools.Qube
{
    public static partial class QubeMesh
    {
        public static unsafe void SetWorldPositions(int offset, int fullWidth, QubePoint* points, QubeData* data, int width, int height, int length, int step)
        {
            int xi = offset % fullWidth;
            int yi = offset / fullWidth % 1;
            int zi = offset / (fullWidth * 1);

            var yCutOff = 0;
            var totalSize = width * height * length;

            for (int n = 0; n < totalSize; n++)
            {
                var x = n % width;
                var y = (n / width) % height;
                var z = n / (width * height);

                var xPos = step * x;
                var yPos = step * y; 
                var zPos = step * z;

                xPos += xi * step * width;
                yPos += yi * step * height;
                zPos += zi * step * length;

                if (xPos >= 35000 || yPos >= 35000 || zPos >= 35000) UnityEngine.Debug.LogWarning("Approaching short limit");

                points->WorldX[n] = (short)xPos;
                points->WorldY[n] = (short)yPos;
                points->WorldZ[n] = (short)zPos;
                data->Visible[n] = y > yCutOff ? (byte)1 : (byte)0;
                
                if (y > yCutOff)
                {
                    data->Faces[n] |= (byte)(QubeFace.Top | QubeFace.Bot | QubeFace.Front | QubeFace.Back | QubeFace.Right | QubeFace.Left);
                }
                else
                {
                    data->Faces[n] = 0;
                }
            }
        }
    }    
}