using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))] // Ajoute automatiquement le LineRenderer si absent
public class ArcherVR_Complet : MonoBehaviour
{
    [Header("Composants")]
    public CharacterController controller;
    public Animator animator;

    [Header("Physique Mouvement")]
    public float speed = 5f;
    public float gravity = -9.81f;
    public float forceSaut = 5.0f;

    [Header("Configuration VR (Inputs)")]
    public Transform cameraTransform;      // GLISSER LA MAIN CAMERA ICI
    public InputActionProperty moveAction; // Joystick Droit (Locomotion)
    public InputActionProperty jumpAction; // Bouton A
    public InputActionProperty rollAction; // Bouton B
    public InputActionProperty fireAction; // Gâchette Index (Tir)

    [Header("Rotation Tête (Anti-Tournis)")]
    public float rotationSmoothing = 5f;
    public float rotationThreshold = 20f;

    [Header("Combat & Visée")]
    public GameObject flechePrefab;
    public Transform pointDeTir;         // L'objet vide devant la main/arc
    public float vitesseFleche = 15.0f;  // Vitesse ajustée pour mieux voir la flèche
    
    [Header("Réglages Trajectoire")]
    public int resolutionTrajectoire = 30; // Nombre de points de la ligne
    public float tempsVision = 2.0f;       // Portée du viseur
    private LineRenderer lineRenderer;

    private Vector3 velocity; // Vitesse verticale (Chute/Saut)

    void Start()
    {
        // Initialisation propre du Line Renderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false; // On le cache au début
        lineRenderer.positionCount = resolutionTrajectoire;
        lineRenderer.useWorldSpace = true; // Important pour la physique
    }

    void OnEnable()
    {
        // Activation sécurisée des inputs
        if (moveAction.action != null) moveAction.action.Enable();
        if (jumpAction.action != null) jumpAction.action.Enable();
        if (rollAction.action != null) rollAction.action.Enable();
        if (fireAction.action != null) fireAction.action.Enable();
    }

    void Update()
    {
        // Sécurité : On réactive le joystick s'il saute
        if (moveAction.action != null && !moveAction.action.enabled) moveAction.action.Enable();

        // =========================================================
        // 1. GESTION DU TIR (Maintenir pour Viser -> Relâcher pour Tirer)
        // =========================================================
        if (fireAction.action != null)
        {
            // TANT QU'ON APPUIE : On affiche la courbe
            if (fireAction.action.IsPressed())
            {
                lineRenderer.enabled = true;
                CalculerTrajectoire();
            }
            // AU MOMENT OU ON LÂCHE : On tire
            else if (fireAction.action.WasReleasedThisFrame())
            {
                lineRenderer.enabled = false; // On cache la ligne
                animator.SetTrigger("Tir");
                TirerFleche();
            }
            // SINON : On s'assure que c'est caché
            else
            {
                lineRenderer.enabled = false;
            }
        }

        // =========================================================
        // 2. ROTATION DU CORPS (Suit la tête)
        // =========================================================
        if (cameraTransform != null)
        {
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, cameraTransform.eulerAngles.y));
            
            // Si la tête tourne trop par rapport au corps, le corps suit doucement
            if (angleDifference > rotationThreshold)
            {
                Vector3 direction = cameraTransform.forward;
                direction.y = 0; // On garde le corps à plat
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
                }
            }
        }

        // =========================================================
        // 3. DEPLACEMENT (Joystick Droit)
        // =========================================================
        Vector2 moveInput = Vector2.zero;
        if (moveAction.action != null) moveInput = moveAction.action.ReadValue<Vector2>();

        Vector3 moveDirection = Vector3.zero;
        
        // Zone morte de 0.1 pour éviter que ça bouge tout seul
        if (moveInput.magnitude > 0.1f)
        {
            // On se déplace par rapport à l'orientation du joueur
            moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            moveDirection *= speed;
            animator.SetBool("Avance", true);
        }
        else
        {
            animator.SetBool("Avance", false);
        }

        // =========================================================
        // 4. GRAVITÉ ET SAUT
        // =========================================================
        if (controller.isGrounded && velocity.y < 0) 
            velocity.y = -2f; // Plaque au sol

        if (jumpAction.action != null && jumpAction.action.WasPressedThisFrame() && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(forceSaut * -2f * gravity);
            animator.SetTrigger("Saut");
        }

        velocity.y += gravity * Time.deltaTime;

        // =========================================================
        // 5. APPLICATION PHYSIQUE (Un seul Move pour éviter les vibrations)
        // =========================================================
        Vector3 finalMovement = (moveDirection + velocity) * Time.deltaTime;
        controller.Move(finalMovement);

        // ROULADE
        if (rollAction.action != null && rollAction.action.WasPressedThisFrame()) 
            animator.SetTrigger("Roulade");
    }

    // --- CALCUL DE LA COURBE DE VISÉE ---
    void CalculerTrajectoire()
    {
        if (pointDeTir == null) return;

        Vector3 depart = pointDeTir.position;
        // On utilise le "Forward" du point de tir (Flèche Bleue)
        Vector3 vitesseInitiale = pointDeTir.forward * vitesseFleche; 

        for (int i = 0; i < resolutionTrajectoire; i++)
        {
            float tempsSimule = (float)i / (float)resolutionTrajectoire * tempsVision;
            
            // Formule physique : P = P0 + Vt + 0.5gt²
            Vector3 point = depart + (vitesseInitiale * tempsSimule) + (Physics.gravity * 0.5f * tempsSimule * tempsSimule);
            
            lineRenderer.SetPosition(i, point);
        }
    }

    // --- CRÉATION DE LA FLÈCHE ---
    void TirerFleche()
    {
        if (flechePrefab && pointDeTir)
        {
            // CORRECTION : On utilise la rotation du PointDeTir, pas celle du joueur
            GameObject maFleche = Instantiate(flechePrefab, pointDeTir.position, pointDeTir.rotation);
            
            Rigidbody rb = maFleche.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.velocity = Vector3.zero; // Reset de sécurité
                // On propulse dans la direction de la flèche bleue du PointDeTir
                rb.velocity = pointDeTir.forward * vitesseFleche;
            }
            
            Destroy(maFleche, 5.0f); // Nettoyage après 5 secondes
        }
    }
}