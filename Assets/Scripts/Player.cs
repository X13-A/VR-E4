using UnityEngine;

public class Player : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'objet avec lequel il y a collision est dans la couche "Enemy"
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            GameOver();
        }
    }

    void GameOver()
    {
        // Affiche "Game Over" dans la console
        Debug.Log("Game Over");
        // Vous pouvez ajouter ici d'autres actions pour le Game Over, comme afficher un écran de Game Over
        // ou arrêter le jeu
    }
}