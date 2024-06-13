using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MainHand { Left, Right }

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private MainHand mainHand = MainHand.Right;
    [SerializeField] private Transform weaponRig;
    [SerializeField] private Transform weaponModel;
    [SerializeField] private Transform closeGrip;
    [SerializeField] private Transform farGrip;

    [SerializeField] private Transform leftHand; 
    [SerializeField] private Transform rightHand;

    private Transform mainHandTransform => mainHand == MainHand.Left ? leftHand : rightHand;
    private Transform secondHandTransform => mainHand == MainHand.Left ? rightHand : leftHand;

    void Update()
    {
        // Align rig with gun barrel
        Vector3 baseForward = Vector3.Normalize(secondHandTransform.position - mainHandTransform.position);
        weaponRig.position = mainHandTransform.position;
        weaponRig.forward = baseForward;

        // Adjust according to the position of the grips
        Vector3 delta = Vector3.Normalize(farGrip.position - closeGrip.position);
        Vector3 adjustedForward = Vector3.Normalize(Vector3.Reflect(-delta, baseForward));
        weaponModel.forward = adjustedForward;
    }
}
