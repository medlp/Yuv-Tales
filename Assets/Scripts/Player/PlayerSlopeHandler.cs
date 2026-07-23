using UnityEngine;

public class PlayerSlopeHandler : MonoBehaviour
{
    public CharacterController controller;
    public float slopeForce = 8.0f; // Légèrement augmenté pour assurer une glissade fluide

    void Update()
    {
        // On appelle la fonction en continu, SANS vérifier isGrounded ici
        ApplySlopeGravity();
    }

    void ApplySlopeGravity()
    {
        RaycastHit hit;

        // On part du centre exact de la capsule (en prenant en compte son décalage défini dans l'Inspecteur)
        Vector3 center = transform.position + controller.center;

        // La distance de détection : la moitié de la hauteur de la capsule + une petite marge de sécurité
        float castDistance = (controller.height / 2.0f) + 0.5f;

        // On lance une sphère de la taille du joueur vers le bas pour détecter les pentes larges
        if (Physics.SphereCast(center, controller.radius, Vector3.down, out hit, castDistance))
        {
            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);

            // 1. Si la pente est trop raide (même si le jeu pense qu'on vole)
            if (slopeAngle > controller.slopeLimit)
            {
                // On force la glissade vers le bas en suivant la normale de la pente
                Vector3 slideDirection = new Vector3(hit.normal.x, -hit.normal.y, hit.normal.z);
                controller.Move(slideDirection * slopeForce * Time.deltaTime);
            }
            // 2. Si la pente est praticable ET qu'on est bien au sol
            else if (slopeAngle > 0.01f && controller.isGrounded)
            {
                // On plaque le joueur au sol. 
                // Le check isGrounded ici empêche d'attirer le joueur vers le bas s'il est en train de sauter.
                controller.Move(Vector3.down * slopeForce * Time.deltaTime);
            }
        }
    }
}