using System.Collections.Generic;
using Game.Scripts.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Levels
{
    public class ShipwreckGenerator : MonoBehaviour
    {
        [SerializeField] [Min(0f)] private float outerRadius = 30f;
        [SerializeField] [Min(0f)] private float innerRadius = 10f;
        [SerializeField] [Min(1)] private int amountPerCircle = 1; 
        
        [Space]
        [SerializeField] private LayerMask obstacleLayer = 1; 
        
        [Space]
        [SerializeField] private GameObject shipwreckPrefab;

        private Vector2 center => transform.position;
        
        public List<GameObject> Generate()
        {
            var ships = new List<GameObject>();
            
            for (var i = 0; i < amountPerCircle; i++)
            {
                var point = GetRandomPoint();
                var rotation = Random.value * 180f;
                
                var ship = SmartPrefab.SmartInstantiate(GetPrefab());

                ship.transform.SetPositionAndRotation(point, Quaternion.Euler(0f, 0f, rotation));
                ship.transform.SetParent(transform, true);
                
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
            return shipwreckPrefab;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, innerRadius);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(center, outerRadius);
        }
    }
}