using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemySpawn enemySpawn;
    private float angle;
    private Rigidbody rb;
    private bool canMove;
    private bool canDie;
    [SerializeField] float attackDistance = 2f;
    [SerializeField] float speed = 2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(EnemySpawn spawnScript, float spawnAngle)
    {
        enemySpawn = spawnScript;
        angle = spawnAngle;
        canDie = true;
        canMove = true;
    }

    void Update()
    {
        if (canMove)
        {
            if (DistanceXZ(enemySpawn.transform.position, transform.position) <= attackDistance)
            {
                Attack();
            }
            else
            {
                rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
            }
        }

    }

    void Attack()
    {
        canMove = false;
        //canDie = false;
    }

    public static float DistanceXZ(Vector3 a, Vector3 b)
    {
        Vector3 delta = new Vector3(a.x - b.x, 0, a.z - b.z);
        return delta.magnitude;
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
            if(canDie) Die();
        }
    }
}
