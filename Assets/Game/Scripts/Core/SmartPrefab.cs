using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core
{
    [DisallowMultipleComponent]
    public sealed class SmartPrefab : MonoBehaviour
    {
        private static readonly Dictionary<GameObject, List<SmartPrefab>> PrefabToObject = new Dictionary<GameObject, List<SmartPrefab>>();

        public static event Action<SmartPrefab> OnInstantiated;

        public static GameObject SmartInstantiate(GameObject prefab)
        {
            if (!prefab.TryGetComponent<SmartPrefab>(out var smartPrefab))
            {
                return Instantiate(prefab);
            }
            
            if (!PrefabToObject.TryGetValue(smartPrefab.gameObject, out var list))
            {
                list = new List<SmartPrefab>();
                
                PrefabToObject.Add(smartPrefab.gameObject, list);
            }

            foreach (var value in list)
            {
                if (value.IsFree)
                {
                    value.Busy();
                    OnInstantiated?.Invoke(value);
                    return value.gameObject;
                }
            }

            var smartObject = Instantiate(smartPrefab);
            smartObject.Prefab = smartPrefab.gameObject;
            list.Add(smartObject);
            
            smartObject.Busy();
            OnInstantiated?.Invoke(smartObject);
            return smartObject.gameObject;
        }
        
        public static T SmartInstantiate<T>(T prefab) where T : Component
        {
            return SmartInstantiate(prefab.gameObject).GetComponent<T>();
        }

        public static void SmartDestroy(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<SmartPrefab>(out var smartObject) && smartObject.Prefab)
            {
                smartObject.Release();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private static void HandleDestroy(SmartPrefab value)
        {
            if (value.Prefab && PrefabToObject.TryGetValue(value.Prefab, out var list))
            {
                list.Remove(value);
            }
        }
        
        [field: SerializeField] public bool IsFree { get; private set; }
        public GameObject Prefab { get; private set; }
        
        public event Action<bool> OnReleased; 
        
        [ContextMenu(nameof(Busy))]
        public void Busy()
        {
            IsFree = false;
            
            gameObject.SetActive(true);
            
            OnReleased?.Invoke(false);
        }
        
        [ContextMenu(nameof(Release))]
        public void Release()
        {
            IsFree = true;
            
            OnReleased?.Invoke(true);
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            HandleDestroy(this);
        }
    }
}
