using UnityEngine;

namespace Game.Scripts.Interactions
{
    public interface IPickable
    {
        public bool IsValid(GameObject actor);
        public void Pickup(GameObject actor);
    }
}
