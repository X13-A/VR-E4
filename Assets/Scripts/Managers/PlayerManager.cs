using System.Collections;
using UnityEngine;
using PostProcess;
using SDD.Events;

public class PlayerManager : MonoBehaviour, IEventHandler
{
    public GameObject m_Camera;
    private BlinkEffect m_BlinkEffect;
    public AnimationCurve m_BlinkCurve;

    private Transform m_CameraParent;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<AttackEvent>(Attack);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<AttackEvent>(Attack);
    }

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    void Awake()
    {
        if (m_Camera == null)
        {
            Debug.LogError("m_Camera is not assigned in the Inspector");
            return;
        }

        m_BlinkEffect = m_Camera.GetComponent<BlinkEffect>();
        if (m_BlinkEffect == null)
        {
            Debug.LogError("BlinkEffect component not found on m_Camera");
        }

        // Get the parent transform of the camera
        m_CameraParent = m_Camera.transform.parent;
        if (m_CameraParent == null)
        {
            Debug.LogError("Camera does not have a parent transform.");
        }
    }

    void Attack(AttackEvent e)
    {
        StartCoroutine(Blink(e.enemyTransform));
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Blink");
            StartCoroutine(Blink());
        }*/
    }

    private IEnumerator Blink(Transform enemyTransform)
    {
        bool changeOrientation = true;
        float t = 0;

        while (t < 3)
        {
            yield return new WaitForSeconds(0.05f);
            m_BlinkEffect.time = m_BlinkCurve.Evaluate(t);
            if (changeOrientation && t >= 1.5f)
            {
                changeOrientation = false;

                // Calculate current rotation and target rotation
                Quaternion currentRotation = m_Camera.transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(enemyTransform.transform.position + new Vector3(0f,1.3f,0f)  - m_Camera.transform.position);

                // Calculate the rotation difference
                Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);

                // Apply the rotation difference to the parent
                m_CameraParent.rotation = rotationDifference * m_CameraParent.rotation;
            }
            t += 0.05f;
        }
    }
}