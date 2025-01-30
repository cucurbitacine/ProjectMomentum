using UnityEngine;

namespace Game.Scripts.Levels
{
    public abstract class Pointer : MonoBehaviour
    {
        public abstract bool IsPointing { get; protected set; }
        public abstract void SetName(string newDisplayName);
        public abstract void SetTarget(Transform newTarget);
        public abstract void Show(bool value);
    }
}