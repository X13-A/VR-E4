using System;
using UnityEngine;

// Impl�mentation de la classe AssaultRifle
[Serializable]
public class AssaultRifle : Weapon
{
    public AssaultRifle(GameObject weaponPrefab, GameObject bulletPrefab, int nMunitionMax, int nMunition, float damage, string name, float maxDistance, float rangeDistance)
        : base(weaponPrefab, bulletPrefab, nMunitionMax, nMunition, damage, name, maxDistance, rangeDistance)
    {
    }

    public override float GetDamage(float distance)
    {
        return 0f;
    }

    public override float GetRange()
    {
        // Impl�mentation vide pour le moment
        return 0f;
    }
}
