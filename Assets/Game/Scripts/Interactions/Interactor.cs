using System;
using System.Collections.Generic;
using CucuTools;
using UnityEngine;

namespace Game.Scripts.Interactions
{
    public class Interactor : MonoBehaviour, IPausable
    {
        [SerializeField] private bool interacting = false;

        public HashSet<IInteractable> SetInteractions { get; } = new HashSet<IInteractable>();

        public event Action<HashSet<IInteractable>> SetUpdated;
        
        public void BeginInteract()
        {
            if (IsPaused) return;
            
            if (interacting) return;
            
            interacting = true;
            
            foreach (var interaction in SetInteractions)
            {
                interaction.StartInteraction(gameObject);
            }
        }
        
        public void EndInteract()
        {
            if (!interacting) return;
            
            interacting = false;
            
            foreach (var interaction in SetInteractions)
            {
                interaction.StopInteraction(gameObject);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IInteractable>(out var interaction))
            {
                if (SetInteractions.Add(interaction))
                {
                    SetUpdated?.Invoke(SetInteractions);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<IInteractable>(out var interaction))
            {
                if (SetInteractions.Remove(interaction))
                {
                    if (interacting)
                    {
                        interaction.StopInteraction(gameObject);
                    }
                    
                    SetUpdated?.Invoke(SetInteractions);
                }
            }
        }

        public bool IsPaused { get; private set; }
        
        public event Action<bool> Paused;

        public void Pause(bool value)
        {
            if (IsPaused == value) return;
            
            IsPaused = value;
            
            if (IsPaused)
            {
                EndInteract();
            }
            
            Paused?.Invoke(IsPaused);
        }
    }
}
