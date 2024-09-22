using System.Collections;
using UnityEngine;

namespace Game.Scripts.Core
{
    [DisallowMultipleComponent]
    public sealed class Lifetime : MonoBehaviour
    {
        [SerializeField] [Min(0f)] private float duration = 30f;
        
        private Coroutine _destroying = null; 
        
        private IEnumerator Destroying(float timer)
        {
            yield return new WaitForSeconds(timer);

            _destroying = null;
            
            SmartObject.SmartDestroy(gameObject);
        }

        private void Destroying()
        {
            if (_destroying == null)
            {
                _destroying = StartCoroutine(Destroying(duration));
            }
        }
        
        private void HandleInstantiation(SmartObject smartObject)
        {
            if (smartObject.gameObject == gameObject)
            {
                Destroying();
            }
        }
        
        private void OnEnable()
        {
            SmartObject.OnInstantiated += HandleInstantiation;
        }

        private void OnDisable()
        {
            SmartObject.OnInstantiated -= HandleInstantiation;

            if (_destroying != null) StopCoroutine(_destroying);
            _destroying = null;
        }

        private void Start()
        {
            Destroying();
        }
    }
}