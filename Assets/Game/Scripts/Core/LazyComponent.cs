using System;
using UnityEngine;

namespace Game.Scripts.Core
{
    [Serializable]
    public class LazyComponent<T> : Lazy<T>
    {
        public LazyComponent(Func<T> builder) : base(builder)
        {
        }

        public LazyComponent(GameObject gameObject) : this(gameObject.GetComponent<T>)
        {
        }
    }

    [Serializable]
    public class Lazy<T>
    {
        [SerializeField] private T value;

        private readonly Func<T> builder;
        
        public T Value
        {
            get
            {
                if (value == null)
                {
                    value = builder.Invoke();
                }

                return value;
            }
        }
        
        public Lazy(Func<T> builder)
        {
            this.builder = builder;
        }
    }
}
