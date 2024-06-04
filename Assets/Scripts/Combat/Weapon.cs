using UnityEngine;
using System;

// Classe de base WeaponBase
[Serializable]
public abstract class Weapon
{
    public GameObject WeaponPrefab { get; set; }
    public GameObject BulletPrefab { get; set; }
    public int NMunitionMax { get; set; }
    public int NMunition { get; set; }
    public float Damage { get; set; }
    public string Name { get; set; }
    public float MaxDistance { get; set; }
    public float RangeDistance { get; set; } // Distance à partir de laquelle les dégâts d'une munitions diminuent
    private bool canShoot;
    
    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        canShoot = true;
    }

    public void Reload()
    {
        NMunition = NMunitionMax;
    }

    public Weapon(GameObject weaponPrefab, GameObject bulletPrefab, int nMunitionMax, int nMunition, float damage, string name, float maxDistance, float rangeDistance)
    {
        WeaponPrefab = weaponPrefab;
        BulletPrefab = bulletPrefab;
        NMunitionMax = nMunitionMax;
        NMunition = nMunition;
        Damage = damage;
        Name = name;
        RangeDistance = rangeDistance;
        MaxDistance = maxDistance;
    }

    public abstract float GetDamage(float distance);
    public abstract float GetRange();
}