using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace RideTools.Qube
{
    public unsafe struct QubePoint
    {
        public short* WorldX;
        public short* WorldY;
        public short* WorldZ;

        public static int AllocatedChunks { get; private set; }
        public static int AllocatedChunkSize { get; private set; }

        public static unsafe QubePoint* AllocateChunks(int size, int chunkSize, Allocator allocator)
        {
            var alignment = UnsafeUtility.AlignOf<QubePoint>();
            var bufferSize = sizeof(QubePoint) * size;
            var fieldsize = sizeof(short) * chunkSize;

            QubePoint* buffer = (QubePoint*)UnsafeUtility.Malloc(bufferSize, alignment, allocator);
            UnsafeUtility.MemClear(buffer, bufferSize);

            for (int i = 0; i < size; i++)
            {
                buffer[i].WorldX = (short*)UnsafeUtility.Malloc(fieldsize, 16, allocator);
                UnsafeUtility.MemClear(buffer[i].WorldX, fieldsize);

                buffer[i].WorldY = (short*)UnsafeUtility.Malloc(fieldsize, 16, allocator);
                UnsafeUtility.MemClear(buffer[i].WorldY, fieldsize);

                buffer[i].WorldZ = (short*)UnsafeUtility.Malloc(fieldsize, 16, allocator);
                UnsafeUtility.MemClear(buffer[i].WorldZ, fieldsize);
            }

            AllocatedChunks = size;
            AllocatedChunkSize = chunkSize;

            return buffer;
        }

        public static unsafe void Free(QubePoint* buffer, Allocator allocator)
        {
            for (int i = 0; i < AllocatedChunks; i++)
            {
                UnsafeUtility.Free(buffer[i].WorldX, allocator);
                UnsafeUtility.Free(buffer[i].WorldY, allocator);
                UnsafeUtility.Free(buffer[i].WorldZ, allocator);
            }

            UnsafeUtility.Free(buffer, allocator);
        }
    }
}