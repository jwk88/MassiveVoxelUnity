namespace RideTools.Qube
{
    public static partial class QubeMesh
    {
        public unsafe static void ProcessFaces(QubePoint* _points, QubeData* _data, int width, int height, int length)
        {
            var size = width * height * length;
            for (int n = 0; n < size; n++)
            {
                var x = n % width;
                var y = (n / width) % height;
                var z = n / (width * height);

                var floor = y <= 0;

                var posX = _points->WorldX[n];
                var posY = _points->WorldY[n];
                var posZ = _points->WorldZ[n];

                if (_data->Visible[n] == 0 && !floor)
                {
                    _data->Faces[n] = 0;
                    continue;
                }

                for (int dz = -1; dz <= 1; dz++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            if (((dx & dy) | (dy & dz) | (dz & dx)) != 0) continue;

                            var x2 = dx + x;
                            var y2 = dy + y;
                            var z2 = dz + z;

                            if (x2 < 0) continue;
                            if (y2 < 0) continue;
                            if (z2 < 0) continue;
                            if (x2 >= width) continue;
                            if (y2 >= height) continue;
                            if (z2 >= length) continue;

                            var ni = x2 + y2 * width + z2 * width * height;
                            if (ni < 0 || ni >= (width * height * length)) continue;
                            if (ni == n) continue;

                            var nX = _points->WorldX[ni];
                            var nY = _points->WorldY[ni];
                            var nZ = _points->WorldZ[ni];

                            if (_data->Visible[ni] == 1)
                            {
                                if (nX > posX) _data->Faces[n] &= (byte)~QubeFace.Right;
                                if (nX < posX) _data->Faces[n] &= (byte)~QubeFace.Left;
                                if (nY > posY) _data->Faces[n] &= (byte)~QubeFace.Top;
                                if (nY < posY) _data->Faces[n] &= (byte)~QubeFace.Bot;
                                if (nZ > posZ) _data->Faces[n] &= (byte)~QubeFace.Front;
                                if (nZ < posZ) _data->Faces[n] &= (byte)~QubeFace.Back;
                            }
                            else
                            {
                                if (floor)
                                {
                                    if (nY > posY)
                                    {
                                        _data->Visible[n] = 1;
                                        _data->Faces[n] |= (byte)QubeFace.Top;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}