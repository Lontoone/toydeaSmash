using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public const string ATTACK = "ATTACK";
    public const string HEAVY_PUNCH = "HEAVY_PUNCH";
    public const string EXPLODE = "EXPLODE";
    public const string HURT = "HURT";
    public const string REVIVE = "REVIVE";
    public const string JUMP = "JUMP";
    public const string BUTTONCLIP = "BUTTONCLIP";

    public static SFXManager instance;
    public SoundData data;
    [SerializeField]
    private AudioSource _audioSourcePrefab;

    private const string _AUDIOSOURCE_GC = "_AUDIOSOURCE_GC";
    private AudioSource _audioSource;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        data.MapInit();
    }
    public void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
        GCManager.RegisterObject(_AUDIOSOURCE_GC, _audioSourcePrefab.gameObject);
        Debug.Log("register sfx manager");
    }

    public void PlaySound(string _key)
    {
        _audioSource.PlayOneShot(SoundData.nameClipPairsMap[_key]);
    }
    //Play Sound with a new audioSource
    public void PlaySoundInstance(string _key)
    {
        AudioSource _as = GCManager.Instantiate(_AUDIOSOURCE_GC).GetComponent<AudioSource>();
        _as.PlayOneShot(SoundData.nameClipPairsMap[_key]);
    }

    public static void PlayerAudioClipInstance(AudioClip _audioClip)
    {
        AudioSource _as = GCManager.Instantiate(_AUDIOSOURCE_GC).GetComponent<AudioSource>();
        _as.PlayOneShot(_audioClip);
    }
}
