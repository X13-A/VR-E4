using UnityEngine;

public class Player : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        // V�rifie si l'objet avec lequel il y a collision est dans la couche "Enemy"
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            GameOver();
        }
    }

    void GameOver()
    {
        // Affiche "Game Over" dans la console
        Debug.Log("Game Over");
        // Vous pouvez ajouter ici d'autres actions pour le Game Over, comme afficher un �cran de Game Over
        // ou arr�ter le jeu
    }
}