using UnityEngine;

namespace Game.Scripts.Interactions
{
    public interface IPickable
    {
        public bool IsReadyBePicked(GameObject actor);
        public void Pickup(GameObject actor);
    }
}
