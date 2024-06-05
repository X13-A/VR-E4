using UnityEngine;
using UnityEngine.UI;
using System;
[Serializable]
public class Level
{
    public int nWalkingEnemy;
    public int nFastEnemy;
    public bool screamingEnemy;
    public GameObject enemy;
    public float spawnRadius;
    public float spawnInterval;  // Intervalle entre les spawns en secondes
    public float deltaspawnInterval;
    public float angleStep;  // Intervalle d'angle en degrés
    public bool readAngleActivate;
    public int enemyLife;


    // Constructeur de la classe Level
    public Level(int nWalkingEnemy, int nFastEnemy, bool screamingEnemy, GameObject enemy, float spawnRadius, float spawnInterval
        , float deltaSpawnInterval, float angleStep, bool readAngleActivate, int enemyLife)
    {
        this.nFastEnemy = nFastEnemy;
        this.enemy = enemy;
        this.spawnRadius = spawnRadius;
        this.spawnInterval = spawnInterval;
        this.deltaspawnInterval = deltaSpawnInterval;
        this.angleStep = angleStep;
        this.readAngleActivate = readAngleActivate;
        this.enemyLife = enemyLife;
    }
}