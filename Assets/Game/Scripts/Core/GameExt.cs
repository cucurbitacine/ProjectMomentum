using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core
{
    public static class GameExt
    {
        #region ParticleSystem

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

        #endregion
        
        #region AudioSource

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
        
        public static void PlayOneShot(this AudioSource audio, List<AudioClip> clips)
        {
            if (clips != null && clips.Count > 0)
            {
                audio.PlayOneShot(clips[Random.Range(0, clips.Count)]);
            }
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

        #endregion

        #region LayerMask

        public static bool CompareLayer(this LayerMask layerMask, int layer)
        {
            return (layerMask.value & (1 << layer)) > 0;
        }
        
        public static bool CompareLayer(this GameObject gameObject, LayerMask layerMask)
        {
            return layerMask.CompareLayer(gameObject.layer);
        }

        public static bool CompareLayer(this Component component, LayerMask layerMask)
        {
            return component.gameObject.CompareLayer(layerMask);
        }

        #endregion
        
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

        public static void PlaySafe(this GameObject gameObject)
        {
            gameObject.PlaySafe(true);
        }

        public static void StopSafe(this GameObject gameObject)
        {
            gameObject.PlaySafe(false);
        }
    }
}