using System;
using UnityEngine;

namespace Game.Scripts.Core
{
    [Serializable]
    public class LazyComponent<T>
    {
        [SerializeField] private T value;
        
        private readonly GameObject gameObject;

        public T Value
        {
            get
            {
                if (value == null)
                {
                    gameObject.TryGetComponent(out value);
                }

                return value;
            }
        }
        
        public LazyComponent(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
    }
}
