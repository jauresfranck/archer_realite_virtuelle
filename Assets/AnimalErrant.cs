using UnityEngine;

public class AnimalErrant : MonoBehaviour
{
    public float vitesse = 2.0f;          // Vitesse de marche
    public float intervalleChangement = 4.0f; // Change de direction toutes les X secondes

    private float timer;

    void Start()
    {
        // Petite astuce : Chaque animal démarre avec un timer décalé
        // pour qu'ils ne tournent pas tous pile en même temps.
        timer = Random.Range(0f, intervalleChangement);
    }

    void Update()
    {
        // 1. AVANCER (Toujours tout droit)
        transform.Translate(Vector3.forward * vitesse * Time.deltaTime);

        // 2. GESTION DU TEMPS (Est-ce qu'on tourne ?)
        timer += Time.deltaTime;

        if (timer > intervalleChangement)
        {
            TournerAleatoirement();
            timer = 0; // On remet le chronomètre à zéro
        }
    }

    void TournerAleatoirement()
    {
        // Choisit un angle entre -90 (gauche) et 90 (droite) degrés
        float angleRotation = Random.Range(-90f, 90f);
        transform.Rotate(0, angleRotation, 0);

        // On change un peu la vitesse pour plus de réalisme
        // Parfois il ralentit, parfois il accélère un peu
        intervalleChangement = Random.Range(2.0f, 6.0f);
    }
}