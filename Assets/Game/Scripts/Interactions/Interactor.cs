using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Interactions
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private bool interacting = false;
        
        private readonly HashSet<IInteractable> _interactions = new HashSet<IInteractable>();
        
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
                _interactions.Add(interaction);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<IInteractable>(out var interaction))
            {
                if (_interactions.Remove(interaction) && interacting)
                {
                    interaction.EndInteraction(gameObject);
                }
            }
        }
    }
}
