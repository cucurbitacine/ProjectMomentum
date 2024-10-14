using System.Collections;
using System.Collections.Generic;
using CucuTools;
using Game.Scripts.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Interactions
{
    public class InteractableStorage : MonoBehaviour, IInteractable, IGateway
    {
        public enum StorageMode
        {
            Source,
            Receiver,
        }
        
        [SerializeField] private bool interacting = false;
        
        [Header("Settings")]
        [SerializeField] private StorageMode mode = StorageMode.Receiver;
        [SerializeField] private GameObject storageObject;
        
        [Space]
        [SerializeField] [Min(0.01f)] private float loadingRate = 1f;
        [SerializeField] private float followDuration = 1f;
        [SerializeField] private Transform gateway;
        
        [Header("Prefab")]
        [SerializeField] private GameObject itemPrefab;

        [Header("SFX")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioClip> pickSfx = new List<AudioClip>();
        [SerializeField] private List<AudioClip> putSfx = new List<AudioClip>();
        
        private float _timerLoading;
        private Coroutine _loading = null;
        private IStorage _mainStorage;
        private IStorageWithGateway _agentStorage;

        private float loadingPeriod => loadingRate > 0f ? 1f / loadingRate : 1f;

        public Transform Gateway => gateway ? gateway.transform : transform;

        public bool IsValid(GameObject actor)
        {
            if (actor.TryGetComponent<IStorage>(out var agent))
            {
                return mode == StorageMode.Source ? _mainStorage.Amount > 0 : agent.Amount > 0;
            }

            return false;
        }

        public void BeginInteraction(GameObject actor)
        {
            if (!actor.TryGetComponent(out _agentStorage)) return;
            
            Load(_agentStorage);
            
            return;
            
            if (interacting) return;

            if (!actor.TryGetComponent(out _agentStorage)) return;
            
            interacting = true;

            if (_loading != null) StopCoroutine(_loading);
            _loading = StartCoroutine(Loading(_agentStorage));
        }

        public void EndInteraction(GameObject actor)
        {
            return;
            
            if (!interacting) return;

            if (!actor.TryGetComponent<IStorageWithGateway>(out var storage) || storage != _agentStorage) return;
            
            interacting = false;

            if (_loading != null) StopCoroutine(_loading);
        }

        private IEnumerator Loading(IStorageWithGateway agentStorage)
        {
            while (true)
            {
                if (_timerLoading <= 0f)
                {
                    Load(agentStorage);
                    
                    _timerLoading = loadingPeriod;
                }
                else
                {
                    _timerLoading -= Time.deltaTime;
                }
                
                yield return null;
            }
        }
        
        private void Load(IStorageWithGateway agentStorage)
        {
            if (mode == StorageMode.Source)
            {
                if (_mainStorage.Amount > 0)
                {
                    var item = SmartPrefab.SmartInstantiate(itemPrefab, Gateway.position, Quaternion.Euler(0f, 0f, Random.value * 360f));

                    StartCoroutine(Throw(item, _mainStorage, agentStorage, Gateway, agentStorage.Gateway));
                }
            }
            else if (mode == StorageMode.Receiver)
            {
                if (agentStorage.Amount > 0)
                {
                    var item = SmartPrefab.SmartInstantiate(itemPrefab, agentStorage.Gateway.position, Quaternion.Euler(0f, 0f, Random.value * 360f));
                    
                    StartCoroutine(Throw(item, agentStorage, _mainStorage, agentStorage.Gateway, Gateway));
                }
            }
        }
        
        private IEnumerator Throw(GameObject item, IStorage source, IStorage receiver, Transform origin, Transform destination)
        {
            source.Amount -= 1;
            
            audioSource?.PlayOneShot(pickSfx);
            
            var originPosition = origin.position;
            var originRotation = item.transform.rotation;
            
            var targetRotation = Quaternion.Euler(0f,0f, Random.value * 360f);
            
            item.transform.position = originPosition;

            var timer = followDuration;
            while (timer > 0f)
            {
                var t = 1f - Mathf.Clamp01(timer / followDuration);
                item.transform.position = Vector3.Lerp(originPosition, destination.position, t);
                item.transform.rotation = Quaternion.Lerp(originRotation, targetRotation, t);
                
                timer -= Time.deltaTime;
                yield return null;
            }
            
            item.transform.position = destination.position;
            item.transform.rotation = targetRotation;

            receiver.Amount += 1;
            audioSource?.PlayOneShot(putSfx);
            
            SmartPrefab.SmartDestroy(item);
        }
        
        private void Awake()
        {
            storageObject.TryGetComponent(out _mainStorage);

            pickSfx.RemoveAll(s => s == null);
            putSfx.RemoveAll(s => s == null);
        }
    }
}