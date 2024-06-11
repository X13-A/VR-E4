using SDD.Events;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour, IEventHandler
{
    [SerializeField] private TextMeshProUGUI munitions;
    [SerializeField] private TextMeshProUGUI waves;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<ShootEvent>(isShooting);
        EventManager.Instance.AddListener<SwitchWeaponEvent>(isSwitching);
        EventManager.Instance.AddListener<LoadLevelEvent>(LoadLevel);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ShootEvent>(isShooting);
        EventManager.Instance.RemoveListener<SwitchWeaponEvent>(isSwitching);
        EventManager.Instance.RemoveListener<LoadLevelEvent>(LoadLevel);
    }

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    void isShooting(ShootEvent e)
    {
        munitions.text = WeaponManager.Instance.CurrentAmmo.ToString();
        if (WeaponManager.Instance.CurrentAmmo == 0)
        {
            munitions.color = Color.red;
        }
    }

    void isSwitching(SwitchWeaponEvent e)
    {
        munitions.text = WeaponManager.Instance.CurrentAmmo.ToString();
        if (WeaponManager.Instance.CurrentAmmo == 0)
        {
            munitions.color = Color.red;
        }
    }

    void LoadLevel(LoadLevelEvent e)
    {
        waves.text = "wave: " + LevelManager.Instance?.IndexLevel + 1;
    }
}