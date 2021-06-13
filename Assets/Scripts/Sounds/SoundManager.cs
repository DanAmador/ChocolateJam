using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    private AudioSource source;

    public static SoundManager GetInstance()
    {
        if (instance == null)
            instance = new SoundManager();
        return instance;
    }
    
    public enum Sounds
    {
        BUTTON = 0,
        POWERUP = 1,
        HEALTHUP = 2,
        SHADOWCOLLISION = 3
    }

    [SerializeField]
    AudioClip button;
    [SerializeField]
    AudioClip powerUp;
    [SerializeField]
    AudioClip healthUp;
    [SerializeField]
    AudioClip shadowCollision;

    [SerializeField]
    private AudioClip menuMusic;
    [SerializeField]
    private AudioClip gameMusic;


    private AudioClip[] clips;

    public void PlaySound(Sounds sound, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clips[(int)sound], position);
    }

    private void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.spatialBlend = 0.0f;

        clips = new AudioClip[] { button, powerUp, healthUp, shadowCollision };
    }

    public void ChangeToMenuMusic()
    {
        StartCoroutine("BlendMusic", menuMusic);
    }

    public void ChangeToGameMusic()
    {
        StartCoroutine("BlendMusic", gameMusic);
        source.time = 0;
        source.Play();
    }

    private IEnumerator BlendMusic(AudioClip clip)
    {
        float volume = source.volume;
        for(; source.volume > 0; source.volume -= 0.05f)
        {
            yield return new WaitForSeconds(0.05f);
        }
        
        source.clip = clip;
        source.time = 0;
        for (; source.volume < volume; source.volume += 0.05f)
        {
            yield return new WaitForSeconds(0.05f);
        }
        source.volume = volume;

        StopCoroutine("BlendMusic");
    }

}
