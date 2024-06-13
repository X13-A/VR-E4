using System.Collections;
using UnityEngine;
using PostProcess;
using SDD.Events;

public class PlayerManager : MonoBehaviour, IEventHandler
{
    public GameObject m_Camera;
    private BlinkEffect m_BlinkEffect;

    public AnimationCurve m_StartBlinkCurve;
    public AnimationCurve m_AttackBlinkCurve;

    private Transform m_CameraParent;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<AttackEvent>(AttackBlink);
        EventManager.Instance.AddListener<GamePlayEvent>(StartBlink);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<AttackEvent>(AttackBlink);
        EventManager.Instance.RemoveListener<GamePlayEvent>(StartBlink);
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

    void AttackBlink(AttackEvent e)
    {
        StartCoroutine(AttackBlink(e.enemyPosition));
    }

    void StartBlink(GamePlayEvent e)
    {
        StartCoroutine(StartBlink());
    }

    private IEnumerator StartBlink()
    {
        float t = 0;
        float tMax = GetMaxTimeFromCurve(m_StartBlinkCurve);
        while (t < tMax)
        {
            m_BlinkEffect.time = m_StartBlinkCurve.Evaluate(t);
            yield return null;
            t += Time.deltaTime;
            
        }
        m_BlinkEffect.time = m_StartBlinkCurve.Evaluate(tMax);
        EventManager.Instance.Raise(new StartBlinkingFinishedEvent());
    }

    private IEnumerator AttackBlink(Vector3 enemyPosition)
    {
        EventManager.Instance.Raise(new DisableAllGunsEvent());
        bool changeOrientation = true;
        float t = 0;
        float tMax = GetMaxTimeFromCurve(m_AttackBlinkCurve);

        while (t < tMax)
        {
            m_BlinkEffect.time = m_AttackBlinkCurve.Evaluate(t);
            yield return null;
            t += Time.deltaTime;
            if (changeOrientation && t >= 1.5f)
            {
                changeOrientation = false;

                // Calculate current rotation and target rotation
                Quaternion currentRotation = m_Camera.transform.rotation;
                Vector3 cameraPosition = new Vector3(m_Camera.transform.position.x,0f, m_Camera.transform.position.z);
                m_Camera.transform.position = cameraPosition;
                Quaternion targetRotation = Quaternion.LookRotation(enemyPosition  - cameraPosition);

                // Calculate the rotation difference
                Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);

                // Apply the rotation difference to the parent
                m_CameraParent.rotation = rotationDifference * m_CameraParent.rotation;
            }
        }
        m_BlinkEffect.time = m_StartBlinkCurve.Evaluate(tMax);
    }

    float GetMaxTimeFromCurve(AnimationCurve curve)
    {
        if (curve == null || curve.length == 0)
        {
            Debug.LogError("La courbe est nulle ou vide.");
            return 0f;
        }

        Keyframe[] keyframes = curve.keys;
        return keyframes[keyframes.Length - 1].time;
    }
}