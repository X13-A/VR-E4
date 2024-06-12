using UnityEngine;
using System.Collections;
using SDD.Events;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IEventHandler
{
    [SerializeField] float m_AttackDistance = 2f;
    [SerializeField] float m_CrawlAttackDistance = 1.3f;
    [SerializeField] float m_Speed = 2f;
    [SerializeField] float m_FastSpeed = 6f;
    [SerializeField] float m_ScreamDistance = 6f;
    [SerializeField] Transform headshotTransform;
    [SerializeField] float headshotDistance;
    [SerializeField] GameObject hitParticles;
    [SerializeField] GameObject headshotParticles;

    private EnemySpawn m_Spawn;
    private float m_Angle;
    private float m_Life;
    private float m_MaxLife;
    private bool canTouch;
    private bool canAttack;
    private bool willScream;
    private bool willCrawl;
    private bool isCrawling;
    private bool otherAttacking;
    private Animator m_Animator;
    private CapsuleCollider m_StandCollider;
    private BoxCollider m_DeathCollider;

    private Coroutine hitCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine dieCoroutine;
    private Coroutine screamCoroutine;
    private Coroutine standUpCoroutine;
    private Coroutine crawlCoroutine;
    private Coroutine groanSoundCoroutine;
    private Rigidbody m_Rigidbody;

    private AudioSource m_audiSource;
    private List<AudioClip> m_CurrentGroans = new List<AudioClip>();
    private float dtMinGroan;
    private float dtMaxGroan;

    [SerializeField] AnimationCurve m_DistanceVolume;
    [SerializeField] List<AudioClip> m_Groans;
    [SerializeField] List<AudioClip> m_SprintGroans;
    [SerializeField] List<AudioClip> m_ScreamerGroans;
    [SerializeField] List<AudioClip> m_CrawlGroans;
    [SerializeField] List<AudioClip> m_Attacks;
    [SerializeField] AudioClip m_HitSound;
    [SerializeField] AudioClip m_ScreamSound;
    [SerializeField] AudioClip m_DeathSound1;
    [SerializeField] AudioClip m_DeathSound2;



    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<DestroyAllEnemiesEvent>(DestroyEnemy);
        EventManager.Instance.AddListener<KillAllEnemiesEvent>(Die);
        EventManager.Instance.AddListener<GamePauseEvent>(Pause);
        EventManager.Instance.AddListener<GameResumeEvent>(Resume);
        EventManager.Instance.AddListener<ScreamEvent>(StandUp);
        EventManager.Instance.AddListener<AttackEvent>(PlayerAttacked);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<DestroyAllEnemiesEvent>(DestroyEnemy);
        EventManager.Instance.RemoveListener<KillAllEnemiesEvent>(Die);
        EventManager.Instance.RemoveListener<GamePauseEvent>(Pause);
        EventManager.Instance.RemoveListener<GameResumeEvent>(Resume);
        EventManager.Instance.RemoveListener<ScreamEvent>(StandUp);
        EventManager.Instance.RemoveListener<AttackEvent>(PlayerAttacked);
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
    }

    public void Initialize(EnemySpawn spawn, float angle, int mode, bool willCrawl, float life)
    {
        canTouch = true;
        willScream = false;
        if (mode == 0) // Walking Zombie Mode
        {
            SetSpeed(m_Speed);
            PlayGroanSound(m_Groans,3f,5f);
            m_Life = life;

        }
        else if(mode == 1) // Fast Zombie Mode
        {
            SetSpeed(m_FastSpeed);
            PlayGroanSound(m_SprintGroans, 1.5f, 2.5f);
            m_Life = life/2;
        }
        else
        {
            canTouch = false;
            willScream = true;
            SetSpeed(m_FastSpeed);
            PlayGroanSound(m_ScreamerGroans, 1.5f, 2.5f);
            m_Life = life;
        }

        this.willCrawl = willCrawl;
        m_Spawn = spawn;
        m_Angle = angle;
      
        canAttack = true;
        m_MaxLife = life;
    }

    void FixedUpdate()
    {
        float distanceToPlayer = DistanceXZ(m_Spawn.transform.position, transform.position);
        if (screamCoroutine == null || otherAttacking)
        {
            float volumeEntry = distanceToPlayer / 20f;
            if (volumeEntry > 1)
            {
                m_audiSource.volume = 0;
            }
            else
            {
                m_audiSource.volume = m_DistanceVolume.Evaluate(distanceToPlayer / 20f);
            }
        }
        if(willScream && distanceToPlayer <= m_ScreamDistance)
        {
            Scream();
        }
        else if ((!isCrawling && distanceToPlayer <= m_AttackDistance) || (isCrawling && distanceToPlayer <= m_CrawlAttackDistance))
        {
            if (canAttack)
                Attack();
            else
                SetSpeed(0f);
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
        canTouch = false;
        attackCoroutine = StartCoroutine(WaitAttack());
        Vector3 position = transform.position;
        if (!isCrawling)
        {
            position += new Vector3(0f, 1.3f, 0f);
        }
        else
        {
            position += new Vector3(0f, 0.2f, 0f);
        }
        EventManager.Instance.Raise(new AttackEvent { enemyPosition = position }) ;
    }

    void PlayerAttacked(AttackEvent e)
    {
        if (attackCoroutine == null)
        {
            DestroyEnemy();
        }
    }

    void StandUp()
    {
        EventManager.Instance.Raise(new ReviveEvent());
        FixRotation();
        m_Animator.SetBool("isStandingUp", true);
        standUpCoroutine = StartCoroutine(WaitStandUp());
        m_DeathCollider.enabled = true;
        m_Rigidbody.useGravity = true;
        m_Life = m_MaxLife;
        SetSpeed(m_Speed);
        PlayRandomSound(m_Groans);
    }

    void StandUp(ScreamEvent e)
    {
        if (m_Life <= 0)
        {
            float distanceToPlayer = DistanceXZ(m_Spawn.transform.position, transform.position);
            if (distanceToPlayer >= (m_AttackDistance + 1f))
                StandUp();
            else
                DestroyEnemy();
        }
    }

    private IEnumerator WaitAttack()
    {
        m_audiSource.Stop();
        m_Animator.speed = 0f;
        yield return new WaitForSeconds(2f);
        PlayRandomSound(m_Attacks);
        m_Animator.speed = 1f;
        yield return new WaitForSeconds(1.5f);
        EventManager.Instance.Raise(new LoseEvent());
        attackCoroutine = null;
    }

    private IEnumerator WaitHit()
    {
        yield return new WaitForSeconds(2f);
        m_Animator.SetBool("isHit1", false);
        m_Animator.SetBool("isHit2", false);
        hitCoroutine = null;
        FixRotation();
        PlayGroanSound();
    }

    void Scream()
    {
        willScream = false;
        m_Animator.SetBool("isScreaming", true);
        screamCoroutine = StartCoroutine(WaitScream());
        
    }

    private IEnumerator WaitScream()
    {
        yield return new WaitForSeconds(0.4f);
        float lastVolume = m_audiSource.volume;
        m_audiSource.volume = 1f;
        PlaySound(m_ScreamSound);
        yield return new WaitForSeconds(1f);
        m_Animator.speed = 0.25f;
        SetSpeed(m_Speed);
        yield return new WaitForSeconds(3f);
        EventManager.Instance.Raise(new ScreamEvent());
        //m_Animator.speed = 1f;
        yield return new WaitForSeconds(2.5f);
        m_Animator.speed = 1f;
        m_Animator.SetBool("isScreaming", false);
        m_audiSource.volume = lastVolume;
        screamCoroutine = null;
        canTouch = true;
        PlayGroanSound();

    }

    private IEnumerator WaitDie()
    {
        yield return new WaitForSeconds(2.5f);
        //DestroyEnemy();
        dieCoroutine = null;
        m_Rigidbody.useGravity = false;
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
        canAttack = true;
        canTouch = true;
        standUpCoroutine = null;
        PlayGroanSound();
    }

    private IEnumerator WaitCrawl()
    {
        yield return new WaitForSeconds(4f);
        crawlCoroutine = null;
        m_Animator.SetBool("isCrawling", true);
        canAttack = true;
        FixRotation();
        canTouch = true;
        m_Life = 1;
        isCrawling = true;
        PlayGroanSound(m_CrawlGroans,4f,5f);
    }

    public static float DistanceXZ(Vector3 a, Vector3 b)
    {
        Vector3 delta = new Vector3(a.x - b.x, 0, a.z - b.z);
        return delta.magnitude;
    }

    public void Die()
    {
        canTouch = false;
        canAttack = false;
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }
        if (willCrawl)
        {
            float distanceToPlayer = DistanceXZ(m_Spawn.transform.position, transform.position);
            PlaySound(m_DeathSound2);
            m_Animator.SetTrigger("isDying2");
            willCrawl = false;

            if (distanceToPlayer < 2f) // If Zombie is killed near the player he will not crawl
            {
                dieCoroutine = StartCoroutine(WaitDie());
            }
            else
            {
                crawlCoroutine = StartCoroutine(WaitCrawl());
            }
        }
        else
        {
            PlaySound(m_DeathSound1);
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
        PlaySound(m_HitSound);
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
        bool headshot = false;
        if (hitPoint != null)
        {
            headshot = Vector3.Distance(hitPoint.Value, headshotTransform.position) < headshotDistance;
            StartCoroutine(PlayHitParticles(hitPoint.Value, headshot));
        }

        if (!canTouch) return;

        if (headshot) m_Life -= damage * 2;
        else m_Life -= damage;

        if(m_Life <= 0)
        {
            m_Life = 0;
            Die();
        }
        else
        {
            Hit();
        }
    }

    private IEnumerator PlayHitParticles(Vector3 hitPoint, bool headshot)
    {
        GameObject particlesInstance;
        if (headshot)
        {
            Debug.Log("HEADSHOT");
            particlesInstance = Instantiate(headshotParticles);
        }
        else
        {
            particlesInstance = Instantiate(hitParticles);
        }

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
        if (groanSoundCoroutine != null)
        {
            StopCoroutine(groanSoundCoroutine);
        }
        Destroy(gameObject);
    }

    void PlaySound(AudioClip audio, bool groan = false)
    {
        if (!groan && groanSoundCoroutine != null)
        {
            StopCoroutine(groanSoundCoroutine);
            groanSoundCoroutine = null;
        }

        m_audiSource.Stop();
        m_audiSource.clip = audio;
        m_audiSource.loop = false;
        m_audiSource.Play();
    }

    void PlayRandomSound(List<AudioClip> audios, bool groan = false)
    {
        int i = Random.Range(0, audios.Count);
        PlaySound(audios[i], groan);
    }

    private void PlayGroanSound(List <AudioClip> groans = null, float dtMin = -1f, float dtMax = -1f)
    {
        if(groans != null)
            m_CurrentGroans = groans;
        if(dtMin >= 0)
            dtMinGroan = dtMin;
        if(dtMax >= 0)
            dtMaxGroan = dtMax;
        if (groanSoundCoroutine != null)
        {
            StopCoroutine(groanSoundCoroutine);
        }

        groanSoundCoroutine = StartCoroutine(GroanSound());
    }


    private IEnumerator GroanSound()
    {
        float dt = Random.Range(dtMinGroan, dtMaxGroan);
        PlayRandomSound(m_CurrentGroans, true);
        while (true)
        {          
            yield return new WaitForSeconds(dt);
            PlayRandomSound(m_CurrentGroans,true);
        }
    }
}
