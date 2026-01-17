using UnityEngine;

public class ArcherControle : MonoBehaviour
{
    public float vitesseMarche = 5.0f;
    public float vitesseRotation = 100.0f;
    public float forceSaut = 5.0f;

    private Rigidbody rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // --- DEPLACEMENT ---
        float marche = Input.GetAxis("Vertical");
        transform.Translate(0, 0, marche * vitesseMarche * Time.deltaTime);

        // --- ANIMATION MARCHE ---
        if (marche != 0)
        {
            anim.SetBool("Avance", true);
        }
        else
        {
            anim.SetBool("Avance", false);
        }

        // --- ROTATION ---
        float rotation = Input.GetAxis("Horizontal") * vitesseRotation * Time.deltaTime;
        transform.Rotate(0, rotation, 0);

        // --- SAUT (Partie modifiée) ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 1. On applique la force physique (comme avant)
            rb.AddForce(Vector3.up * forceSaut, ForceMode.Impulse);

            // 2. NOUVELLE LIGNE : On déclenche l'animation !
            // On utilise "SetTrigger" car c'est une action unique
            anim.SetTrigger("Saut");
        }
    }
}