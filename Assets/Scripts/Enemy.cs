using UnityEngine;
using System.Collections;
using SDD.Events;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IEventHandler
{
    [SerializeField] float m_AttackDistance = 2f;
    [SerializeField] float m_Speed = 2f;
    [SerializeField] float m_FastSpeed = 6f;
    [SerializeField] float m_ScreamDistance = 6f;

    [SerializeField] GameObject hitParticles;

    private EnemySpawn m_Spawn;
    private float m_Angle;
    private float m_Life;
    private float m_MaxLife;
    private bool canTouch;
    private bool canAttack;
    private bool willScream;
    private bool willCrawl;
    private Animator m_Animator;
    private CapsuleCollider m_StandCollider;
    private BoxCollider m_DeathCollider;

    private Coroutine hitCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine dieCoroutine;
    private Coroutine screamCoroutine;
    private Coroutine standUpCoroutine;
    private Coroutine crawlCoroutine;
    private Rigidbody m_Rigidbody;

    private AudioSource m_audiSource;
    private AudioClip m_Groan;
    private AudioClip m_Death;
    [SerializeField] List<AudioClip> m_Groans;
    [SerializeField] List<AudioClip> m_Deaths;
    [SerializeField] AudioClip m_Hit;
    [SerializeField] AudioClip m_Runner;



    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<DestroyAllEnemiesEvent>(DestroyEnemy);
        EventManager.Instance.AddListener<KillAllEnemiesEvent>(Die);
        EventManager.Instance.AddListener<GamePauseEvent>(Pause);
        EventManager.Instance.AddListener<GameResumeEvent>(Resume);
        EventManager.Instance.AddListener<ScreamEvent>(StandUp);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<DestroyAllEnemiesEvent>(DestroyEnemy);
        EventManager.Instance.RemoveListener<KillAllEnemiesEvent>(Die);
        EventManager.Instance.RemoveListener<GamePauseEvent>(Pause);
        EventManager.Instance.RemoveListener<GameResumeEvent>(Resume);
        EventManager.Instance.RemoveListener<ScreamEvent>(StandUp);
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
        m_Animator.SetFloat("Speed", speed);
    }

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_DeathCollider = GetComponent<BoxCollider>();
        m_StandCollider = GetComponent<CapsuleCollider>();
        m_audiSource = GetComponent<AudioSource>();
        if(m_Groans.Count>0)
            m_Groan = m_Groans[Random.Range(0, m_Groans.Count)];
        if(m_Deaths.Count > 0)
            m_Death = m_Deaths[Random.Range(0, m_Deaths.Count)];
    }

    public void Initialize(EnemySpawn spawn, float angle, int mode, bool willCrawl, float life)
    {
        canTouch = true;
        willScream = false;
        Debug.Log("Zombie mode :" + mode);
        if (mode == 0) // Walking Zombie Mode
        {
            SetSpeed(m_Speed);
        }
        else if(mode == 1) // Fast Zombie Mode
        {
            SetSpeed(m_FastSpeed);
        }
        else
        {
            canTouch = false;
            willScream = true;
            SetSpeed(m_FastSpeed);

        }

        this.willCrawl = willCrawl;
        m_Spawn = spawn;
        m_Angle = angle;
      
        canAttack = true;
        m_Life = life;
        m_MaxLife = life;
        m_audiSource.clip = m_Groan;
        m_audiSource.loop = true;
        m_audiSource.Play();
    }

    void FixedUpdate()
    {
        float distanceToPlayer = DistanceXZ(m_Spawn.transform.position, transform.position);
        if(willScream && distanceToPlayer <= m_ScreamDistance)
        {
            Scream();
            Debug.Log("Scream");
        }
        else if (canAttack & distanceToPlayer <= m_AttackDistance)
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
        standUpCoroutine = StartCoroutine(WaitStandUp());
        m_DeathCollider.enabled = true;
        m_Rigidbody.useGravity = true;
        m_Life = m_MaxLife;
    }

    void StandUp(ScreamEvent e)
    {
        if(m_Life <= 0)
        {
            StandUp();
        }
    }

    private IEnumerator WaitAttack()
    {
        yield return new WaitForSeconds(1.5f);
        EventManager.Instance.Raise(new LoseEvent());
        attackCoroutine = null;
    }

    private IEnumerator WaitHit()
    {
        m_audiSource.Stop();
        m_audiSource.clip = m_Hit;
        m_audiSource.loop = false;
        m_audiSource.Play();
        yield return new WaitForSeconds(2f);
        m_audiSource.clip = m_Groan;
        m_audiSource.loop = true;
        m_audiSource.Play();
        m_Animator.SetBool("isHit1", false);
        m_Animator.SetBool("isHit2", false);
        Debug.Log("Can Touch !");
        hitCoroutine = null;
        FixRotation();
    }

    void Scream()
    {
        willScream = false;
        m_Animator.SetBool("isScreaming", true);
        screamCoroutine = StartCoroutine(WaitScream());
    }

    private IEnumerator WaitScream()
    {
        yield return new WaitForSeconds(1.15f);
        m_Animator.speed = 0.25f;
        SetSpeed(m_Speed);
        yield return new WaitForSeconds(3f);
        EventManager.Instance.Raise(new ScreamEvent());
        //m_Animator.speed = 1f;
        yield return new WaitForSeconds(3f);
        m_Animator.speed = 1f;
        m_Animator.SetBool("isScreaming", false);
        screamCoroutine = null;
        canTouch = true;

    }

    private IEnumerator WaitDie()
    {
        m_audiSource.Stop();
        m_audiSource.clip = m_Death;
        m_audiSource.loop = false;
        m_audiSource.Play();
        yield return new WaitForSeconds(2.5f);
        //DestroyEnemy();
        dieCoroutine = null;
        m_Rigidbody.useGravity = true;
        m_DeathCollider.enabled = false;
        m_Animator.SetBool("isCrawling", false);
        //StandUp();
        if (m_Spawn != null)
        {
            m_Spawn.Death(m_Angle);
        }
    }

    private IEnumerator WaitStandUp()
    {
        yield return new WaitForSeconds(3.3f);
        m_StandCollider.enabled = true;
        m_DeathCollider.enabled = false;
        m_Animator.SetBool("isStandingUp", false);
        canTouch = true;
        standUpCoroutine = null;
    }

    private IEnumerator WaitCrawl()
    {
        yield return new WaitForSeconds(4f);
        crawlCoroutine = null;
        m_Animator.SetBool("isCrawling", true);
        canTouch = true;
        m_Life = 1;
    }

    public static float DistanceXZ(Vector3 a, Vector3 b)
    {
        Vector3 delta = new Vector3(a.x - b.x, 0, a.z - b.z);
        return delta.magnitude;
    }

    public void Die()
    {
        canTouch = false;
        if (willCrawl)
        {
            m_Animator.SetTrigger("isDying2");
            willCrawl = false;
            crawlCoroutine = StartCoroutine(WaitCrawl());
        }
        else
        {
            m_Animator.SetTrigger("isDying");
            dieCoroutine = StartCoroutine(WaitDie());
        }

        m_StandCollider.enabled = false;
        m_DeathCollider.enabled = true;
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
            }
            else
            {
                m_Animator.SetBool("isHit2", false);
                m_Animator.SetBool("isHit1", true);
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

    public void Touch(float damage, Vector3? hitPoint = null)
    {
        m_Life -= damage;
        if(m_Life <= 0)
        {
            m_Life = 0;
            Die();
        }
        else
        {
            Hit();
        }

        Debug.Log(hitPoint);
        if (hitPoint != null)
        {
            StartCoroutine(PlayHitParticles(hitPoint.Value));
        }
    }

    private IEnumerator PlayHitParticles(Vector3 hitPoint)
    {
        GameObject particlesInstance = Instantiate(hitParticles);
        particlesInstance.transform.position = hitPoint;
        particlesInstance.GetComponent<ParticleSystem>().Stop();
        particlesInstance.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(5);
        Destroy(particlesInstance);
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
