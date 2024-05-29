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
        Vector3 baseForward = Vector3.Normalize(secondHandTransform.position - mainHandTransform.position);
        weaponRig.position = mainHandTransform.position;
        weaponRig.forward = baseForward;

        Vector3 delta = Vector3.Normalize(farGrip.position - closeGrip.position);
        Vector3 adjustedForward = Vector3.Normalize(Vector3.Reflect(-delta, baseForward));
        weaponModel.forward = adjustedForward;
    }

    private void OnDrawGizmos()
    {
        return;
        Vector3 baseForward = Vector3.Normalize(secondHandTransform.position - mainHandTransform.position);
        Vector3 delta = Vector3.Normalize(farGrip.position - closeGrip.position);
        Vector3 adjustedForward = Vector3.Normalize(Vector3.Reflect(-delta, baseForward));
        Gizmos.color = Color.red;
        Gizmos.DrawRay(closeGrip.position, delta);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(mainHandTransform.position, baseForward * 10);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(mainHandTransform.position, adjustedForward * 10);
    }
}
