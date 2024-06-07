using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : Singleton<WeaponManager>, IEventHandler
{
    [SerializeField] private List<GameObject> weapons;
    [SerializeField] private List<int> ammunitions;
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction switchTo1Action;
    private InputAction switchTo2Action;
    private InputAction switchTo3Action;

    public int CurrentWeapon { get; private set; }
    public int CurrentAmmo => ammunitions[CurrentWeapon];
    public bool CanShoot => CurrentAmmo > 0;

    protected override void Awake()
    {
        base.Awake();
        switchTo1Action = inputActionAsset.FindActionMap("XRI RightHand Interaction").FindAction("SwitchTo1");
        switchTo2Action = inputActionAsset.FindActionMap("XRI RightHand Interaction").FindAction("SwitchTo2");
        switchTo3Action = inputActionAsset.FindActionMap("XRI RightHand Interaction").FindAction("SwitchTo3");
        CurrentWeapon = -1;
        SwitchWeapon(0);
    }

    private void OnEnable()
    {
        switchTo1Action.performed += OnSwitchTo1Performed;
        switchTo2Action.performed += OnSwitchTo2Performed;
        switchTo3Action.performed += OnSwitchTo3Performed;
        switchTo1Action.Enable();
        switchTo2Action.Enable();
        switchTo3Action.Enable();
        SubscribeEvents();
    }

    private void OnDisable()
    {
        switchTo1Action.performed -= OnSwitchTo2Performed;
        switchTo2Action.performed -= OnSwitchTo2Performed;
        switchTo3Action.performed -= OnSwitchTo3Performed;
        switchTo1Action.Disable();
        switchTo2Action.Disable();
        switchTo3Action.Disable();
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

    private void OnSwitchTo1Performed(InputAction.CallbackContext context)
    {
        SwitchWeapon(0);
    }

    private void OnSwitchTo2Performed(InputAction.CallbackContext context)
    {
        SwitchWeapon(1);
    }

    private void OnSwitchTo3Performed(InputAction.CallbackContext context)
    {
        SwitchWeapon(2);
    }

    private void ConsumeAmmo(ShootEvent e)
    {
        ammunitions[CurrentWeapon] -= 1;
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
    }
}
