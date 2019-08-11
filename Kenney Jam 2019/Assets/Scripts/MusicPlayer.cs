using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    [SerializeField]
    private AudioSource MenuMusicAudioSource = null;

    [SerializeField]
    private AudioSource GameMusicAudioSource = null;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Instance.PlayMusic();

            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Instance.PlayMusic();
    }

    private void PlayMusic()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Menu":
            case "Credits":
            case "Controls":
            case "Settings":
                GameMusicAudioSource.Stop();

                if (!MenuMusicAudioSource.isPlaying)
                    MenuMusicAudioSource.Play();
                break;

            case "Main":
            case "GameOver":
                MenuMusicAudioSource.Stop();

                if (!GameMusicAudioSource.isPlaying)
                    GameMusicAudioSource.Play();
                break;
        }
    }
}
