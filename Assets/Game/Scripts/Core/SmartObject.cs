using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core
{
    [DisallowMultipleComponent]
    public sealed class SmartObject : MonoBehaviour
    {
        private static readonly Dictionary<GameObject, List<SmartObject>> PrefabToObject = new Dictionary<GameObject, List<SmartObject>>();

        public static event Action<SmartObject> OnInstantiated;

        public static GameObject SmartInstantiate(GameObject prefab)
        {
            if (!prefab.TryGetComponent<SmartObject>(out var smartPrefab))
            {
                return Instantiate(prefab);
            }
            
            if (!PrefabToObject.TryGetValue(smartPrefab.gameObject, out var list))
            {
                list = new List<SmartObject>();
                
                PrefabToObject.Add(smartPrefab.gameObject, list);
            }

            foreach (var value in list)
            {
                if (value.IsFree)
                {
                    value.gameObject.SetActive(true);
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
            if (gameObject.TryGetComponent<SmartObject>(out var smartObject))
            {
                smartObject.Release();
                smartObject.gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private static void HandleDestroy(SmartObject value)
        {
            if (value.Prefab && PrefabToObject.TryGetValue(value.Prefab, out var list))
            {
                list.Remove(value);
            }
        }
        
        [field: SerializeField] public bool IsFree { get; private set; }
        public GameObject Prefab { get; private set; }
        
        public event Action<bool> OnReleased; 
        
        public void Busy()
        {
            IsFree = false;
            
            OnReleased?.Invoke(IsFree);
        }
        
        public void Release()
        {
            IsFree = true;
            
            OnReleased?.Invoke(IsFree);
        }

        private void OnDestroy()
        {
            HandleDestroy(this);
        }
    }
}
