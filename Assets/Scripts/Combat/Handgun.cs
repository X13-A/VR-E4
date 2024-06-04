using UnityEngine;
using System; // Assurez-vous que cet espace de noms est inclus

[Serializable]
public class Handgun : Weapon
{
    public Handgun(GameObject weaponPrefab, GameObject bulletPrefab, int nMunitionMax, int nMunition, float damage, string name, float maxDistance, float rangeDistance)
        : base(weaponPrefab, bulletPrefab, nMunitionMax, nMunition, damage, name, maxDistance, rangeDistance)
    {
    }

    public override float GetDamage(float distance)
    {
        // Impl�mentation vide pour le moment
        return 0f;
    }

    public override float GetRange()
    {
        // Impl�mentation vide pour le moment
        return 0f;
    }
}