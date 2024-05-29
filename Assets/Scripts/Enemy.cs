using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemySpawn enemySpawn;
    private float angle;
    private Rigidbody rb;
    [SerializeField] float speed = 2f;

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
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    public void Die()
    {
        if (enemySpawn != null)
        {
            enemySpawn.ReaddAngle(angle);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            Die();
        }
    }
}
