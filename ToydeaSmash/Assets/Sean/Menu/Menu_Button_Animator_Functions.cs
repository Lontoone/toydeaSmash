using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Button_Animator_Functions : MonoBehaviour
{
    [SerializeField] Menu_Button_Controller menu_Button_Controller;
    public bool disableOnce;

    void PlaySound(AudioClip whichSound)
    {
        if (!disableOnce)
        {
            menu_Button_Controller.audioSource.PlayOneShot(whichSound);
        }
        else
        {
            disableOnce = false;
        }
    }
}

