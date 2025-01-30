using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class RandomAudioClip : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> _clips = new List<AudioClip>(); 
        
        private LazyComponent<AudioSource> _lazyAudio;
        
        public AudioSource Audio => (_lazyAudio ??= new LazyComponent<AudioSource>(gameObject)).Value;

        private AudioClip GetRandomClip()
        {
            return _clips != null && _clips.Count > 0 ? _clips[Random.Range(0, _clips.Count)] : null;
        }

        private void RandomClip()
        {
            Audio.clip = GetRandomClip();
        }
        
        private void Awake()
        {
            _clips.RemoveAll(c => c == null);
        }

        private void OnEnable()
        {
            RandomClip();
        }
    }
}