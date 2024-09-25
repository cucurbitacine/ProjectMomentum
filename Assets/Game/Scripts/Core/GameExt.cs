using UnityEngine;

namespace Game.Scripts.Core
{
    public static class GameExt
    {
        public static void PlaySafe(this ParticleSystem particleSystem, bool play)
        {
            if (play)
            {
                if (!particleSystem.isPlaying)
                {
                    particleSystem.Play();
                }
            }
            else
            {
                if (particleSystem.isPlaying)
                {
                    particleSystem.Stop();
                }
            }
        }

        public static void PlayFX(this GameObject gameObject)
        {
            if (gameObject.TryGetComponent<ParticleSystem>(out var particleSystem))
            {
                particleSystem.Play();
            }

            if (gameObject.TryGetComponent<AudioSource>(out var audioSource))
            {
                audioSource.Play();
            }
        }

        public static bool CompareLayer(this GameObject gameObject, LayerMask layerMask)
        {
            return (layerMask.value & (1 << gameObject.layer)) > 0;
        }
        
        public static bool CompareLayer(this Transform transform, LayerMask layerMask)
        {
            return transform.gameObject.CompareLayer(layerMask);
        }
    }
}