using System;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace RideTools.Qube
{
    public class QubeManager : MonoBehaviour
    {
        [SerializeField] NoiseTexturer _noise;
        [SerializeField] Material _material;
        
        [SerializeField]
        QubeConfig _config;

        unsafe QubePoint* _pointBuffer;
        unsafe QubeData* _dataBuffer;

        QubeRenderer _prototype;
        QubeRenderer[] _renderers;
        QubeRuntime _runtime;
        GUIStyle _style = new GUIStyle();
        private float deltaTime = 0.0f;

        void OnValidate()
        {
            if (_config != null && _config.UseNoiseTexture)
            {
                if (GetComponent<NoiseTexturer>() == null)
                {
                    _noise = gameObject.AddComponent<NoiseTexturer>();
                    _noise.SetDimensions(_config.FullWidth, _config.FullLength);
                }
            }
        }

        unsafe void Awake()
        {
            _style.fontSize = 24;
            _style.normal.textColor = Color.white;

            _prototype = new GameObject("Qube Renderer").AddComponent<QubeRenderer>();
            _prototype.GetComponent<MeshRenderer>().material = _material;
            _prototype.gameObject.SetActive(false);

            if (_config.UseNoiseTexture)
            {
                var buffer = _noise.Generate(_config.FullWidth, _config.FullLength);
                QubeMesh.SetColorBuffer(buffer, _config.FullWidth);
            }

            _pointBuffer = QubePoint.AllocateChunks(_config.ChunkCount, _config.ChunkSize, Allocator.Persistent);
            _dataBuffer = QubeData.AllocateChunks(_config.ChunkCount, _config.ChunkSize, Allocator.Persistent);
            _renderers = new QubeRenderer[_config.ChunkCount];
            _runtime = new QubeRuntime(_config);
            
            for (int i = 0; i < _config.ChunkCount; i++)
            {
                var renderer = Instantiate(_prototype, _prototype.transform.parent);
                renderer.gameObject.SetActive(true);
                renderer.Initialize();
                _renderers[i] = renderer;
            }

            RunTasks();
            ApplyMeshes();
        }

        void Update()
        {
            var yMax = _config.ChunkHeight - 1;

            _config.HeightNoiseMin = Math.Clamp(_config.HeightNoiseMin, 0, yMax);
            _config.HeightNoiseMax = Math.Clamp(_config.HeightNoiseMax, 0, yMax);
            _config.HeightNoiseMin = Math.Clamp(_config.HeightNoiseMin, 0, yMax - _config.HeightNoiseMax);
            _config.HeightNoiseMax = Math.Clamp(_config.HeightNoiseMax, 0, yMax - _config.HeightNoiseMin);
            _config.NoiseScale = Math.Clamp(_config.NoiseScale, 0, _config.HeightNoiseMax * _config.HeightNoiseMax);

            if (_runtime.NoiseScale != _config.NoiseScale)
            {
                RunTasks();
                ClearMeshes();
                ApplyMeshes();

                _runtime.NoiseScale = _config.NoiseScale;
            }

            if (_runtime.HeightRange.x != _config.HeightNoiseMin || _runtime.HeightRange.y != _config.HeightNoiseMax)
            {
                RunTasks();
                ClearMeshes();
                ApplyMeshes();

                _runtime.HeightRange = new Vector2Int(_config.HeightNoiseMin, _config.HeightNoiseMax);
            }

            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void ApplyMeshes()
        {
            for (int i = 0; i < _config.ChunkCount; i++)
            {
                var renderer = _renderers[i];
                renderer.ApplyMesh();
            }
        }

        void ClearMeshes()
        {
            for (int i = 0; i < _config.ChunkCount; i++)
            {
                var renderer = _renderers[i];
                renderer.ClearMesh();
            }
        }

        unsafe void RunTasks()
        {
            QubeMesh.VisibleCubes = 0;

            Task[] tasks = new Task[_config.ChunkCount];
            for (int i = 0; i < _config.ChunkCount; i++)
            {
                int taskNum = i;
                var points = _pointBuffer + i;
                var data = _dataBuffer + i;
                var renderer = _renderers[i];
                tasks[i] = Task.Run(() => { renderer.Run(taskNum, points, data, _config); });
            }

            Task.WaitAll(tasks);
        }

        private string FormatCount(int count)
        {
            if (count >= 1000000)
                return (count / 1000000f).ToString("0.0") + "M";
            else if (count >= 1000)
                return (count / 1000f).ToString("0") + "K";
            else
                return count.ToString();
        }

        void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 100, 20), "Constructed Cubes: " + FormatCount(QubeMesh.VisibleCubes), _style);
            float fps = 1.0f / deltaTime;
            GUI.Label(new Rect(Screen.width - 150, 10, 150, 20), string.Format("{0:0.} fps", fps), _style);
        }

        unsafe void Flush()
        {
            QubePoint.Free(_pointBuffer, Allocator.Persistent);
            QubeData.Free(_dataBuffer, Allocator.Persistent);
            QubeMesh.DisposeColorBuffer();
        }

        void OnDisable()
        {
            Flush();
        }
    }
}