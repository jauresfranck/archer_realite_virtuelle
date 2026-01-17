using UnityEngine;

public class LapinSauteur : MonoBehaviour
{
    public float vitesse = 3.0f;        // Vitesse vers l'avant
    public float hauteurSaut = 0.5f;    // À quel point il saute haut
    public float vitesseSaut = 10.0f;   // À quelle vitesse il rebondit

    private float positionY_Base;       // Pour se souvenir du sol

    void Start()
    {
        // On mémorise la hauteur du sol au début
        positionY_Base = transform.position.y;
        
        // Petite variation pour qu'ils ne sautent pas tous en même temps
        vitesseSaut += Random.Range(-1.0f, 1.0f);
        vitesse += Random.Range(-0.5f, 0.5f);
    }

    void Update()
    {
        // 1. AVANCER (Comme avant)
        transform.Translate(0, 0, vitesse * Time.deltaTime);

        // 2. SAUTILLER (La magie des Maths)
        // On utilise Sinus (Sin) pour faire une vague, et Abs pour que ça reste positif (rebond)
        float nouveauY = Mathf.Abs(Mathf.Sin(Time.time * vitesseSaut)) * hauteurSaut;
        
        // On applique la nouvelle hauteur
        // On prend la position actuelle, et on change juste le Y
        Vector3 pos = transform.position;
        pos.y = positionY_Base + nouveauY; 
        transform.position = pos;
    }
}