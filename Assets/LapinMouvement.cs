using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapinMouvement : MonoBehaviour
{
    // Variables réglables dans l'Inspecteur
    public float vitesse = 1.5f;          // Vitesse de marche
    public float tempsAvantRotation = 4.0f; // Change de direction toutes les 4 secondes
    
    private float chronometre;

    // Start est appelé au début du jeu
    void Start()
    {
        // On initialise le chronomètre
        chronometre = tempsAvantRotation;
    }

    // Update est appelé à chaque image (environ 60 fois par seconde)
    void Update()
    {
        // 1. FAIRE AVANCER LE LAPIN
        // Vector3.forward = Devant lui (axe Z bleu)
        // Time.deltaTime = Rend le mouvement fluide quel que soit l'ordinateur
        transform.Translate(Vector3.forward * vitesse * Time.deltaTime);

        // 2. COMPTER LE TEMPS
        chronometre -= Time.deltaTime; // On diminue le temps

        // 3. CHANGER DE DIRECTION QUAND LE TEMPS EST ÉCOULÉ
        if (chronometre <= 0)
        {
            // On choisit un angle au hasard entre 90 et 270 degrés (pour faire demi-tour ou tourner)
            float angleAleatoire = Random.Range(90f, 270f);
            
            // On applique la rotation sur l'axe Y (l'axe vertical, pour tourner sur le sol)
            transform.Rotate(0, angleAleatoire, 0);

            // On remet le chronomètre à zéro pour la prochaine fois
            chronometre = tempsAvantRotation;
        }
    }
}