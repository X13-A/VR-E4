using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private EnemySpawn m_Spawn;
    private float m_Angle;
    [SerializeField] private int m_Life = 1;
    private bool canTouch;
    [SerializeField] float m_AttackDistance = 2f;
    [SerializeField] float m_Speed = 2f;
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

    public void Initialize(EnemySpawn spawn, float angle)
    {
        m_Spawn = spawn;
        m_Angle = angle;
        canTouch = true;
        SetSpeed(m_Speed);

    }

    void FixedUpdate()
    {

        if (DistanceXZ(m_Spawn.transform.position, transform.position) <= m_AttackDistance)
        {
            if(!m_Animator.GetBool("isAttacking"))
                Attack();
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && canTouch)
        {
            Touch();
        }
    }

    void Attack()
    {
        m_Animator.SetBool("isAttacking", true);
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
            m_Spawn.ReaddAngle(m_Angle);
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

    public void Touch()
    {
        m_Life -= 1;
        if(m_Life == 0)
        {
            Die();
        }
        else
        {
            Hit();
        }
    }
}
