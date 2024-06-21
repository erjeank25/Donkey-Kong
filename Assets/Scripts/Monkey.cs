using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkey : MonoBehaviour
{
    private Animator animator;
    private bool bandera = true;
    private BarrelSpawn barrelSpawn; // Referencia al BarrelSpawn

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Asumiendo que el BarrelSpawn está en el mismo GameObject o en otro conocido
        barrelSpawn = FindObjectOfType<BarrelSpawn>(); // Encuentra el BarrelSpawn en la escena
        Invoke("CambiarMono", 3f);
    }

    void CambiarMono()
    {
        if (bandera)
        {
            animator.SetBool("Monkey_right", false); 
            animator.SetBool("Monkey_left", true); 
        }
        else
        {
            animator.SetBool("Monkey_left", false);    
            animator.SetBool("Monkey_right", true);
            
            // Generar un barril cuando el mono mire a la derecha
            if (barrelSpawn != null)
            {
                Invoke("GenerarBarril", 0.5f); // Ajusta el retraso según sea necesario
            }
        }

        bandera = !bandera;

        Invoke("CambiarMono", 3f);
    }

    void GenerarBarril()
    {
        if (barrelSpawn != null)
        {
            barrelSpawn.Spawn();
        }
    }
}