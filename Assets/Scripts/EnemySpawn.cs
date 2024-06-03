using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class EnemySpawn : MonoBehaviour, IEventHandler
{

    private int nEnemy;
    private int nFastEnemy;
    private GameObject enemy;
    private GameObject player;
    private float spawnRadius = 10f;
    private float spawnInterval = 5f;  // Intervalle entre les spawns en secondes
    private float deltaspawnInterval = 1f;
    private float angleStep = 2f;  // Intervalle d'angle en degrés
    private List<float> availableAngles;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<LoadLevelEvent>(SetEnemySpawn);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<LoadLevelEvent>(SetEnemySpawn);
    }

    void SetEnemySpawn(LoadLevelEvent e)
    {
        Level currentLevel = e.level;
        if (currentLevel != null) 
        {
            enemy = currentLevel.enemy;
            nEnemy = currentLevel.nEnemy;
            nFastEnemy = currentLevel.nFastEnemy;
            spawnRadius = currentLevel.spawnRadius;
            spawnInterval = currentLevel.spawnInterval;
            deltaspawnInterval = currentLevel.deltaspawnInterval;
            angleStep = currentLevel.angleStep;
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
            if (availableAngles.Count > 0)
            {
                Spawn();
            }
            float randomFloat = Random.Range(-deltaspawnInterval, deltaspawnInterval);
            yield return new WaitForSeconds(spawnInterval+deltaspawnInterval);
        }
        EventManager.Instance.Raise(new AllEnemyHaveSpawnEvent());
    }

    void Spawn()
    {
        if (availableAngles.Count == 0) return;

        // Tirer un angle aléatoire de la liste
        int randomIndex = Random.Range(0, availableAngles.Count);
        // Tirer un type d'ennemi aléatoire dans la liste
        int randomIndex2 = Random.Range(0, nEnemy);
        float angle = availableAngles[randomIndex];
        availableAngles.RemoveAt(randomIndex);

        Vector3 spawnPosition = GetPointOnCircle(spawnRadius, player.transform.position, angle);
        GameObject newEnemy = Instantiate(enemy, spawnPosition, Quaternion.identity);

        // Pass the angle information to the enemy script
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            if (randomIndex2 < nFastEnemy)
            {
                enemyScript.Initialize(this, angle, 6);
                nFastEnemy -= 1;
            }
            else
            {
                enemyScript.Initialize(this, angle);
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
        // Convertir l'angle en radians pour les calculs trigonométriques
        float angleRad = angle * Mathf.Deg2Rad;

        // Calculer les coordonnées x et z en utilisant l'équation paramétrique du cercle
        float x = radius * Mathf.Cos(angleRad);
        float z = radius * Mathf.Sin(angleRad);

        // Ajuster les coordonnées par rapport au centre du cercle
        Vector3 pointOnCircle = new Vector3(x, 0, z) + center;

        return pointOnCircle;
    }

    public void ReaddAngle(float angle)
    {
        if (!availableAngles.Contains(angle))
        {
            availableAngles.Add(angle);
        }
    }
}