using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameMusicPlayer : MonoBehaviour
{
    [Header("Clip")]
    [SerializeField] private AudioClip gameMusic;

    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float volume = 0.55f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool playOnEnable = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f; // 2D
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            Play();
        }
    }

    public void Play()
    {
        if (gameMusic == null || audioSource == null)
        {
            return;
        }

        if (audioSource.clip != gameMusic)
        {
            audioSource.clip = gameMusic;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void Stop()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void OnDisable()
    {
        Stop();
    }
}
