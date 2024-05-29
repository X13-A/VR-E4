using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private Transform weapon;

    [SerializeField] private Transform player;
    [SerializeField] private Transform leftHand; 
    [SerializeField] private Transform rightHand; 

    private float GetHandDist(Transform hand)
    {
        return Vector3.Distance(hand.position, player.position);
    }

    void Update()
    {
        float leftHandDist = GetHandDist(leftHand);
        float rightHandDist = GetHandDist(rightHand);

        Transform closestHand;
        Transform furthestHand;

        if (leftHandDist > rightHandDist)
        {
            closestHand = rightHand;
            furthestHand = leftHand;
        }
        else
        {
            closestHand = rightHand;
            furthestHand = leftHand;
        }

        Vector3 direction = Vector3.Normalize(leftHand.position - rightHand.position);

        weapon.position = closestHand.position;
        weapon.forward = direction;
    }
}
