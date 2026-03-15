using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace RideTools.Qube
{
    public static class QubeVertices
    {
        public struct MeshBuildData
        {
            public NativeArray<Vector3> Vertices;
            public NativeArray<int> Triangles;
            public NativeArray<Vector2> UVs;
        }

        public unsafe static MeshBuildData Build(QubePoint* points, QubeData* data, QubeConfig cfg)
        {
            var vertexCount = 0;
            var triangleCount = 0;

            for (int n = 0; n < cfg.ChunkSize; n++)
            {
                if (data->Visible[n] == 0) continue;
                if (data->Faces[n] == 0) continue;

                if ((data->Faces[n] & (byte)QubeFace.Top) != 0)
                {
                    vertexCount += 4;
                    triangleCount += 6;
                }
                if ((data->Faces[n] & (byte)QubeFace.Bot) != 0)
                {
                    vertexCount += 4;
                    triangleCount += 6;
                }
                if ((data->Faces[n] & (byte)QubeFace.Front) != 0)
                {
                    vertexCount += 4;
                    triangleCount += 6;
                }
                if ((data->Faces[n] & (byte)QubeFace.Back) != 0)
                {
                    vertexCount += 4;
                    triangleCount += 6;
                }
                if ((data->Faces[n] & (byte)QubeFace.Right) != 0)
                {
                    vertexCount += 4;
                    triangleCount += 6;
                }
                if ((data->Faces[n] & (byte)QubeFace.Left) != 0)
                {
                    vertexCount += 4;
                    triangleCount += 6;
                }
            }

            var vertices = new NativeArray<Vector3>(vertexCount, Allocator.TempJob);
            var triangles = new NativeArray<int>(triangleCount, Allocator.TempJob);
            var uvs = new NativeArray<Vector2>(vertexCount, Allocator.TempJob);

            var vi = 0; var ti = 0;
            for (int n = 0; n < cfg.ChunkSize; n++)
            {
                if (data->Faces[n] == 0 && data->Visible[n] == 1)
                {
                    data->Visible[n] = 0;
                }

                if (data->Visible[n] == 0) continue;

                var x = points->WorldX[n];
                var y = points->WorldY[n];
                var z = points->WorldZ[n];

                var center = new Vector3Int(x, y, z);
                var halfSize = cfg.Step / 2;

                if ((data->Faces[n] & (byte)QubeFace.Top) != 0)
                {
                    Vector3 v0 = center + new Vector3(-halfSize, halfSize, -halfSize);
                    Vector3 v1 = center + new Vector3(-halfSize, halfSize, halfSize);
                    Vector3 v2 = center + new Vector3(halfSize, halfSize, halfSize);
                    Vector3 v3 = center + new Vector3(halfSize, halfSize, -halfSize);

                    vertices[vi] = v0;
                    vertices[vi + 1] = v1;
                    vertices[vi + 2] = v2;
                    vertices[vi + 3] = v3;

                    uvs[vi] = new Vector2(0, 0);
                    uvs[vi + 1] = new Vector2(0, 1);
                    uvs[vi + 2] = new Vector2(1, 1);
                    uvs[vi + 3] = new Vector2(1, 0);

                    triangles[ti] = vi + 0;
                    triangles[ti + 1] = vi + 1;
                    triangles[ti + 2] = vi + 2;
                    triangles[ti + 3] = vi + 0;
                    triangles[ti + 4] = vi + 2;
                    triangles[ti + 5] = vi + 3;

                    vi += 4;
                    ti += 6;
                }

                if ((data->Faces[n] & (byte)QubeFace.Bot) != 0)
                {
                    Vector3 v0 = center + new Vector3(-halfSize, -halfSize, -halfSize);
                    Vector3 v1 = center + new Vector3(-halfSize, -halfSize, halfSize);
                    Vector3 v2 = center + new Vector3(halfSize, -halfSize, halfSize);
                    Vector3 v3 = center + new Vector3(halfSize, -halfSize, -halfSize);

                    vertices[vi] = v0;
                    vertices[vi + 1] = v1;
                    vertices[vi + 2] = v2;
                    vertices[vi + 3] = v3;

                    uvs[vi] = new Vector2(0, 0);
                    uvs[vi + 1] = new Vector2(1, 0);
                    uvs[vi + 2] = new Vector2(1, 1);
                    uvs[vi + 3] = new Vector2(0, 1);

                    triangles[ti] = vi + 0;
                    triangles[ti + 1] = vi + 2;
                    triangles[ti + 2] = vi + 1;
                    triangles[ti + 3] = vi + 0;
                    triangles[ti + 4] = vi + 3;
                    triangles[ti + 5] = vi + 2;

                    vi += 4;
                    ti += 6;
                }

                if ((data->Faces[n] & (byte)QubeFace.Front) != 0)
                {
                    Vector3 v0 = center + new Vector3(-halfSize, -halfSize, halfSize);
                    Vector3 v1 = center + new Vector3(-halfSize, halfSize, halfSize);
                    Vector3 v2 = center + new Vector3(halfSize, halfSize, halfSize);
                    Vector3 v3 = center + new Vector3(halfSize, -halfSize, halfSize);

                    vertices[vi] = v0;
                    vertices[vi + 1] = v1;
                    vertices[vi + 2] = v2;
                    vertices[vi + 3] = v3;

                    uvs[vi] = new Vector2(1, 0);
                    uvs[vi + 1] = new Vector2(1, 1);
                    uvs[vi + 2] = new Vector2(0, 1);
                    uvs[vi + 3] = new Vector2(0, 0);

                    triangles[ti] = vi + 0;
                    triangles[ti + 1] = vi + 2;
                    triangles[ti + 2] = vi + 1;
                    triangles[ti + 3] = vi + 0;
                    triangles[ti + 4] = vi + 3;
                    triangles[ti + 5] = vi + 2;

                    vi += 4;
                    ti += 6;
                }

                if ((data->Faces[n] & (byte)QubeFace.Back) != 0)
                {
                    Vector3 v0 = center + new Vector3(-halfSize, -halfSize, -halfSize);
                    Vector3 v1 = center + new Vector3(-halfSize, halfSize, -halfSize);
                    Vector3 v2 = center + new Vector3(halfSize, halfSize, -halfSize);
                    Vector3 v3 = center + new Vector3(halfSize, -halfSize, -halfSize);

                    vertices[vi] = v0;
                    vertices[vi + 1] = v1;
                    vertices[vi + 2] = v2;
                    vertices[vi + 3] = v3;

                    uvs[vi] = new Vector2(0, 0);
                    uvs[vi + 1] = new Vector2(0, 1);
                    uvs[vi + 2] = new Vector2(1, 1);
                    uvs[vi + 3] = new Vector2(1, 0);

                    triangles[ti] = vi + 0;
                    triangles[ti + 1] = vi + 1;
                    triangles[ti + 2] = vi + 2;
                    triangles[ti + 3] = vi + 0;
                    triangles[ti + 4] = vi + 2;
                    triangles[ti + 5] = vi + 3;

                    vi += 4;
                    ti += 6;
                }

                if ((data->Faces[n] & (byte)QubeFace.Right) != 0)
                {
                    Vector3 v0 = center + new Vector3(halfSize, -halfSize, -halfSize);
                    Vector3 v1 = center + new Vector3(halfSize, -halfSize, halfSize);
                    Vector3 v2 = center + new Vector3(halfSize, halfSize, halfSize);
                    Vector3 v3 = center + new Vector3(halfSize, halfSize, -halfSize);

                    vertices[vi] = v0;
                    vertices[vi + 1] = v1;
                    vertices[vi + 2] = v2;
                    vertices[vi + 3] = v3;

                    uvs[vi] = new Vector2(0, 0);
                    uvs[vi + 1] = new Vector2(1, 0);
                    uvs[vi + 2] = new Vector2(1, 1);
                    uvs[vi + 3] = new Vector2(0, 1);

                    triangles[ti] = vi + 0;
                    triangles[ti + 1] = vi + 2;
                    triangles[ti + 2] = vi + 1;
                    triangles[ti + 3] = vi + 0;
                    triangles[ti + 4] = vi + 3;
                    triangles[ti + 5] = vi + 2;

                    vi += 4;
                    ti += 6;
                }

                if ((data->Faces[n] & (byte)QubeFace.Left) != 0)
                {
                    Vector3 v0 = center + new Vector3(-halfSize, -halfSize, -halfSize);
                    Vector3 v1 = center + new Vector3(-halfSize, -halfSize, halfSize);
                    Vector3 v2 = center + new Vector3(-halfSize, halfSize, halfSize);
                    Vector3 v3 = center + new Vector3(-halfSize, halfSize, -halfSize);

                    vertices[vi] = v0;
                    vertices[vi + 1] = v1;
                    vertices[vi + 2] = v2;
                    vertices[vi + 3] = v3;

                    uvs[vi] = new Vector2(1, 0);
                    uvs[vi + 1] = new Vector2(0, 0);
                    uvs[vi + 2] = new Vector2(0, 1);
                    uvs[vi + 3] = new Vector2(1, 1);

                    triangles[ti] = vi + 0;
                    triangles[ti + 1] = vi + 1;
                    triangles[ti + 2] = vi + 2;
                    triangles[ti + 3] = vi + 0;
                    triangles[ti + 4] = vi + 2;
                    triangles[ti + 5] = vi + 3;

                    vi += 4;
                    ti += 6;
                }
            }

            //mesh.Clear();
            //mesh.indexFormat = IndexFormat.UInt32;
            // mesh.SetVertices(vertices);
            // mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
            // mesh.SetUVs(0, uvs);
            // mesh.RecalculateNormals();

            // mesh.SetIndexBufferParams(triangles.Length, IndexFormat.UInt32);
            // var indexBuffer = mesh.GetIndexBuffer();
            // indexBuffer.SetData(triangles, 0, 0, triangles.Length);

            // // Set submesh
            // mesh.subMeshCount = 1;
            // mesh.SetSubMesh(0, new SubMeshDescriptor(0, triangles.Length));

            // mesh.SetUVs(0, uvs);
            // mesh.RecalculateNormals();

            return new MeshBuildData() { Vertices = vertices, Triangles = triangles, UVs = uvs };

            // vertices.Dispose();
            // uvs.Dispose();
            // triangles.Dispose();
        }
    }
}