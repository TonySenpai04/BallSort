using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Sound Clips")]
    public AudioClip clickSound;
    public AudioClip dropSound;
    public AudioClip failSound;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // giữ lại giữa các scene nếu cần
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayClick()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void PlayDrop()
    {
        if (dropSound != null)
            audioSource.PlayOneShot(dropSound);
    }

    public void PlayFail()
    {
        if (failSound != null)
            audioSource.PlayOneShot(failSound);
    }
}
