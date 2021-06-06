using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Animtion_Playsound_Function : MonoBehaviour
{


    public AudioSource audioSource;


    void Update()
    {
        audioSource.Play();
    }
}