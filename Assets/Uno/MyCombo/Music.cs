using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour
{
    public AudioSource audioSource;
    public enum Type { None, MainMusic };
    public static Music instance;

    [HideInInspector]
    public AudioClip[] musicClips;

    private Type currentType = Type.None;

    private void Awake()
    {
        instance = this;
    }

   

  

    public void Play(Music.Type type)
    {
        if (type == Type.None) return;
        if (currentType != type || !audioSource.isPlaying)
        {
            StartCoroutine(PlayNewMusic(type));
        }
    }

    public void Play()
    {
        Play(currentType);
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    private IEnumerator PlayNewMusic(Music.Type type)
    {
        while (audioSource.volume >= 0.1f)
        {
            audioSource.volume -= 0.2f;
            yield return new WaitForSeconds(0.1f);
        }
        audioSource.Stop();
        currentType = type;
        audioSource.clip = musicClips[(int)type];
      
        audioSource.volume = 1;
    }
}
