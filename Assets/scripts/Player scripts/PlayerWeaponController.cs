using UnityEngine;
using System.Collections.Generic;  // Required for using Dictionary

public class PlayerWeaponController : MonoBehaviour
{
    // Enum to define weapon types
    private enum WeaponType
    {
        Weapon1,
        Weapon2,
        Weapon3
    }

    [SerializeField] private GameObject[] weapons; // Array to store weapon GameObjects

    private Dictionary<WeaponType, GameObject> weaponMap;  // Dictionary to map weapon types to GameObjects
    private Dictionary<WeaponType, string> weaponSoundMap; // Dictionary to map weapon types to sound names

    private GameObject currentWeapon;  // The current active weapon

    void Start()
    {
        // Initialize weaponMap and weaponSoundMap
        weaponMap = new Dictionary<WeaponType, GameObject>()
        {
            { WeaponType.Weapon1, weapons[0] },
            { WeaponType.Weapon2, weapons[1] },
            { WeaponType.Weapon3, weapons[2] }
        };

        //PLAY THE RELOAD SFX BASED ON WEAPON
        weaponSoundMap = new Dictionary<WeaponType, string>()
        {
            { WeaponType.Weapon1, "weapon1 reload" },
            { WeaponType.Weapon2, "weapon2 reload" },
            { WeaponType.Weapon3, "weapon3 reload" }
        };

        // Initialize with weapon1 as the starting weapon
        SetActiveWeapon(WeaponType.Weapon1);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Determine which weapon to activate based on the tag of the collided object
        if (collision.gameObject.CompareTag("Weapon1Pickable"))
        {
            SetActiveWeapon(WeaponType.Weapon1);
            Destroy(collision.gameObject);  // Destroy the pickable object
        }
        else if (collision.gameObject.CompareTag("Weapon2Pickable"))
        {
            SetActiveWeapon(WeaponType.Weapon2);
            Destroy(collision.gameObject);  // Destroy the pickable object
        }
        else if (collision.gameObject.CompareTag("Weapon3Pickable"))
        {
            SetActiveWeapon(WeaponType.Weapon3);
            Destroy(collision.gameObject);  // Destroy the pickable object
        }
    }

    void SetActiveWeapon(WeaponType weaponType)
    {
        // Deactivate the current weapon
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }

        // Set the new weapon as active
        if (weaponMap.TryGetValue(weaponType, out GameObject newWeapon) && newWeapon != null)
        {
            currentWeapon = newWeapon;
            currentWeapon.SetActive(true);

            // Play the new weapon's sound
            PlayWeaponSound(weaponType);
        }
    }

    void PlayWeaponSound(WeaponType weaponType)
    {
        // Play sound if the weapon type has a corresponding sound
        if (weaponSoundMap.TryGetValue(weaponType, out string soundName))
        {
            AudioManager.Instance.PlaySFX(soundName);
        }
        else
        {
            Debug.LogError("Sound not found for weapon: " + weaponType);
        }
    }
}
