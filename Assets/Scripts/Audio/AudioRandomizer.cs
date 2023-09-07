using UnityEngine;

namespace RPG.Audio
{
    public class AudioRandomizer : MonoBehaviour
    {
        [SerializeField]
        AudioClip[] clips;
        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Play()
        {
            if (audioSource.isPlaying)
                return;
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
