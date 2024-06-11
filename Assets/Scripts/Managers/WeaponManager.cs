using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : Singleton<WeaponManager>, IEventHandler
{
    [SerializeField] private List<GameObject> weapons;
    [SerializeField] private List<int> ammunitions;
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction switchNextAction;
    private InputAction switchPrevAction;

    public int CurrentWeapon { get; private set; }
    public int CurrentAmmo => ammunitions[CurrentWeapon];
    public bool CanShoot => CurrentAmmo > 0;

    protected override void Awake()
    {
        base.Awake();
        switchNextAction = inputActionAsset.FindActionMap("XRI RightHand Interaction").FindAction("SwitchNext");
        switchPrevAction = inputActionAsset.FindActionMap("XRI RightHand Interaction").FindAction("SwitchPrev");
        CurrentWeapon = -1;
        SwitchWeapon(0);
    }

    private void OnEnable()
    {
        switchNextAction.performed += OnSwitchNextPerformed;
        switchPrevAction.performed += OnSwitchPrevPerformed;
        switchNextAction.Enable();
        switchPrevAction.Enable();
        SubscribeEvents();
    }

    private void OnDisable()
    {
        switchNextAction.performed -= OnSwitchPrevPerformed;
        switchPrevAction.performed -= OnSwitchPrevPerformed;
        switchNextAction.Disable();
        switchPrevAction.Disable();
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<ShootEvent>(ConsumeAmmo);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ShootEvent>(ConsumeAmmo);
    }

    private void OnSwitchNextPerformed(InputAction.CallbackContext context)
    {
        SwitchWeapon((CurrentWeapon + 1) % weapons.Count);
    }

    private void OnSwitchPrevPerformed(InputAction.CallbackContext context)
    {
        SwitchWeapon((weapons.Count + CurrentWeapon - 1) % weapons.Count);
    }

    private void ConsumeAmmo(ShootEvent e)
    {
        ammunitions[CurrentWeapon] -= 1;
    }

    private void DisableAllGuns(DisableAllGunsEvent e)
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
    }

    private void SwitchWeapon(int index)
    {
        if (CurrentWeapon == index) return;

        CurrentWeapon = index;
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
        weapons[index].SetActive(true);

        // Switch sound
        EventManager.Instance.Raise(new PlaySoundEvent
        {
            eNameClip = $"switch_gun{index + 1}",
            eLoop = false,
            eCanStack = true,
            eDestroyWhenFinished = true,
            eVolumeMultiplier = 1f
        });

        EventManager.Instance.Raise(new SwitchWeaponEvent());
    }
}
