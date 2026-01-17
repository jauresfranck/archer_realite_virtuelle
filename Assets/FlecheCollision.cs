using UnityEngine;

public class FlecheCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // 1. On vérifie ce qu'on a touché
        // "other" est l'objet qui est entré dans ma zone
        if (other.gameObject.CompareTag("Animal"))
        {
            // 2. Victoire ! On détruit l'animal
            Destroy(other.gameObject);

            // 3. On détruit aussi la flèche (pour ne pas qu'elle tue 10 lapins d'un coup)
            Destroy(this.gameObject);
        }
        
        // Optionnel : Si la flèche touche un arbre ou le sol (Terrain)
        // On détruit juste la flèche pour faire le ménage
        else if (other.gameObject.CompareTag("Terrain") || other.gameObject.name == "Terrain")
        {
            Destroy(this.gameObject);
        }
    }
}