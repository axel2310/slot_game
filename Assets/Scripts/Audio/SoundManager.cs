using UnityEngine;

/// <summary>
/// Central audio manager for the slot game.
/// Plays retro 16-bit sound effects for lever pull, reel spins, stops, and wins.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSource;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip leverPullClip;
    [SerializeField] private AudioClip reelSpinLoopClip;
    [SerializeField] private AudioClip reelStopClip;
    [SerializeField] private AudioClip winJingleClip;
    [SerializeField] private AudioClip jackpotClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Setup audio sources if not assigned
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        if (loopSource == null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.playOnAwake = false;
            loopSource.loop = true;
        }
    }

    public void PlayLeverPull()
    {
        PlayOneShot(leverPullClip, 0.9f);
    }

    public void StartReelSpin()
    {
        if (reelSpinLoopClip != null && loopSource != null)
        {
            loopSource.clip = reelSpinLoopClip;
            loopSource.volume = 0.5f;
            loopSource.Play();
        }
    }

    public void StopReelSpin()
    {
        if (loopSource != null && loopSource.isPlaying)
        {
            loopSource.Stop();
        }
    }

    public void PlayReelStop()
    {
        PlayOneShot(reelStopClip, 0.8f);
    }

    public void PlayWin(bool isJackpot)
    {
        if (isJackpot && jackpotClip != null)
        {
            PlayOneShot(jackpotClip, 1.0f);
        }
        else if (winJingleClip != null)
        {
            PlayOneShot(winJingleClip, 0.9f);
        }
    }

    private void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
}
