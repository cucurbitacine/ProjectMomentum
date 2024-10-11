using System;
using UnityEngine;

namespace Game.Scripts.Levels
{
    [Serializable]
    public struct Chunk
    {
        public Vector2Int index;
        public Vector2 size;
        public Vector2 worldCenter;
        
        public Vector2 localCenter => Vector2.Scale(index, size);
        public Vector2 center => localCenter + worldCenter;
        public Vector2 extents => size * 0.5f;
        public Vector2 min => center - extents;
        public Vector2 max => center + extents;

        public Chunk up => Offset(0, 1);
        public Chunk right => Offset(1, 0);
        public Chunk down => Offset(0, -1);
        public Chunk left => Offset(-1, 0);
        
        public Chunk upRight => Offset(1, 1);
        public Chunk downRight => Offset(1, -1);
        public Chunk downLeft => Offset(-1, -1);
        public Chunk upLeft => Offset(-1, 1);
        
        public Chunk(Vector2Int index, Vector2 size, Vector2 worldCenter)
        {
            this.index = index;
            this.size = size;
            this.worldCenter = worldCenter;
        }

        public Chunk(int i, int j, Vector2 size, Vector2 worldCenter) : this(new Vector2Int(i, j), size, worldCenter)
        {
        }

        public Chunk Offset(Vector2Int offset)
        {
            return new Chunk(index + offset, size, worldCenter);
        }

        public Chunk Offset(int i, int j)
        {
            return Offset(new Vector2Int(i, j));
        }
        
        public bool Contains(Vector2 point)
        {
            return Chunk.Contains(this, point);
        }
        
        public Chunk Divide(int dimension)
        {
            return Chunk.Divide(this, dimension);
        }
        
        public bool Equals(Chunk other)
        {
            return index.Equals(other.index);
        }
        
        public static Vector2Int GetIndex(Vector2 point, Vector2 chunkSize, Vector2 worldCenter)
        {
            var shiftedPosition = (point - worldCenter) + chunkSize * 0.5f;

            return new Vector2Int
            {
                x = Mathf.FloorToInt(shiftedPosition.x / chunkSize.x),
                y = Mathf.FloorToInt(shiftedPosition.y / chunkSize.y),
            };
        }

        public static Chunk GetChunk(Vector2 point, Vector2 chunkSize, Vector2 worldCenter)
        {
            return new Chunk(GetIndex(point, chunkSize, worldCenter), chunkSize, worldCenter);
        }
        
        public static bool Contains(Chunk chunk, Vector2 point)
        {
            return chunk.index.Equals(GetIndex(point, chunk.size, chunk.worldCenter));
        }
        
        public static Chunk Divide(Chunk master, int dimension)
        {
            dimension = dimension > 0 ? dimension : 1;
            
            return new Chunk(0, 0, master.size / dimension, master.center);
        }
    }
}