using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelSpawn : MonoBehaviour
{
    public GameObject prefab; // Referencia del barril

    public void Spawn() // Genera un barril
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}