using System;
using UnityEngine;

namespace Game.Scripts.Levels
{
    public class ChunkAgent : MonoBehaviour, IChunkAgent
    {
        [SerializeField] private Transform target;
        
        [Header("Chunk Settings")]
        [SerializeField] [Min(0f)] private float chunkWidth = 100f;
        [SerializeField] private Vector2 chunkWorldCenter = Vector2.zero;
        
        [Header("Gizmos Settings")]
        [SerializeField] private Chunk editorChunk;
        [SerializeField] private bool gizmosGridEnabled = true;
        [SerializeField] private Color gizmosGridColor = new Color(1f, 1f, 1f, 0.1f);
        [SerializeField] [Min(0f)] private float gizmosGridScaleDisplay = 5f;

        private Chunk _lastChunk;
        
        private float radiusDisplay => chunkWidth * gizmosGridScaleDisplay;

        public Vector2 Position => target ? target.position : transform.position;
        public Vector2 ChunkSize => Vector2.one * chunkWidth;
        public Vector2 WorldCenter => chunkWorldCenter;

        public event Action<Chunk> ChunkChanged; 
        
        public Chunk GetChunk()
        {
            return Chunk.GetChunk(Position, ChunkSize, WorldCenter);
        }

        private void UpdateChunk()
        {
            var index = Chunk.GetIndex(Position, ChunkSize, WorldCenter);

            if (_lastChunk.index.Equals(index)) return;

            _lastChunk = new Chunk(index, ChunkSize, WorldCenter);
            
            ChunkChanged?.Invoke(_lastChunk);
        }
        
        private void Start()
        {
            _lastChunk = GetChunk();
        }

        private void Update()
        {
            UpdateChunk();
        }

        private void OnValidate()
        {
            editorChunk = GetChunk();
        }

        private void OnDrawGizmos()
        {
            editorChunk = GetChunk();
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(editorChunk.center, editorChunk.size);

            if (gizmosGridEnabled)
            {
                var numberDisplay = chunkWidth > 0 ? (int)(radiusDisplay / chunkWidth) : 0;
                for (var i = -numberDisplay; i <= numberDisplay; i++)
                {
                    for (var j = -numberDisplay; j <= numberDisplay; j++)
                    {
                        if (i == 0 && j == 0) continue;
                    
                        var gizmosSelectedChunk = GetChunk().Offset(i, j);

                        var t = Vector2.Distance(gizmosSelectedChunk.center, Position) / radiusDisplay;
                        Gizmos.color = Color.Lerp(gizmosGridColor, Color.clear, Mathf.Clamp01(t));

                        Gizmos.DrawWireCube(gizmosSelectedChunk.center, gizmosSelectedChunk.size);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(Position, chunkWidth * 0.05f);
            
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(WorldCenter, chunkWidth * 0.05f);
            
            Gizmos.color = Color.Lerp(Color.clear, Color.white, 0.5f);
            Gizmos.DrawLine(WorldCenter, Position);
        }
    }

    public interface IChunkAgent
    {
        public event Action<Chunk> ChunkChanged;
        public Chunk GetChunk();
    }
}