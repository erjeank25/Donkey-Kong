using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class velBarrel : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    public float speed = 1f;
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collison)
    {
        if (collison.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rigidbody.AddForce(collison.transform.right * speed,ForceMode2D.Impulse);
        }
    }
}
