using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace RideTools.Qube
{
    public unsafe struct QubeData
    {
        public byte* Faces;
        public byte* Visible;

        public static int AllocatedChunks { get; private set; }
        public static int AllocatedChunkSize { get; private set; }

        public static unsafe QubeData* AllocateChunks(int size, int chunkSize, Allocator allocator)
        {
            var alignment = UnsafeUtility.AlignOf<QubeData>();
            var bufferSize = sizeof(QubeData) * size;
            var fieldsize = sizeof(byte) * chunkSize;

            QubeData* buffer = (QubeData*)UnsafeUtility.Malloc(bufferSize, alignment, allocator);
            UnsafeUtility.MemClear(buffer, bufferSize);

            for (int i = 0; i < size; i++)
            {
                buffer[i].Faces = (byte*)UnsafeUtility.Malloc(fieldsize, 16, allocator);
                UnsafeUtility.MemClear(buffer[i].Faces, fieldsize);

                buffer[i].Visible = (byte*)UnsafeUtility.Malloc(fieldsize, 16, allocator);
                UnsafeUtility.MemClear(buffer[i].Visible, fieldsize);
            }

            AllocatedChunks = size;
            AllocatedChunkSize = chunkSize;

            return buffer;
        }

        public static unsafe void Free(QubeData* buffer, Allocator allocator)
        {
            for (int i = 0; i < AllocatedChunks; i++)
            {
                UnsafeUtility.Free(buffer[i].Faces, allocator);
                UnsafeUtility.Free(buffer[i].Visible, allocator);
            }

            UnsafeUtility.Free(buffer, allocator);
        }
    }
}