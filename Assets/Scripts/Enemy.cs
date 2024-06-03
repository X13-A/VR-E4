using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private EnemySpawn m_Spawn;
    private float m_Angle;
    private int m_Life;
    private bool canTouch;
    private bool canAttack;
    [SerializeField] float m_AttackDistance = 2f;
    [SerializeField] float m_Speed = 2f;
    [SerializeField] float m_FastSpeed = 6f;
    private Animator m_Animator;
    private CapsuleCollider m_StandCollider;
    private BoxCollider m_DeathCollider;
    private Coroutine hitCoroutine;


    void SetSpeed(float speed)
    {
        m_Speed = speed;
        m_Animator.SetFloat("Speed", speed);
    }

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_DeathCollider = GetComponent<BoxCollider>();
        m_StandCollider = GetComponent<CapsuleCollider>();
    }

    public void Initialize(EnemySpawn spawn, float angle, bool fast, int life)
    {
        if (!fast)
        {
            SetSpeed(m_Speed);
        }
        else
        {
            SetSpeed(m_FastSpeed);
        }

        m_Spawn = spawn;
        m_Angle = angle;
        canTouch = true;
        canAttack = true;
        m_Life = life;

    }

    void FixedUpdate()
    {

        if (canAttack & DistanceXZ(m_Spawn.transform.position, transform.position) <= m_AttackDistance)
        {
            Attack();
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && canTouch)
        {
            Touch(1);
        }
    }

    void Attack()
    {
        m_Animator.SetTrigger("isAttacking");
        canAttack = false;
        //canTouch = false;
        StartCoroutine(WaitAttack());
    }

    private IEnumerator WaitAttack()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Game Over !");
    }

    private IEnumerator WaitHit()
    {
        yield return new WaitForSeconds(2f);
        m_Animator.SetBool("isHit1", false);
        m_Animator.SetBool("isHit2", false);
        Debug.Log("Can Touch !");
        hitCoroutine = null;
        FixRotation();
    }

    private IEnumerator WaitDie()
    {
        yield return new WaitForSeconds(2.5f);
        Debug.Log("Enemy Dead !");
        Destroy(gameObject);
    }

    public static float DistanceXZ(Vector3 a, Vector3 b)
    {
        Vector3 delta = new Vector3(a.x - b.x, 0, a.z - b.z);
        return delta.magnitude;
    }

    public void Die()
    {
        canTouch = false;
        if (m_Spawn != null)
        {
            m_Spawn.ReadAngle(m_Angle);
        }

        m_Animator.SetTrigger("isDying");
        m_StandCollider.enabled = false;
        m_DeathCollider.enabled = true;
        StartCoroutine(WaitDie());
    }

    void Hit()
    {
        //m_Animator.SetBool("isHit", false);
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);

            if(m_Animator.GetBool("isHit1"))
            {
                m_Animator.SetBool("isHit2", true);
                m_Animator.SetBool("isHit1", false);
                Debug.Log("Hit 2");
            }
            else
            {
                m_Animator.SetBool("isHit2", false);
                m_Animator.SetBool("isHit1", true);
                Debug.Log("Hit 1");
            }
        }
        else
        {
            m_Animator.SetBool("isHit1", true);
        }

        hitCoroutine = StartCoroutine(WaitHit());
    }

    public void FixRotation()
    {
        Vector3 direction = m_Spawn.transform.position - transform.position;
        direction.y = 0;  // Garder la direction dans le plan XZ
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void Touch(int damage)
    {
        m_Life -= damage;
        if(m_Life <= 0)
        {
            Die();
        }
        else
        {
            Hit();
        }
    }
}
