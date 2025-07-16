using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource sfxSource;
    public AudioClip raceStart;
    public AudioClip raceGo;
    public AudioClip sixty;
    public AudioClip shiftSound;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // Optional shortcuts
    public void PlayRaceGo() => PlaySound(raceGo);
}
