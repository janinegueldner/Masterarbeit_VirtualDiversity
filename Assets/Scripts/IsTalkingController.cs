using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsTalkingController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public bool isTalking;

    void Update()
    {
        if (isTalking) 
        {
            animator.SetBool("isTalking", true);
        } else {
            animator.SetBool("isTalking", false);
        }
    }
}
