using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpman : MonoBehaviour
{
    
    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex;
    private Animator animator;

    private new Rigidbody2D rigidbody;
    private new Collider2D collider;

    private Vector2 direccion;
    private Collider2D[] resultados;

    public float velMovimiento = 1f;
    public float fuerzaSalto = 1f;

    private bool grounded;
    private bool climbing;
    private bool hasHammer;
    private float hammerTime;

    private void Awake() // Instanciamos las referencias a los componentes
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        resultados = new Collider2D[4];
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f / 12, 1f / 12);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void CheckCollision() // Verifica con qué objeto está colisionando Jumpman
    {
        grounded = false;
        climbing = false;

        Vector2 size = collider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;
        int objetosSuperpuestos = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, resultados);

        for (int i = 0; i < objetosSuperpuestos; i++)
        {
            GameObject hit = resultados[i].gameObject;

            if (hit.layer == LayerMask.NameToLayer("Ground")) //si colisiona con el suelo
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.5f); // Verifica si toca alguna plataforma
                Physics2D.IgnoreCollision(collider, resultados[i], !grounded);
                animator.SetBool("Climb", false);
                animator.SetBool("Jump", false);
            }
            else if (hit.layer == LayerMask.NameToLayer("Ladder") && !hasHammer)
            {
                climbing = true;
            }
        }
    }

    // Mueve a Mario
    void Update()
    {
        CheckCollision(); //chequear con que objeto colisiona mario
        animator.SetBool("Dead", false); //en caso de reiniciarse el nivell por una muerte, se desactiva la animacion

        // Actualizar el temporizador del martillo
        if (hasHammer)
        {
            animator.SetBool("Attack", true); // Activa la animación del martillo
            hammerTime -= Time.deltaTime; //diminuye el tiempo de accion del martillo
            if (hammerTime <= 0)
            {
                hasHammer = false;
                animator.SetBool("Attack", false);
            }
        }

        // TREPAR
        if (climbing) //si colisiona con una escalera
        {
            direccion.x = 0f; // No permitir movimiento horizontal mientras trepa
            animator.SetBool("Walk", false); // Desactivar animación de caminar
            animator.SetBool("Climb", true);
            float verticalInput = Input.GetAxis("Vertical") * velMovimiento;
            if (verticalInput != 0)
            {
                direccion.y = verticalInput;
            }
            else
            {
                direccion.y = 0; // Mantener la posición actual si no se presiona ninguna tecla de movimiento vertical
            }
            //direccion.y = Input.GetAxis("Vertical") * velMovimiento;
        }

        // SALTO
        else if (grounded && Input.GetButtonDown("Jump") && !hasHammer) 
        //si tocca el suelo, se toca la barra espaciadora y no tiene un martillo
        {
            animator.SetBool("Jump", true);
            direccion = Vector2.up * fuerzaSalto;
        }
        else //caida con gravedad
        {
            direccion += Physics2D.gravity * Time.deltaTime;
        }

        // DETECTAR UN MOVIMIENTO (QUIETO)
        direccion.x = Input.GetAxis("Horizontal") * velMovimiento;
        if (grounded) //si mario colisiona con el suelo
        {
            direccion.y = Mathf.Max(direccion.y, -1f);
        }

        // Cambiar la cara del personaje y CAMINAR
        if (direccion.x > 0f) //si se mueve a la derecha
        {
            transform.eulerAngles = Vector3.zero;
            if (hasHammer)
            {
                animator.SetBool("Attack", true); //ataque a la derecha
            }
            animator.SetBool("Walk", true); //caminar a la derecha
        }
        else if (direccion.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
            if (hasHammer)
            {
                animator.SetBool("Attack", true); //ataque a la izquierda
            }
            animator.SetBool("Walk", true); //caminar a la izquierda
        }
        else
        {
            animator.SetBool("Walk", false); //quedarse quieto
        }
    }

    private void FixedUpdate() //arregla cualquier bug de posicion con el personaje
    {
        rigidbody.MovePosition(rigidbody.position + direccion * Time.fixedDeltaTime);
    }

    private void AnimateSprite() // Animaciones de Mario manuales (tenia que ser con el animator, decidi dejar este codigo aqui)
    {
        if (climbing)
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (direccion.x != 0f)
        {
            spriteIndex++;
            if (spriteIndex >= runSprites.Length)
            {
                spriteIndex = 0;
            }
            spriteRenderer.sprite = runSprites[spriteIndex];
        }
    }

    // Se compara a qué objeto está tocando Mario
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objective")) //Si se gana el juego
        {
            enabled = false; //se paraliza mario
            Time.timeScale = 0f; // Congela el juego
            FindObjectOfType<GameManager>().GameComplete(); //Llama al metodo que hace ganar, se congela la pantalla
        }
        else if (collision.gameObject.CompareTag("Obstacle")) //Si se choca con un barril
        {
            if (hasHammer)
            {
                Destroy(collision.gameObject); // Destruye el obstáculo si Mario tiene el martillo
            }
            else
            {
                JuegoPerdido(); //Perdiste
            }
        }
        else if (collision.gameObject.CompareTag("martillo"))
        {
            animator.SetBool("Attack", true); // Activa la animación del martillo
            Destroy(collision.gameObject); // Destruye el martillo
            hasHammer = true; // Mario ahora tiene el martillo
            hammerTime = 10f; // El martillo dura 10 segundos
        }

    }

    private void JuegoPerdido()
    {
        enabled = false;
        animator.SetBool("Dead", true);

        Invoke(nameof(Perder), 2f); //Se esperan 2 segundos para ver la muerte de mario y se reinicia el nivel
    }

    private void Perder() //Reinicia el nivel si se pierde
    {
        FindObjectOfType<GameManager>().GameFailed();
    }
}