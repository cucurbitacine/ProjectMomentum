using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Core
{
    [CreateAssetMenu(menuName = "Game/Create PrefabList", fileName = "PrefabList", order = 0)]
    public class PrefabList : ScriptableObject, IPrefabSource
    {
        [SerializeField] private List<PrefabData> prefabData = new List<PrefabData>();

        public GameObject GetPrefab()
        {
            if (prefabData.Count == 0) return null;
            
            var total = prefabData.Sum(d => d.weight);

            if (total == 0)
            {
                return prefabData[Random.Range(0, prefabData.Count)].prefab;
            }

            var randomNumber = Random.Range(0, total);
            
            var leftValue = 0;
            foreach (var data in prefabData)
            {
                var rightValue = data.weight + leftValue;

                if (leftValue <= randomNumber && randomNumber < rightValue)
                {
                    return data.prefab;
                }

                leftValue = rightValue;
            }

            return prefabData[0].prefab;
        }

        private void OnValidate()
        {
            if (prefabData != null && prefabData.Count > 0)
            {
                var total = prefabData.Sum(d => d.weight);

                for (var i = 0; i < prefabData.Count; i++)
                {
                    var data = prefabData[i];

                    var chance = 100f * (total > 0 ? ((float)data.weight / total) : (1f / prefabData.Count));
                    var prefabName = $"{(data.prefab ? data.prefab.name : "NULL")}";
                    
                    data.displayName = $"{chance:F1}% {prefabName}";
                    prefabData[i] = data;
                }
            }
        }

        [Serializable]
        public struct PrefabData
        {
            [HideInInspector] public string displayName;
            public GameObject prefab;
            public int weight;
        }
    }

    public interface IPrefabSource
    {
        public GameObject GetPrefab();
    }

    public interface IIconSource
    {
        public Sprite GetIcon();
    }
}
