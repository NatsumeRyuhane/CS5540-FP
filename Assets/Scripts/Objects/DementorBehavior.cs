using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class DementorBehavior : MonoBehaviour {
    [SerializeField] private float slowDownDuration = 5f;
    private AudioSource _audioSource;

    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource?.Play();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            StartCoroutine(SlowDownMusicToHalt(_audioSource));
            LevelManager.Instance.LevelFail();
        }
    }
    
    private IEnumerator SlowDownMusicToHalt(AudioSource musicSource)
    {
        float startPitch = musicSource.pitch;
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < slowDownDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slowDownDuration;

            // Gradually reduce pitch to slow down music
            musicSource.pitch = Mathf.Lerp(startPitch, 0.1f, t);
            
            // Optionally fade out volume as well
            musicSource.volume = Mathf.Lerp(startVolume, 0.0f, t);

            yield return null;
        }

        musicSource.Stop();
    }
}