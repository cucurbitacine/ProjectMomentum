using System.Collections.Generic;
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

        public static void PlaySafe(this AudioSource audioSource, bool play)
        {
            if (play)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }

        public static void PlaySafe(this GameObject gameObject, bool value)
        {
            if (gameObject.TryGetComponent<ParticleSystem>(out var particle))
            {
                particle.PlaySafe(value);
            }

            if (gameObject.TryGetComponent<AudioSource>(out var audio))
            {
                audio.PlaySafe(value);
            }
        }

        public static void Play(this GameObject gameObject)
        {
            gameObject.PlaySafe(true);
        }

        public static void Stop(this GameObject gameObject)
        {
            gameObject.PlaySafe(false);
        }
        
        public static void PlayOneShot(this AudioSource audio, List<AudioClip> clips)
        {
            audio.PlayOneShot(clips != null && clips.Count > 0 ? clips[Random.Range(0, clips.Count)] : null);
        }

        public static void PlayOneShot(this GameObject gameObject, List<AudioClip> clips)
        {
            if (gameObject.TryGetComponent<AudioSource>(out var audio))
            {
                audio.PlayOneShot(clips);
            }
        }
        
        public static void PlayOneShot(this GameObject gameObject, AudioClip clip)
        {
            if (gameObject.TryGetComponent<AudioSource>(out var audio))
            {
                audio.PlayOneShot(clip);
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