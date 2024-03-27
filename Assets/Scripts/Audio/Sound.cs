using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        private AudioSource _source;
        
        [Range(0f, 1f)] public float volume;

        public void Initialize(AudioSource source)
        {
            _source = source;
            _source.clip = clip;
            _source.volume = volume;
        }

        public void Play() => _source.Play();
    }
}