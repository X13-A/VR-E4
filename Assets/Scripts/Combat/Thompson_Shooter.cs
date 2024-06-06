using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class Thompson_Shooter : WeaponShooter
{
    protected override void Shoot()
    {
        if (!CanShoot()) return;
        base.Shoot();
        AnimateShot();
        ShootBullet(barrel.forward);
    }

    private void AnimateShot()
    {
        // Sound
        float pitchVariation = Mathf.Sin(Time.time) * Random.Range(0, 0.025f);
        PlaySoundEvent soundEvent = new PlaySoundEvent
        {
            eNameClip = "thompson_shot",
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
        if (hapticFeedbackCoroutine != null)
        {
            StopCoroutine(hapticFeedbackCoroutine);
        }
        hapticFeedbackCoroutine = StartCoroutine(TriggerHapticFeedback());

        // Muzzle flash
        if (muzzleFlashCoroutine != null)
        {
            StopCoroutine(muzzleFlashCoroutine);
        }
        muzzleFlashCoroutine = StartCoroutine(MuzzleFlash());
    }
    
    private IEnumerator MuzzleFlash()
    {
        muzzleFlashParticles.Stop();
        muzzleFlashParticles.Play();
        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.enabled = false;
    }

    private IEnumerator TriggerHapticFeedback()
    {
        float duration = 0.05f;
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
