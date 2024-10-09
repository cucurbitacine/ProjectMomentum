using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class SmartRigidbodyManager : MonoBehaviour
    {
        [SerializeField] private float radius = 50f;
        
        private readonly HashSet<IRigidbody2D> _set = new HashSet<IRigidbody2D>();
        private LazyComponent<Rigidbody2D> _lazyRigidbody2D;
        private LazyComponent<CircleCollider2D> _lazyCircle;

        private Rigidbody2D Rigid2d => (_lazyRigidbody2D ??= new LazyComponent<Rigidbody2D>(gameObject)).Value;
        private CircleCollider2D Circle => (_lazyCircle ??= new LazyComponent<CircleCollider2D>(gameObject)).Value;

        private void Awake()
        {
            Rigid2d.bodyType = RigidbodyType2D.Kinematic;
            Circle.radius = radius;
        }

        private void OnValidate()
        {
            Rigid2d.bodyType = RigidbodyType2D.Kinematic;
            Circle.radius = radius;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //Debug.Log($"{name} :: Enter {other.name}");
            
            if (other.TryGetComponent<IRigidbody2D>(out var rigid2d))
            {
                //Debug.Log($"{name} :: {other.name} is {nameof(IRigidbody2D)}");
                
                if (_set.Add(rigid2d))
                {
                    //Debug.Log($"{name} :: {other.name} is {nameof(IRigidbody2D)}");
                    
                    rigid2d.Simulate(true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<IRigidbody2D>(out var rigid2d))
            {
                if (_set.Remove(rigid2d))
                {
                    rigid2d.Simulate(false);
                }
            }
        }
    }
}