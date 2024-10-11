using System.Collections.Generic;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Levels
{
    public class ChunkGenerationSystem : MonoBehaviour
    {
        [SerializeField] private bool generating = false;
        [SerializeField] private LayerMask obstacleLayer = 1;
        
        [Header("Physics")]
        [SerializeField] private bool doPhysics = true;
        [SerializeField] [Min(0f)] private float torqueMax = 1f;
        [SerializeField] [Min(0f)] private float forceMax = 1f;
        
        [Header("References")]
        [SerializeField] private GameObject agentObject;
        [SerializeField] private GameObject configSourceObject;
        
        private readonly HashSet<Vector2Int> visitedChunks = new HashSet<Vector2Int>();
        private readonly List<Collider2D> overlap = new List<Collider2D>();
        
        private IChunkAgent agent;
        private ISpawnConfigSource spawnConfigSource;

        public void StartGeneration()
        {
            if (generating) return;

            generating = true;
            
            OnAgentChunkChanged(agent.GetChunk());
        }
        
        private Vector2 GetRandomPointInChunk(Chunk chunk)
        {
            return chunk.center + Vector2.Scale(new Vector2(Random.value, Random.value) - Vector2.one * 0.5f, chunk.size);
        }
        
        private Vector2 GetPrefabSize(GameObject prefab)
        {
            return prefab.TryGetComponent<Renderer>(out var render)
                ? render.bounds.size
                : Vector2.one;
        }
        
        private void SpawnPrefab(GameObject prefab, Chunk chunk)
        {
            var point = GetRandomPointInChunk(chunk);
            var angle = Random.value * 360f;
            var rotation = Quaternion.Euler(0, 0, angle);

            var prefabSize = GetPrefabSize(prefab);

            var filter = new ContactFilter2D()
            {
                useLayerMask = true,
                layerMask = obstacleLayer,
                useTriggers = false,
            };
                        
            var count = Physics2D.OverlapBox(point, prefabSize, angle, filter, overlap);
                        
            if (count > 0)
            {
                return;
            }

            var spawned = SmartPrefab.SmartInstantiate(prefab, point, rotation, transform);

            if (spawned.TryGetComponent<IRigidbody2D>(out var rigid2d))
            {
                rigid2d.Simulate(false);
                            
                if (doPhysics)
                {
                    rigid2d.AddTorque((Random.value * 2f - 1f) * torqueMax, ForceMode2D.Impulse);
                    rigid2d.AddForce(Random.insideUnitCircle * forceMax, ForceMode2D.Impulse);
                }
            }
        }
        
        private void GenerationChunk(Chunk chunk)
        {
            if (!visitedChunks.Add(chunk.index)) return;

            if (spawnConfigSource == null) return;
            
            var spawnConfig = spawnConfigSource.GetSpawnConfig(chunk);

            for (var i = 0; i < spawnConfig.amount; i++)
            {
                var prefab = spawnConfig.prefabSource.GetPrefab();

                SpawnPrefab(prefab, chunk);
            }
        }
        
        private void OnAgentChunkChanged(Chunk chunk)
        {
            if (!generating) return;
            
            // Spawn into neighbors and itself
            
            GenerationChunk(chunk);
            GenerationChunk(chunk.up);
            GenerationChunk(chunk.upRight);
            GenerationChunk(chunk.right);
            GenerationChunk(chunk.downRight);
            GenerationChunk(chunk.down);
            GenerationChunk(chunk.downLeft);
            GenerationChunk(chunk.left);
            GenerationChunk(chunk.upLeft);
        }

        private void Awake()
        {
            agentObject.TryGetComponent(out agent);

            if (configSourceObject == null) configSourceObject = gameObject;
            configSourceObject.TryGetComponent(out spawnConfigSource);
        }

        private void OnEnable()
        {
            agent.ChunkChanged += OnAgentChunkChanged;
        }

        private void OnDisable()
        {
            agent.ChunkChanged -= OnAgentChunkChanged;
        }

        private void Start()
        {
            if (generating)
            {
                OnAgentChunkChanged(agent.GetChunk());
            }
        }
    }
}