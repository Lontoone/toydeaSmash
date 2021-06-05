using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Button_new : MonoBehaviour
{

    [SerializeField] Menu_Button_Controller menuButtonController;
    [SerializeField] Animator animator;
    [SerializeField] Menu_Button_Animator_Functions animatorFunctions;
    [SerializeField] int thisIndex;

    void Update(){
        if (menuButtonController.index == thisIndex)
        {
            animator.SetBool("selected", true);
            if (Input.GetAxis("Submit") == 1)
            {
                animator.SetBool("pressed", true);
            }
            else if (animator.GetBool("pressed"))
            {
                animator.SetBool("pressed", false);
                animatorFunctions.disableOnce = true;
            }
        }
        else {
            animator.SetBool("selected", false);
        }
    }
}
