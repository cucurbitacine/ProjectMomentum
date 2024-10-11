using System.Linq;
using UnityEngine;

namespace Game.Scripts.Levels
{
    public class SpawnConfigSource : MonoBehaviour, ISpawnConfigSource
    {
        [SerializeField] private PrefabSpreadConfig spreadConfig;

        [Header("Gizmos")]
        [SerializeField] private bool gizmosEnabled = true;
        
        private Vector2 center => transform.position;
        
        public SpawnConfig GetSpawnConfig(Chunk chunk)
        {
            if (spreadConfig == null) return default;
            
            var distance = Vector2.Distance(center, chunk.center);

            var spreadData = spreadConfig.GetSpreadData(distance);

            if (spreadData.prefabList != null)
            {
                return new SpawnConfig()
                {
                    amount = (int)(spreadData.density * chunk.size.x * chunk.size.y),
                    prefabSource = spreadData.prefabList,
                };
            }
            
            return default;
        }

        private void OnDrawGizmos()
        {
            if (!gizmosEnabled) return;
            
            if (spreadConfig == null || spreadConfig.GetSpreading().Count == 0) return;
            
            var orderedZones = spreadConfig.GetSpreading().OrderBy(s => s.radius).ToList();
            
            var minRadius = orderedZones.Min(s => s.radius);
            var maxRadius = orderedZones.Max(z => z.radius);
            
            for (var i = 0; i < orderedZones.Count; i++)
            {
                var zone = orderedZones[i];
                
                var lerpByRadius = maxRadius > minRadius ? (zone.radius - minRadius) / (maxRadius - minRadius) : 0f;
                var lerpByIndex = (float)i / (orderedZones.Count - 1);
                var lerp = (lerpByRadius + lerpByIndex) * 0.5f;
                
                Gizmos.color = Color.Lerp(Color.green, Color.red, lerp);
                
                Gizmos.DrawWireSphere(center, zone.radius);
            }
        }

        
    }
}