using System.Collections.Generic;
using Game.Scripts.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Levels
{
    public class RingSpawnerSystem : MonoBehaviour
    {
        [SerializeField] [Min(0f)] private float outerRadius = 30f;
        [SerializeField] [Min(0f)] private float innerRadius = 10f;
        [SerializeField] [Min(1)] private int amountPerCircle = 1; 
        
        [Space]
        [SerializeField] private LayerMask obstacleLayer = 1; 
        
        [Space]
        [SerializeField] private GameObject prefab;

        private Vector2 center => transform.position;
        
        public List<GameObject> Spawn()
        {
            var ships = new List<GameObject>();
            
            for (var i = 0; i < amountPerCircle; i++)
            {
                var point = GetRandomPoint();
                var angle = Random.value * 180f;
                var rotation = Quaternion.Euler(0f, 0f, angle);
                
                var ship = SmartPrefab.SmartInstantiate(GetPrefab(), point, rotation, transform);
                
                ships.Add(ship);
            }

            return ships;
        }

        private Vector2 GetRandomPoint()
        {
            var randomRadius = Random.value * (outerRadius - innerRadius) + innerRadius;
            var randomDirection = Random.insideUnitCircle.normalized;
            return center + randomDirection * randomRadius;
        }

        private GameObject GetPrefab()
        {
            return prefab;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
            Gizmos.DrawWireSphere(center, innerRadius);
            
            Gizmos.color = new Color(0f, 0f, 1f, 0.1f);
            Gizmos.DrawWireSphere(center, outerRadius);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, innerRadius);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(center, outerRadius);
        }
    }
}