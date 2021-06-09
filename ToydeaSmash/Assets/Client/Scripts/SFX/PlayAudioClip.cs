using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioClip : MonoBehaviour
{
    public AudioClip clip;
    public void Play()
    {
        SFXManager.PlayerAudioClipInstance(clip);
    }
}
