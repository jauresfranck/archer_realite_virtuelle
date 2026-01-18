using UnityEngine;
using UnityEngine.InputSystem;

public class ArcherVR_Complet : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    public float speed = 5f;
    public float gravity = -9.81f;
    public float forceSaut = 5.0f;

    [Header("Configuration VR (Actions)")]
    // Joystick Droit pour marcher
    public InputActionProperty moveAction; 
    // Joystick Gauche pour tourner
    public InputActionProperty turnAction; 
    // Boutons et Gâchettes
    public InputActionProperty jumpAction;   // Bouton A (South)
    public InputActionProperty rollAction;   // Bouton B (East)
    public InputActionProperty fireAction;   // Gâchette Droite (Activate)

    [Header("Combat")]
    public GameObject flechePrefab;
    public Transform pointDeTir;
    public float vitesseFleche = 20.0f;

    private Vector3 velocity;

    void Update()
    {
        // --- 1. ROTATION (Joystick GAUCHE) ---
        // On fait tourner l'archer et la caméra enfant suit
        Vector2 turnInput = turnAction.action.ReadValue<Vector2>();
        transform.Rotate(0, turnInput.x * 100f * Time.deltaTime, 0);

        // --- 2. MOUVEMENT (Joystick DROIT) ---
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        // --- 3. GRAVITÉ ---
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- 4. ANIMATION MARCHE ---
        bool isMoving = moveInput.magnitude > 0.1f;
        animator.SetBool("Avance", isMoving);

        // --- 5. SAUT (Bouton A / Espace) ---
        if (jumpAction.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.Space))
        {
            if (controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(forceSaut * -2f * gravity);
                animator.SetTrigger("Saut");
            }
        }

        // --- 6. ROULADE (Bouton B / Shift) ---
        if (rollAction.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetTrigger("Roulade");
        }

        // --- 7. TIR (Gâchette Droite / Clic Gauche) ---
        if (fireAction.action.WasPressedThisFrame() || Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Tir");
            CreateArrow();
        }
    }

    void CreateArrow()
    {
        if (flechePrefab && pointDeTir)
        {
            GameObject maFleche = Instantiate(flechePrefab, pointDeTir.position, transform.rotation);
            Rigidbody rb = maFleche.GetComponent<Rigidbody>();
            if (rb) rb.velocity = transform.forward * vitesseFleche;
            Destroy(maFleche, 3.0f);
        }
    }
}