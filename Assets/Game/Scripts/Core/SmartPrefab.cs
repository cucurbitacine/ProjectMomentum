using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core
{
    [DisallowMultipleComponent]
    public sealed class SmartPrefab : MonoBehaviour
    {
        public GameObject Prefab { get; private set; }

        public event Action Initialized;
        public event Action Uninitialized;

        private void Initialize()
        {
            Initialized?.Invoke();
        }

        private void Uninitialize()
        {
            Uninitialized?.Invoke();
        }
        
        private void OnDestroy()
        {
            HandleDestroy(this);
        }

        #region Static Pool
        
        private const int PoolSize = 32;

        private static readonly Dictionary<GameObject, List<SmartPrefab>> Pool = new Dictionary<GameObject, List<SmartPrefab>>();
        
        public static void AddToPool(SmartPrefab smartObject)
        {
            if (smartObject.Prefab)
            {
                var pool = GetPool(smartObject.Prefab);

                if (pool.Contains(smartObject)) return;

                if (pool.Count < PoolSize)
                {
                    pool.Add(smartObject);
                }
                else
                {
                    Destroy(smartObject.gameObject);
                }
            }
        }

        public static void RemoveFromPool(SmartPrefab smartObject)
        {
            if (smartObject.Prefab)
            {
                var pool = GetPool(smartObject.Prefab);

                pool.Remove(smartObject);
            }
        }

        public static GameObject SmartInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (!prefab.TryGetComponent<SmartPrefab>(out _))
            {
                // This is not a Smart Object

                return Instantiate(prefab, position, rotation, parent);
            }

            var pool = GetPool(prefab);

            if (pool.Count > 0)
            {
                // There are at least one Smart Object
                // Return it as new
                
                return InstantiateFromPool(pool[0], position, rotation, parent);
            }

            // Pool is empty
            // Return just created object

            return InstantiateNew(prefab, position, rotation, parent);
        }

        public static void SmartDestroy(SmartPrefab smartObject)
        {
            if (smartObject.Prefab)
            {
                // Remove its parent and hide it

                smartObject.Uninitialize();
                
                smartObject.transform.SetParent(null, true);
                smartObject.gameObject.SetActive(false);

                AddToPool(smartObject);
            }
            else
            {
                // It has no Prefab, so it must be destroyed 
                
                Destroy(smartObject.gameObject);
            }
        }
        
        public static void SmartDestroy(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<SmartPrefab>(out var smartObject))
            {
                // If it is a smart object, it is possible reuse it
                
                SmartDestroy(smartObject);
            }
            else
            {
                // It is not a smart object, or it has no prefab (already has been on a scene)

                Destroy(gameObject);
            }
        }

        public static GameObject SmartInstantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return SmartInstantiate(prefab, position, rotation, null);
        }

        public static GameObject SmartInstantiate(GameObject prefab)
        {
            return SmartInstantiate(prefab, Vector3.zero, Quaternion.identity);
        }

        public static T SmartInstantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent)
            where T : Component
        {
            return SmartInstantiate(prefab.gameObject, position, rotation, parent).GetComponent<T>();
        }

        public static T SmartInstantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return SmartInstantiate(prefab, position, rotation, null);
        }

        public static T SmartInstantiate<T>(T prefab) where T : Component
        {
            return SmartInstantiate(prefab, Vector3.zero, Quaternion.identity);
        }

        private static GameObject InstantiateFromPool(SmartPrefab smartObject, Vector3 position, Quaternion rotation, Transform parent)
        {
            RemoveFromPool(smartObject);
            
            smartObject.transform.SetPositionAndRotation(position, rotation);
            smartObject.transform.SetParent(parent, true);
            smartObject.gameObject.SetActive(true);

            smartObject.Initialize();
            
            return smartObject.gameObject;
        }

        private static GameObject InstantiateNew(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var gameObject = Instantiate(prefab, position, rotation, parent);
            
            if (gameObject.TryGetComponent<SmartPrefab>(out var smartObject))
            {
                smartObject.Prefab = prefab;

                smartObject.Initialize();
            }

            return gameObject;
        }

        private static List<SmartPrefab> GetPool(GameObject prefab)
        {
            if (Pool.TryGetValue(prefab, out var pool)) return pool;

            pool = new List<SmartPrefab>();

            Pool.Add(prefab, pool);

            return pool;
        }
        
        private static void HandleDestroy(SmartPrefab smartObject)
        {
            RemoveFromPool(smartObject);
        }

        #endregion
    }
}
