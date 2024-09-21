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
    }
}