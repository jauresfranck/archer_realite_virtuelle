using UnityEngine;

public class EauMouvement : MonoBehaviour
{
    // Réglages pour la direction et la vitesse du courant
    public float vitesseX = 0.0f; 
    public float vitesseY = 0.5f; 

    private Renderer rendu;

    void Start()
    {
        // On récupère le composant qui dessine l'eau
        rendu = GetComponent<Renderer>();
    }

    void Update()
    {
        // On calcule le décalage basé sur le temps qui passe
        float decallageX = Time.time * vitesseX;
        float decallageY = Time.time * vitesseY;

        // On applique ce décalage à la texture (le matériau)
        rendu.material.mainTextureOffset = new Vector2(decallageX, decallageY);
    }
}