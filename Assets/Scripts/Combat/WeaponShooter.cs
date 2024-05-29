using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private Transform barrel;
    [SerializeField] private float range = 100f;

    void Shoot()
    {
        fireParticles.Stop();
        fireParticles.Play();

        RaycastHit hit;
        if (Physics.Raycast(barrel.position, barrel.forward, out hit, range, LayerMask.GetMask("Enemy")))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Touch();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }
}
