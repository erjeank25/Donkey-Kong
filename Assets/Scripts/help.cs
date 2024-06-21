using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class help : MonoBehaviour
{
    private Animator animator;
    private bool bandera = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Active", true);
        Invoke("Activar", 1.5f);
    }

    void Activar()
    {
        if (bandera)
        {
            animator.SetBool("Active", false); 
        }
        else
        {
            animator.SetBool("Active", true); 
        }
        bandera = !bandera;
        Invoke("Activar", 1.5f);
    }
}
