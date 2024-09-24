using System;
using UnityEngine;

namespace Game.Scripts.Interactions
{
    public class Picker : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform.TryGetComponent<IPickable>(out var pickable))
            {
                if(pickable.IsValid(gameObject))
                {
                    pickable.Pickup(gameObject);
                }
            }
        }
    }
}