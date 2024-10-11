using Game.Scripts.Core;

namespace Game.Scripts.Levels
{
    public interface ISpawnConfigSource
    {
        public SpawnConfig GetSpawnConfig(Chunk chunk);
    }
    
    public struct SpawnConfig
    {
        public IPrefabSource prefabSource;
        public int amount;
    }
}