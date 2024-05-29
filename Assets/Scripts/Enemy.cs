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
    
    void SetSpeed(float speed)
    {
        m_Speed = speed;
        m_Animator.SetFloat("Speed", speed);
    }

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
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
        canTouch = true;
        m_Animator.SetBool("isHit", false);
        Debug.Log("Can Touch !");
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
        if (m_Spawn != null)
        {
            m_Spawn.ReaddAngle(m_Angle);
        }

        m_Animator.SetBool("isDying", true);
        StartCoroutine(WaitDie());
    }

    void Hit()
    {
        m_Animator.SetBool("isHit", true);
        StartCoroutine(WaitHit());
    }

    void Touch()
    {
        m_Life -= 1;
        canTouch = false;
        if(m_Life == 0)
        {
            Die();
        }
        else
        {
            Hit();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            if(canTouch) Touch();
        }
    }
}
