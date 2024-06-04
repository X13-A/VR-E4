using UnityEngine;
using System.Collections;
using SDD.Events;

public class Enemy : MonoBehaviour, IEventHandler
{
    private EnemySpawn m_Spawn;
    private float m_Angle;
    private int m_Life;
    private int m_MaxLife;
    private bool canTouch;
    private bool canAttack;
    [SerializeField] float m_AttackDistance = 2f;
    [SerializeField] float m_Speed = 2f;
    [SerializeField] float m_FastSpeed = 6f;
    private Animator m_Animator;
    private CapsuleCollider m_StandCollider;
    private BoxCollider m_DeathCollider;
    private Coroutine hitCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine dieCoroutine;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<DestroyAllEnemiesEvent>(DestroyEnemy);
        EventManager.Instance.AddListener<KillAllEnemiesEvent>(Die);
        EventManager.Instance.AddListener<GamePauseEvent>(Pause);
        EventManager.Instance.AddListener<GameResumeEvent>(Resume);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<DestroyAllEnemiesEvent>(DestroyEnemy);
        EventManager.Instance.RemoveListener<KillAllEnemiesEvent>(Die);
        EventManager.Instance.RemoveListener<GamePauseEvent>(Pause);
        EventManager.Instance.RemoveListener<GameResumeEvent>(Resume);
    }

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();

    }

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
        m_MaxLife = life;

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
        attackCoroutine = StartCoroutine(WaitAttack());
    }

    void StandUp()
    {
        FixRotation();
        m_Animator.SetBool("isStandingUp", true);
        StartCoroutine(WaitStandUp());
        m_Life = m_MaxLife;
    }

    private IEnumerator WaitAttack()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Game Over !");
        EventManager.Instance.Raise(new LoseEvent());
        attackCoroutine = null;
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
        //DestroyEnemy();
        dieCoroutine = null;
        StandUp();
        if (m_Spawn != null)
        {
            //m_Spawn.Death(m_Angle);
        }
    }

    private IEnumerator WaitStandUp()
    {
        yield return new WaitForSeconds(3.3f);
        m_StandCollider.enabled = true;
        m_DeathCollider.enabled = false;
        m_Animator.SetBool("isStandingUp", false);
        canTouch = true;
    }

    public static float DistanceXZ(Vector3 a, Vector3 b)
    {
        Vector3 delta = new Vector3(a.x - b.x, 0, a.z - b.z);
        return delta.magnitude;
    }

    public void Die()
    {
        canTouch = false;
        m_Animator.SetTrigger("isDying");
        m_StandCollider.enabled = false;
        m_DeathCollider.enabled = true;
        dieCoroutine = StartCoroutine(WaitDie());
    }

    void Die(KillAllEnemiesEvent e)
    {
        Die();
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

    void Pause(GamePauseEvent e)
    {
        m_Animator.speed = 0;
    }

    void Resume(GameResumeEvent e)
    {
        m_Animator.speed = 1;
    }

    void DestroyEnemy(DestroyAllEnemiesEvent e)
    {
        DestroyEnemy();
    }

    void DestroyEnemy()
    {
        if (dieCoroutine != null)
        {
            StopCoroutine(dieCoroutine);
        }
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        Destroy(gameObject);
    }
}
