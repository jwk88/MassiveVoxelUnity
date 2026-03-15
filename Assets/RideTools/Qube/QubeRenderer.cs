using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace RideTools.Qube
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class QubeRenderer : MonoBehaviour
    {
        MeshFilter _filter;
        Mesh _mesh;
        QubeMesh.VertexData _result;

        public bool Initialized { get; private set; }

        public unsafe void Initialize()
        {
            _filter = GetComponent<MeshFilter>();
            _mesh = new Mesh();
            _mesh.indexFormat = IndexFormat.UInt16;
            _mesh.MarkDynamic();
            _filter.mesh = _mesh;
            
            Initialized = true;
        }

        public unsafe void Run(int offset, QubePoint* points, QubeData* data, QubeConfig cfg)
        {
            if (!Initialized)
                throw new Exception("You are trying to run a QubeRenderer before it has been initialized");

            var width = cfg.ChunkWidth;
            var height = cfg.ChunkHeight;
            var length = cfg.ChunkLength;
            var size = cfg.ChunkSize;
            
            var fullWidth = cfg.Width;
            var step = cfg.Step;

            QubeMesh.SetWorldPositions(offset, fullWidth, points, data, width, height, length, step);

            if (cfg.UseNoiseTexture)
            {
                var heightRange = new Vector2Int(cfg.HeightNoiseMin, cfg.HeightNoiseMax);
                var scale = cfg.NoiseScale;
                QubeMesh.FilterByNoise(points, data, width, height, length, heightRange, scale);
            }

            QubeMesh.ProcessFaces(points, data, width, height, length);
            _result = QubeMesh.Build(points, data, size, step);
        }

        public void ApplyMesh()
        {
            _mesh.SetVertices(_result.Vertices);
            _mesh.SetIndices(_result.Triangles, MeshTopology.Triangles, 0);
            _mesh.SetUVs(0, _result.UVs);
            _mesh.RecalculateNormals();

            _result.Vertices.Dispose();
            _result.Triangles.Dispose();
            _result.UVs.Dispose();
        }

        public void ClearMesh()
        {
            _mesh.Clear();
        }
    }
}