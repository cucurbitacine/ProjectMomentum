using UnityEngine;

namespace Game.Scripts.Interactions
{
    public interface IInteractable
    {
        public void BeginInteraction(GameObject actor);
        public void EndInteraction(GameObject actor);
    }
}