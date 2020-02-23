
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SoundType
{
    None,
    Drink,
    OpenHatch,
    PickUpKey,
    PickUpWand
}

public class SoundManager : MonoBehaviour
{

    public static SoundManager main;


    [SerializeField]
    private List<GameSound> sounds = new List<GameSound>();
    private bool sfxMuted = false;


    void Awake()
    {
        main = this;
    }

    private AudioSource GetGameSound(GameSound gameSound)
    {
        if (gameSound.sound == null)
        {
            return gameSound.sounds[Random.Range(0, gameSound.sounds.Count - 1)];
        }
        return gameSound.sound;
    }

    public void PlaySound(SoundType soundType)
    {
        if (!sfxMuted)
        {
            foreach (GameSound gameSound in sounds)
            {
                if (gameSound.soundType == soundType)
                {
                    AudioSource audio = GetGameSound(gameSound);
                    if (audio.isPlaying)
                    {
                        audio.Stop();
                    }
                    audio.Play();
                }
            }
        }
    }

    public void StopSound(SoundType soundType)
    {
        if (!sfxMuted)
        {
            foreach (GameSound gameSound in sounds)
            {
                if (gameSound.soundType == soundType)
                {
                    AudioSource audio = GetGameSound(gameSound);
                    if (audio.isPlaying)
                    {
                        audio.Stop();
                    }
                }
            }
        }
    }

    public void ToggleSfx()
    {
        sfxMuted = !sfxMuted;
    }

}

[System.Serializable]
public class GameSound : System.Object
{
    public SoundType soundType;
    //public Action actionType;
    public AudioSource sound;
    public List<AudioSource> sounds;
}
