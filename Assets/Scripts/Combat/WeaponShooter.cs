using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public enum WeaponType { Auto, SemiAuto }

public class WeaponShooter : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected float shootRate;
    [SerializeField] protected WeaponType weaponType;

    [Header("Function")]
    [SerializeField] protected InputActionAsset inputActionAsset;
    [SerializeField] protected Transform barrel;

    [Header("SFX")]
    [SerializeField] protected Light muzzleFlash;
    [SerializeField] protected ParticleSystem muzzleFlashParticles;
    [SerializeField] protected List<Animation> shootAnimations;


    protected float shootDelay => 1f / shootRate;
    protected float lastShootTime;

    protected InputAction shootAction;

    protected void Awake()
    {
        shootAction = inputActionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Shoot");
        shootAction.performed += contex => Shoot();
    }

    protected void OnEnable()
    {
        shootAction.Enable();
    }

    protected void OnDisable()
    {
        shootAction.performed -= ctx => Shoot();
        shootAction.Disable();
    }

    protected virtual bool CanShoot()
    {
        return Time.time - lastShootTime > shootDelay;
    }

    protected virtual void Shoot()
    {
        lastShootTime = Time.time;
    }
}
