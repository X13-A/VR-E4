using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class EnemySpawn : MonoBehaviour, IEventHandler
{
    private int crawlPorcent;
    private int nEnemy;
    private int nFastEnemy;
    private int nWalkingEnemy;
    private bool screamingEnemy;
    private GameObject enemy;
    [SerializeField] GameObject player;
    private float spawnRadius;
    private float spawnInterval;  // Intervalle entre les spawns en secondes
    private float deltaspawnInterval;
    private float angleStep;  // Intervalle d'angle en degr�s
    private bool readAngleActivate;
    private List<float> availableAngles;
    private float enemyLife;
    private int nNotDeadEnemy;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<LoadLevelEvent>(SetEnemySpawn);
        EventManager.Instance.AddListener<LoseEvent>(DestroyAllEnemies);
        EventManager.Instance.AddListener<ReviveEvent>(ReviveEnemy);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<LoadLevelEvent>(SetEnemySpawn);
        EventManager.Instance.RemoveListener<LoseEvent>(DestroyAllEnemies);
        EventManager.Instance.RemoveListener<ReviveEvent>(ReviveEnemy);
    }

    void DestroyAllEnemies(LoseEvent e)
    {
        EventManager.Instance.Raise(new DestroyAllEnemiesEvent());
    }

    void SetEnemySpawn(LoadLevelEvent e)
    {
        Level currentLevel = e.level;
        if (currentLevel != null) 
        {
            enemy = currentLevel.enemy;
            nWalkingEnemy = currentLevel.nWalkingEnemy;
            nFastEnemy = currentLevel.nFastEnemy;
            screamingEnemy = currentLevel.screamingEnemy;
            nEnemy = nWalkingEnemy + nFastEnemy;
            if (screamingEnemy)
            {
                nEnemy += 1;
            }
            nNotDeadEnemy = nEnemy;
            spawnRadius = currentLevel.spawnRadius;
            spawnInterval = currentLevel.spawnInterval;
            deltaspawnInterval = currentLevel.deltaspawnInterval;
            angleStep = currentLevel.angleStep;
            enemyLife   = currentLevel.enemyLife;
            readAngleActivate = currentLevel.readAngleActivate;
            crawlPorcent = currentLevel.crawlPorcent;
            InitializeAngles();
            StartCoroutine(SpawnEnemies());
        }
    }

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    void InitializeAngles()
    {
        availableAngles = new List<float>();
        for (float angle = 0; angle < 360; angle += angleStep)
        {
            availableAngles.Add(angle);
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (nEnemy>0)
        {
            if (availableAngles.Count >0)
            {
                if (nEnemy>1 || (nEnemy==1 && (!screamingEnemy||nNotDeadEnemy==1)))
                {
                    Spawn();
                    float randomFloat = Random.Range(-deltaspawnInterval, deltaspawnInterval);
                    yield return new WaitForSeconds(spawnInterval + randomFloat);
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    void Spawn()
    {
        // Tirer un angle al�atoire de la liste
        int randomIndex = Random.Range(0, availableAngles.Count);
        Debug.Log(randomIndex);
        Debug.Log(availableAngles.Count);
        float angle = availableAngles[randomIndex];
        availableAngles.RemoveAt(randomIndex);

        Vector3 spawnPosition = GetPointOnCircle(spawnRadius, player.transform.position, angle);
        GameObject newEnemy = Instantiate(enemy, spawnPosition, Quaternion.identity);

        // Pass the angle information to the enemy script
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            if(screamingEnemy && nEnemy == 1)
            {
                enemyScript.Initialize(this, angle, 2, false, enemyLife);
            }
            else
            {
                // Tirer un type d'ennemi al�atoire dans la liste
                int randomPorcent = Random.Range(0, 100);
                bool willCrawl = (randomPorcent < crawlPorcent);
                Debug.Log("willCrawl : " + willCrawl);
                int randomIndex2 = Random.Range(0, nFastEnemy + nWalkingEnemy);
                if (randomIndex2 < nFastEnemy)
                {
                    enemyScript.Initialize(this, angle, 1, willCrawl, enemyLife / 2);
                    nFastEnemy -= 1;
                }
                else
                {
                    enemyScript.Initialize(this, angle, 0, willCrawl, enemyLife);
                    nWalkingEnemy -= 1;
                }
            }
        }

        // Orienter l'ennemi vers le joueur uniquement sur l'axe Y
        Vector3 direction = player.transform.position - newEnemy.transform.position;
        direction.y = 0;  // Garder la direction dans le plan XZ
        newEnemy.transform.rotation = Quaternion.LookRotation(direction);

        nEnemy -= 1;
    }

    Vector3 GetPointOnCircle(float radius, Vector3 center, float angle)
    {
        // Convertir l'angle en radians pour les calculs trigonom�triques
        float angleRad = angle * Mathf.Deg2Rad;

        // Calculer les coordonn�es x et z en utilisant l'�quation param�trique du cercle
        float x = radius * Mathf.Cos(angleRad);
        float z = radius * Mathf.Sin(angleRad);

        // Ajuster les coordonn�es par rapport au centre du cercle
        Vector3 pointOnCircle = new Vector3(x, 0, z) + center;

        return pointOnCircle;
    }

    public void Death(float angle)
    {
        if (availableAngles.Count == 0)
        {
            InitializeAngles();
            Debug.Log("INITIALIZE ANGLES :"+availableAngles.Count);
        }
        else if (readAngleActivate && !availableAngles.Contains(angle))
        {
            availableAngles.Add(angle);
        }
        nNotDeadEnemy -= 1;
        if(nNotDeadEnemy == 0  && nEnemy==0)
        {
            EventManager.Instance.Raise(new AllEnemyDeadEvent());
            EventManager.Instance.Raise(new DestroyAllEnemiesEvent());
        }
    }

    void ReviveEnemy(ReviveEvent e)
    {
        nNotDeadEnemy += 1;
    }
}