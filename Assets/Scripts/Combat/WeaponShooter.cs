using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class WeaponShooter : MonoBehaviour
{
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private Transform barrel;
    [SerializeField] private float range = 100f;

    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction shootAction;
    
    private void Awake()
    {
        shootAction = inputActionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Shoot");
        shootAction.performed += contex => Shoot();

    }
    private void OnEnable()
    {
        shootAction.Enable();
    }

    private void OnDisable()
    {
        shootAction.performed -= ctx => Shoot();
        shootAction.Disable();
    }

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
                enemy.Touch(1);
            }
        }

        StartCoroutine(TriggerHapticFeedback());
    }

    private IEnumerator TriggerHapticFeedback()
    {
        float duration = 0.1f;
        float amplitude = 1f;

        // Find the right hand device
        var rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightHandDevice.isValid)
        {
            rightHandDevice.SendHapticImpulse(0, amplitude, duration);
        }

        // Find the left hand device
        var leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (leftHandDevice.isValid)
        {
            leftHandDevice.SendHapticImpulse(0, amplitude, duration);
        }

        yield return new WaitForSeconds(duration);
    }
}
