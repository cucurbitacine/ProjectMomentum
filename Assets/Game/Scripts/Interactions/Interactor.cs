using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Interactions
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private bool interacting = false;
        
        private readonly HashSet<IInteractable> _interactions = new HashSet<IInteractable>();

        public HashSet<IInteractable> Interactions => _interactions;
        
        public event Action<HashSet<IInteractable>> OnSetChanged;
        
        public void BeginInteract()
        {
            if (interacting) return;
            
            interacting = true;
            
            foreach (var interaction in _interactions)
            {
                interaction.BeginInteraction(gameObject);
            }
        }
        
        public void EndInteract()
        {
            if (!interacting) return;
            
            interacting = false;
            
            foreach (var interaction in _interactions)
            {
                interaction.EndInteraction(gameObject);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IInteractable>(out var interaction))
            {
                if (_interactions.Add(interaction))
                {
                    OnSetChanged?.Invoke(_interactions);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<IInteractable>(out var interaction))
            {
                if (_interactions.Remove(interaction))
                {
                    if (interacting)
                    {
                        interaction.EndInteraction(gameObject);
                    }
                    
                    OnSetChanged?.Invoke(_interactions);
                }
            }
        }
    }
}
