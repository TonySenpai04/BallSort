using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Sound Clips")]
    public AudioClip clickSound;
    public AudioClip dropSound;
    public AudioClip failSound;
    public AudioClip clickButtonSound;
    public AudioClip winSound;

    private AudioSource audioSource;

    public List<Button> buttons = new List<Button>();

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
        foreach (Button button in buttons)
        {
            AddButtonSound(button);
        }

    }
    public void AddButtonSound(Button button)
    {
        if (button != null )
        {
            button.onClick.AddListener(PlayButtonClick);
            
        }
    }
    public void PlayClick()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
    public void PlayButtonClick()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickButtonSound);
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
     public void PlayWin()
    {
        if (winSound != null)
            audioSource.PlayOneShot(winSound);
    }
}
