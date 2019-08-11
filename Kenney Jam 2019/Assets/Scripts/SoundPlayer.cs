using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    private AudioSource _audioSource;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _audioSource = GetComponent<AudioSource>();

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Play(AudioClip clip)
    {
        if (clip != null)
            Instance._audioSource.PlayOneShot(clip);
    }
}
