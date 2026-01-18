using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))] // Ajoute auto le LineRenderer
public class ArcherVR_Complet : MonoBehaviour
{
    [Header("Composants")]
    public CharacterController controller;
    public Animator animator;

    [Header("Physique Mouvement")]
    public float speed = 5f;
    public float gravity = -9.81f;
    public float forceSaut = 5.0f;

    [Header("Configuration VR")]
    public Transform cameraTransform;
    public InputActionProperty moveAction; // Joystick Droit
    public InputActionProperty jumpAction; // Bouton A
    public InputActionProperty rollAction; // Bouton B
    public InputActionProperty fireAction; // Gâchette

    [Header("Rotation Tête")]
    public float rotationSmoothing = 5f;
    public float rotationThreshold = 20f;

    [Header("Combat & Visée")]
    public GameObject flechePrefab;
    public Transform pointDeTir;
    public float vitesseFleche = 20.0f;
    
    // NOUVEAU : Réglages de la ligne de visée
    public int resolutionTrajectoire = 30; // Nombre de points de la ligne
    public float tempsVision = 2.0f; // Jusqu'où on voit (en secondes de vol)
    private LineRenderer lineRenderer;

    private Vector3 velocity; // Vitesse verticale du joueur

    void Start()
    {
        // On récupère le LineRenderer et on le configure
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false; // Caché par défaut
        lineRenderer.positionCount = resolutionTrajectoire;
    }

    void OnEnable()
    {
        if (moveAction.action != null) moveAction.action.Enable();
        if (jumpAction.action != null) jumpAction.action.Enable();
        if (rollAction.action != null) rollAction.action.Enable();
        if (fireAction.action != null) fireAction.action.Enable();
    }

    void Update()
    {
        // Sécurité Input
        if (moveAction.action != null && !moveAction.action.enabled) moveAction.action.Enable();

        // ---------------------------------------------------------
        // 1. GESTION DE LA VISÉE ET DU TIR (NOUVEAU)
        // ---------------------------------------------------------
        if (fireAction.action != null)
        {
            // TANT QU'ON APPUIE : On montre la trajectoire
            if (fireAction.action.IsPressed())
            {
                lineRenderer.enabled = true;
                MontrerTrajectoire();
            }
            // QUAND ON RELACHE : On tire
            else if (fireAction.action.WasReleasedThisFrame())
            {
                lineRenderer.enabled = false;
                animator.SetTrigger("Tir");
                CreateArrow();
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }

        // ---------------------------------------------------------
        // 2. ROTATION (Tête)
        // ---------------------------------------------------------
        if (cameraTransform != null)
        {
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, cameraTransform.eulerAngles.y));
            if (angleDifference > rotationThreshold)
            {
                Vector3 direction = cameraTransform.forward;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
                }
            }
        }

        // ---------------------------------------------------------
        // 3. MOUVEMENT (Joystick Droit)
        // ---------------------------------------------------------
        Vector2 moveInput = Vector2.zero;
        if (moveAction.action != null) moveInput = moveAction.action.ReadValue<Vector2>();

        Vector3 moveDirection = Vector3.zero;
        if (moveInput.magnitude > 0.1f)
        {
            moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            moveDirection *= speed;
            animator.SetBool("Avance", true);
        }
        else
        {
            animator.SetBool("Avance", false);
        }

        // ---------------------------------------------------------
        // 4. GRAVITÉ & SAUT
        // ---------------------------------------------------------
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;

        if (jumpAction.action != null && jumpAction.action.WasPressedThisFrame() && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(forceSaut * -2f * gravity);
            animator.SetTrigger("Saut");
        }

        velocity.y += gravity * Time.deltaTime;

        // ---------------------------------------------------------
        // 5. APPLICATION PHYSIQUE
        // ---------------------------------------------------------
        Vector3 finalMovement = (moveDirection + velocity) * Time.deltaTime;
        controller.Move(finalMovement);

        if (rollAction.action != null && rollAction.action.WasPressedThisFrame()) 
            animator.SetTrigger("Roulade");
    }

    // --- FONCTION MATHEMATIQUE POUR DESSINER LA COURBE ---
    void MontrerTrajectoire()
    {
        if (pointDeTir == null) return;

        Vector3 depart = pointDeTir.position;
        Vector3 vitesseInitiale = pointDeTir.forward * vitesseFleche; // La force du tir

        // On calcule les points de la courbe physique
        for (int i = 0; i < resolutionTrajectoire; i++)
        {
            float tempsSimule = (float)i / (float)resolutionTrajectoire * tempsVision;
            
            // Formule physique : Position = Départ + (Vitesse * temps) + (0.5 * Gravité * temps^2)
            Vector3 point = depart + (vitesseInitiale * tempsSimule) + (Physics.gravity * 0.5f * tempsSimule * tempsSimule);
            
            lineRenderer.SetPosition(i, point);
        }
    }

    void CreateArrow()
    {
        if (flechePrefab && pointDeTir)
        {
            GameObject maFleche = Instantiate(flechePrefab, pointDeTir.position, transform.rotation);
            Rigidbody rb = maFleche.GetComponent<Rigidbody>();
            if (rb) rb.velocity = transform.forward * vitesseFleche;
            Destroy(maFleche, 5.0f);
        }
    }
}