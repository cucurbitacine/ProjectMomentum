using UnityEngine;

namespace Game.Scripts.Interactions
{
    public interface IInteractable
    {
        public bool IsValid(GameObject actor);
        public void BeginInteraction(GameObject actor);
        public void EndInteraction(GameObject actor);
    }
}