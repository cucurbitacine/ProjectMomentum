using UnityEngine;

namespace Game.Scripts.Interactions
{
    public interface IInteractable
    {
        public bool IsReadyInteractWith(GameObject actor);
        
        public void StartInteraction(GameObject actor);
        public void StopInteraction(GameObject actor);
    }
}