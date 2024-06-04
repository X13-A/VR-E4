using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SmoothCameraMovement : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    public float speed = .8f;

    private Vector3 targetPosition;

    void Start()
    {
        if (pointA != null && pointB != null)
        {
            targetPosition = pointB.transform.position;
        }
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            if (targetPosition == pointA.transform.position)
            {
                targetPosition = pointB.transform.position;
            }
            else
            {
                targetPosition = pointA.transform.position;
            }
        }
    }
}

