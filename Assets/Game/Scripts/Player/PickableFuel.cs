using Game.Scripts.Core;
using Game.Scripts.Interactions;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PickableFuel : MonoBehaviour, IPickable
    {
        [SerializeField] private float fuelAmount = 100;
        
        public bool IsValid(GameObject actor)
        {
            if (actor.TryGetComponent<PlayerController>(out var player))
            {
                return player.Spaceship.Fuel < player.Spaceship.FuelMax;
            }

            return false;
        }

        public void Pickup(GameObject actor)
        {
            if (actor.TryGetComponent<PlayerController>(out var player))
            {
                player.Spaceship.AddFuel(fuelAmount);
                
                SmartPrefab.SmartDestroy(gameObject);
            }
        }
    }
}