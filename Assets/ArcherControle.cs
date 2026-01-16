using UnityEngine;

public class ArcherControle : MonoBehaviour
{
    public float vitesseMarche = 5.0f;
    public float vitesseRotation = 100.0f;
    public float forceSaut = 5.0f;

    private Rigidbody rb;
    private Animator anim; // On ajoute la référence à l'animateur

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); // On récupère l'animateur au démarrage
    }

    void Update()
    {
        // --- DEPLACEMENT ---
        // On récupère la valeur (W/S ou Flèches Haut/Bas)
        float marche = Input.GetAxis("Vertical"); 
        
        // On bouge physiquement
        transform.Translate(0, 0, marche * vitesseMarche * Time.deltaTime);

        // --- ANIMATION ---
        // Si "marche" n'est pas égal à 0, c'est qu'on appuie sur une touche
        if (marche != 0) 
        {
            anim.SetBool("Avancer", true); // Lance l'animation de course
        }
        else 
        {
            anim.SetBool("Avancer", false); // Revient à l'animation de visée
        }

        // --- ROTATION ---
        float rotation = Input.GetAxis("Horizontal") * vitesseRotation * Time.deltaTime;
        transform.Rotate(0, rotation, 0);

        // --- SAUT ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * forceSaut, ForceMode.Impulse);
        }
    }
}