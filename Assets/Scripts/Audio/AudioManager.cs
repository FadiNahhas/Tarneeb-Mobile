using System;
using Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [SerializeField, Range(0, 1)] private float masterVolume = 1f;
        
        [SerializeField] private Sound[] sounds;
        private void OnValidate()
        {
            AudioListener.volume = masterVolume;
        }

        private void Start()
        {
            SceneManager.sceneLoaded += SetMasterVolume;
            InitializeSounds();
        }

        private void SetMasterVolume(Scene arg0, LoadSceneMode arg1)
        {
            AudioListener.volume = masterVolume;
        }
        
        private void InitializeSounds()
        {
            foreach (var sound in sounds)
            {
                GameObject soundObject = new GameObject("Sound: " + sound.name);
                soundObject.transform.SetParent(transform);
                sound.Initialize(soundObject.AddComponent<AudioSource>());
            }
        }

        public void PlaySound(string soundName)
        {
            var sound = Array.Find(sounds, s => s.name == soundName);
            if (sound == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
                return;
            }
            sound.Play();
        }
    }
}
