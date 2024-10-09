using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Core
{
    public class InvokeOnStart : MonoBehaviour
    {
        [SerializeField] private UnityEvent onStart = new UnityEvent();

        private void Start()
        {
            onStart.Invoke();
        }
    }
}