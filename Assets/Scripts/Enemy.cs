using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemySpawn enemySpawn;
    private float angle;
    private Rigidbody rb;
    [SerializeField] float speed = 2f;  // Vitesse de déplacement de l'ennemi

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(EnemySpawn spawnScript, float spawnAngle)
    {
        enemySpawn = spawnScript;
        angle = spawnAngle;
    }

    void Update()
    {
        // Déplacer l'ennemi vers le joueur
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    public void Die()
    {
        // Logic for when the enemy dies
        // This could include playing a death animation, dropping loot, etc.

        // Re-add the angle to the available angles list in the EnemySpawn script
        if (enemySpawn != null)
        {
            enemySpawn.ReaddAngle(angle);
        }

        // Destroy the enemy game object
        Destroy(gameObject);
    }

    public void OnDisable()
    {
        Die();
    }
}
