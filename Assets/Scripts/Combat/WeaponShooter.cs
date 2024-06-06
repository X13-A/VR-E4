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
    [SerializeField] protected AnimationCurve falloffCurve;

    [Header("Function")]
    [SerializeField] protected InputActionAsset inputActionAsset;
    [SerializeField] protected Transform barrel;

    [Header("SFX")]
    [SerializeField] protected Light muzzleFlash;
    [SerializeField] protected ParticleSystem muzzleFlashParticles;
    [SerializeField] protected List<Animation> shootAnimations;

    protected Coroutine hapticFeedbackCoroutine;
    protected Coroutine muzzleFlashCoroutine;

    protected float shootDelay => 1f / shootRate;
    protected float lastShootTime;

    protected InputAction shootAction;

    protected void Awake()
    {
        shootAction = inputActionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Shoot");
        shootAction.performed += context => Shoot();
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

    protected virtual void ShootBullet(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(barrel.position, direction, out hit, range, LayerMask.GetMask("Enemy")))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                float fallOff = Mathf.Clamp01(falloffCurve.Evaluate(hit.distance / range));
                enemy.Touch((int)(damage * fallOff));
            }
        }
    }

    private void Update()
    {
        if (weaponType == WeaponType.Auto && shootAction.ReadValue<float>() > 0)
        {
            Shoot();
        }
    }
}
