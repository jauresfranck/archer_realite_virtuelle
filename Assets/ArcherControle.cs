using UnityEngine;

public class ArcherControle : MonoBehaviour
{
    public float vitesseMarche = 5.0f;
    public float vitesseRotation = 100.0f;
    public float forceSaut = 5.0f;

    // --- C'est ici que ça se joue ---
    // Vérifiez bien que ces lignes sont là après le collage
    public GameObject flechePrefab;  
    public Transform pointDeTir;     
    public float vitesseFleche = 20.0f; 

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

        // --- SAUT ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * forceSaut, ForceMode.Impulse);
            anim.SetTrigger("Saut");
        }

        // --- ROULADE ---
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetTrigger("Roulade");
        }

        // --- TIR ---
        if (Input.GetMouseButtonDown(0)) 
        {
            anim.SetTrigger("Tir");
            CreateArrow();
        }
    }

    void CreateArrow()
    {
        // Sécurité : On ne tire que si les liens sont faits
        if (flechePrefab != null && pointDeTir != null)
        {
            GameObject maFleche = Instantiate(flechePrefab, pointDeTir.position, transform.rotation);
            Rigidbody rbFleche = maFleche.GetComponent<Rigidbody>();
            
            if (rbFleche != null)
            {
                rbFleche.velocity = transform.forward * vitesseFleche;
            }
            
            Destroy(maFleche, 3.0f);
        }
        else
        {
            Debug.LogError("Erreur : Il manque la flèche ou le point de tir dans l'Inspecteur !");
        }
    }
}