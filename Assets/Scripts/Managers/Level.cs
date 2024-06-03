using UnityEngine;
using UnityEngine.UI;
using System;
[Serializable]
public class Level
{
    public int nEnemy;
    public int nFastEnemy;
    public GameObject enemy;
    public float spawnRadius = 10f;
    public float spawnInterval = 5f;  // Intervalle entre les spawns en secondes
    public float deltaspawnInterval = 1f;
    public float angleStep = 2f;  // Intervalle d'angle en degrés


    // Constructeur de la classe Level
    public Level(int nEnemy, int nFastEnemy, GameObject enemy, float spawnRadius, float spawnInterval, float deltaSpawnInterval, float angleStep)
    {
        this.nEnemy = nEnemy;
        this.nFastEnemy = nFastEnemy;
        this.enemy = enemy;
        this.spawnRadius = spawnRadius;
        this.spawnInterval = spawnInterval;
        this.deltaspawnInterval = deltaSpawnInterval;
        this.angleStep = angleStep;
    }
}