using SDD.Events;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour, IEventHandler
{
    [SerializeField] private TextMeshProUGUI munitions;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<ShootEvent>(isShooting);
        EventManager.Instance.AddListener<SwitchWeaponEvent>(isSwitching);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ShootEvent>(isShooting);
        EventManager.Instance.RemoveListener<SwitchWeaponEvent>(isSwitching);
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
}