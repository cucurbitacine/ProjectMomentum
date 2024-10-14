using System.Collections;
using CucuTools;
using UnityEngine;

namespace Game.Scripts.Core
{
    [DisallowMultipleComponent]
    public sealed class Lifetime : MonoBehaviour
    {
        [SerializeField] [Min(0f)] private float duration = 30f;

        private Coroutine _destroying = null;

        private IEnumerator Destroying(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            SmartPrefab.SmartDestroy(gameObject);
        }

        private void OnEnable()
        {
            _destroying = StartCoroutine(Destroying(duration));
        }

        private void OnDisable()
        {
            if (_destroying != null) StopCoroutine(_destroying);
        }
    }
}