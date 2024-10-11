using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Levels
{
    [CreateAssetMenu(menuName = "Game/Create PrefabSpreadConfig", fileName = "PrefabSpreadConfig", order = 0)]
    public class PrefabSpreadConfig : ScriptableObject
    {
        [SerializeField] [Min(0f)] private float testWidth = 100; 
        [SerializeField] [Min(0)] private int totalAmount = 100;
        
        [Space]
        [SerializeField] private List<PrefabSpreadData> spreading = new List<PrefabSpreadData>();

        public List<PrefabSpreadData> GetSpreading()
        {
            return spreading;
        }

        public PrefabSpreadData GetSpreadData(float distance)
        {
            var bestSpread = GetSpreading()
                .Where(zone => distance <= zone.radius && zone.prefabList != null)
                .OrderBy(zone => zone.radius)
                .FirstOrDefault();

            if (bestSpread.prefabList != null)
            {
                return bestSpread;
            }

            var defaultSpread = GetSpreading()
                .Where(zone => zone.prefabList != null)
                .OrderByDescending(zone => zone.radius)
                .FirstOrDefault();

            return defaultSpread;
        }
        
        private void OnValidate()
        {
            totalAmount = 0;
            for (var i = 0; i < spreading.Count; i++)
            {
                var zone = spreading[i];
                
                var amount = (int)(testWidth * testWidth * zone.density);
                zone.displayName = $"[{amount}] {zone.radius:F1} m";

                spreading[i] = zone;
                
                totalAmount += amount;
            }
        }
        
        [Serializable]
        public struct PrefabSpreadData
        {
            [HideInInspector] public string displayName;
            
            [Min(0f)] public float radius;
            [Min(0f)] public float density;
            public PrefabList prefabList;
        }
    }
}