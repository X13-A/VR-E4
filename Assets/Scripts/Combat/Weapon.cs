using UnityEngine;
using System;
[Serializable]
public class Weapon
{
    public GameObject weaponPrefab;
    public GameObject bulletPrefab;
    public int nMunitionMax;
    public int nMunition;
    public float damage;
    public string name;

    // Constructeur de la classe Weapon
    public Weapon(GameObject weaponPrefab, GameObject bulletPrefab, int nMunitionMax, int nMunition, float damage, string name)
    {
        this.weaponPrefab = weaponPrefab;
        this.bulletPrefab = bulletPrefab;
        this.nMunitionMax = nMunitionMax;
        this.nMunition = nMunition;
        this.damage = damage;
        this.name = name;
    }
}