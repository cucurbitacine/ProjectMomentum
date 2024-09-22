using System.Collections;
using Game.Scripts.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Interactions
{
    public class CatsStorage : MonoBehaviour, IInteractable, IStorage
    {
        public enum StorageMode
        {
            Source,
            Receiver,
        }
        
        [SerializeField] private bool interacting = false;

        [field: Space]
        [field: SerializeField] public int Amount { get; set; }
        [SerializeField] private StorageMode mode = StorageMode.Receiver;
        
        [Space]
        [SerializeField] [Min(0.01f)] private float loadingRate = 1f; 
        [SerializeField] private GameObject itemPrefab;
        
        [Space]
        [SerializeField] private float followDuration = 1f;
        [SerializeField] private Transform originFollow;
        
        private float _timerLoading;
        private Coroutine _loading = null;
        private IStorage _agent;
        
        private float loadingPeriod => loadingRate > 0f ? 1f / loadingRate : 1f;

        public Vector3 Gateway => originFollow ? originFollow.position : transform.position;
        
        public void BeginInteraction(GameObject actor)
        {
            if (interacting) return;

            if (!actor.TryGetComponent(out _agent)) return;
            
            interacting = true;

            if (_loading != null) StopCoroutine(_loading);
            _loading = StartCoroutine(Loading(_agent));
        }

        public void EndInteraction(GameObject actor)
        {
            if (!interacting) return;

            if (!actor.TryGetComponent<IStorage>(out var storage) || storage != _agent) return;
            
            interacting = false;

            if (_loading != null) StopCoroutine(_loading);
        }

        private IEnumerator Loading(IStorage agent)
        {
            while (true)
            {
                if (_timerLoading <= 0f)
                {
                    Load(agent);
                    
                    _timerLoading = loadingPeriod;
                }
                else
                {
                    _timerLoading -= Time.deltaTime;
                }
                
                yield return null;
            }
        }
        
        private void Load(IStorage agent)
        {
            if (mode == StorageMode.Source)
            {
                if (Amount > 0)
                {
                    Amount -= 1;

                    var item = SmartObject.SmartInstantiate(itemPrefab);
                    item.transform.rotation = Quaternion.Euler(0f, 0f, Random.value * 360f);

                    StartCoroutine(Throw(item, this, agent));
                }
            }
            else if (mode == StorageMode.Receiver)
            {
                if (agent.Amount > 0)
                {
                    agent.Amount -= 1;
                    
                    var item = SmartObject.SmartInstantiate(itemPrefab);
                    item.transform.rotation = Quaternion.Euler(0f, 0f, Random.value * 360f);
                    
                    StartCoroutine(Throw(item, agent, this));
                }
            }
        }
        
        private IEnumerator Throw(GameObject item, IStorage source, IStorage receiver)
        {
            var originPosition = source.Gateway;
            var originRotation = item.transform.rotation;
            
            var targetRotation = Quaternion.Euler(0f,0f, Random.value * 360f);
            
            item.transform.position = originPosition;

            var timer = followDuration;
            while (timer > 0f)
            {
                var t = 1f - Mathf.Clamp01(timer / followDuration);
                item.transform.position = Vector3.Lerp(originPosition, receiver.Gateway, t);
                item.transform.rotation = Quaternion.Lerp(originRotation, targetRotation, t);
                
                timer -= Time.deltaTime;
                yield return null;
            }
            
            item.transform.position = receiver.Gateway;
            item.transform.rotation = targetRotation;

            receiver.Amount += 1;
            
            SmartObject.SmartDestroy(item);
        }
    }
}