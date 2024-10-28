using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace _Game.Scripts.Manager
{
    public class AudioManager : MonoBehaviour
    {
        public AudioSource[] soundEffects;
        public static AudioManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            PlaySFX(0);
        }
        public void PlaySFX(int sxfNumber)
        {
            soundEffects[sxfNumber].Play();
        }
        public void StopSFX(int sxfNumber)
        {
            soundEffects[sxfNumber].Stop();
        }
    }
}
