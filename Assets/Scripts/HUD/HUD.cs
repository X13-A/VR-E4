using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI munitions;
    [SerializeField] private TextMeshProUGUI waves;

    void Update()
    {
        munitions.text = WeaponManager.Instance.CurrentAmmo.ToString();
        waves.text = "wave: " + LevelManager.Instance.IndexLevel + 1;
    }
}