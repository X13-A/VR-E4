using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{

    [SerializeField] int nEnemy;
    [SerializeField] int nFastEnemy;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject player;
    [SerializeField] float spawnRadius = 10f;
    [SerializeField] float spawnInterval = 5f;  // Intervalle entre les spawns en secondes
    [SerializeField] float deltaspawnInterval = 1f;
    [SerializeField] float angleStep = 2f;  // Intervalle d'angle en degrés
    private List<float> availableAngles = new List<float>();

    void Start()
    {
        InitializeAngles();
        StartCoroutine(SpawnEnemies());
    }

    void InitializeAngles()
    {
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
            Debug.Log(randomFloat);
            yield return new WaitForSeconds(spawnInterval+deltaspawnInterval);
        }
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