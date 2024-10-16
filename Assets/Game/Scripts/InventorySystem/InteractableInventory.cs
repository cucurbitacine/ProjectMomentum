using System.Collections;
using System.Collections.Generic;
using CucuTools;
using Game.Scripts.Interactions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.InventorySystem
{
    public class InteractableInventory : MonoBehaviour, IInteractable, IGateway
    {
        public enum StorageMode
        {
            Source,
            Receiver,
        }
        
        [Header("Settings")]
        [SerializeField] private StorageMode mode = StorageMode.Receiver;
        [SerializeField] private GameObject storageObject;
        
        [Space]
        [SerializeField] private float followDuration = 1f;
        [SerializeField] private Transform gateway;

        [Header("SFX")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioClip> pickSfx = new List<AudioClip>();
        [SerializeField] private List<AudioClip> putSfx = new List<AudioClip>();
        
        private float _timerLoading;
        private IInventory _storage;
        private IInventoryWithGateway _agentInventory;

        public Transform Gateway => gateway ? gateway.transform : transform;

        public bool IsReadyBeInteracted(GameObject actor)
        {
            if (actor.TryGetComponent<IInventory>(out var outsource))
            {
                return mode == StorageMode.Source
                    ? _storage.IsPossibleGiveAnything(outsource)
                    : outsource.IsPossibleGiveAnything(_storage);
            }

            return false;
        }

        public void StartInteraction(GameObject actor)
        {
            if (!actor.TryGetComponent(out _agentInventory)) return;
            
            Interaction(_agentInventory);
        }

        public void StopInteraction(GameObject actor)
        {
        }
        
        private void Interaction(IInventoryWithGateway agentStorage)
        {
            if (mode == StorageMode.Source)
            {
                if (_storage.TryPickItem(out var item))
                {
                    var itemPrefab = item.GetPrefab();
                    
                    var spawnedItem = SmartPrefab.SmartInstantiate(itemPrefab, Gateway.position, Quaternion.Euler(0f, 0f, Random.value * 360f));

                    StartCoroutine(Throw(spawnedItem, _storage, agentStorage, Gateway, agentStorage.Gateway));
                }
            }
            else if (mode == StorageMode.Receiver)
            {
                if (agentStorage.TryPickItem(out var item))
                {
                    var itemPrefab = item.GetPrefab();
                    
                    var spawnedItem = SmartPrefab.SmartInstantiate(itemPrefab, agentStorage.Gateway.position, Quaternion.Euler(0f, 0f, Random.value * 360f));
                    
                    StartCoroutine(Throw(spawnedItem, agentStorage, _storage, agentStorage.Gateway, Gateway));
                }
            }
        }
        
        private IEnumerator Throw(GameObject obj, IInventory source, IInventory receiver, Transform origin, Transform destination)
        {
            audioSource?.PlayOneShot(pickSfx);
            
            var originPosition = origin.position;
            var originRotation = obj.transform.rotation;
            
            var targetRotation = Quaternion.Euler(0f,0f, Random.value * 360f);
            
            obj.transform.position = originPosition;

            var timer = followDuration;
            while (timer > 0f)
            {
                var t = 1f - Mathf.Clamp01(timer / followDuration);
                obj.transform.position = Vector3.Lerp(originPosition, destination.position, t);
                obj.transform.rotation = Quaternion.Lerp(originRotation, targetRotation, t);
                
                timer -= Time.deltaTime;
                yield return null;
            }
            
            obj.transform.position = destination.position;
            obj.transform.rotation = targetRotation;

            if (obj.TryGetComponent<IItem>(out var item) && item.Config)
            {
                receiver.TryPut(item.Config);
            }
            
            audioSource?.PlayOneShot(putSfx);
            
            SmartPrefab.SmartDestroy(obj);
        }
        
        private void Awake()
        {
            storageObject.TryGetComponent(out _storage);

            pickSfx.RemoveAll(s => s == null);
            putSfx.RemoveAll(s => s == null);
        }
    }
}