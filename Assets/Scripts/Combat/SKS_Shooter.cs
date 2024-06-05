using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class SKS_Shooter : WeaponShooter
{
    protected override void Shoot()
    {
        if (!CanShoot()) return;
        base.Shoot();
        StartCoroutine(AnimateShot());

        RaycastHit hit;
        if (Physics.Raycast(barrel.position, barrel.forward, out hit, range, LayerMask.GetMask("Enemy")))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Touch((int) damage);
            }
        }
    }

    private IEnumerator AnimateShot()
    {
        // Sound
        float pitchVariation = Mathf.Sin(Time.time) * Random.Range(0, 0.15f);
        PlaySoundEvent soundEvent = new PlaySoundEvent
        {
            eNameClip = "SKS_shot",
            eLoop = false,
            eCanStack = true,
            eDestroyWhenFinished = true,
            ePitch = 1 + pitchVariation,
            eVolumeMultiplier = 1.5f
        };
        EventManager.Instance.Raise(soundEvent);

        // Animations
        foreach (Animation animation in shootAnimations)
        {
            animation.Stop();
            animation.Play();
        }

        // Haptic feedback
        StartCoroutine(TriggerHapticFeedback());

        // Muzzle flash
        muzzleFlashParticles.Stop();
        muzzleFlashParticles.Play();
        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.enabled = false;
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
