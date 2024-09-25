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
    }
}