using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    // Prefabs for the different items
    [SerializeField] private GameObject healItemPrefab;
    [SerializeField] private GameObject weapon1Prefab;
    [SerializeField] private GameObject weapon2Prefab;
    [SerializeField] private GameObject weapon3Prefab;

    // Drop chances for items
    [SerializeField] [Range(0f, 1f)] private float dropChance = 0.1f;
    [SerializeField] [Range(0f, 1f)] private float healDropChance = 0.3f;
    [SerializeField] [Range(0f, 1f)] private float weaponDropChance = 0.2f; // Shared drop chance for each weapon

    // Cooldown management for weapon drops
    private float weaponCooldown = 120f;
    private float lastWeaponDropTime = -120f;

    /// <summary>
    /// Attempts to drop an item at the specified position.
    /// </summary>
    // <param name="position">The position where the item should be dropped.</param>
    public void TryDropItem(Vector2 position)
    {
        // Check if an item should be dropped
        if (Random.value <= dropChance)
        {
            // Calculate total probability range for deciding between heal and weapons
            float totalDropChance = healDropChance + weaponDropChance * 3;
            float randomValue = Random.value * totalDropChance;

            // Determine if we should drop a heal item
            if (randomValue <= healDropChance)
            {
                Instantiate(healItemPrefab, position, Quaternion.identity);
            }
            else
            {
                // Check if weapon drop is available based on cooldown
                if (Time.time >= lastWeaponDropTime + weaponCooldown)
                {
                    // Determine which weapon to drop
                    int weaponIndex = Mathf.FloorToInt((randomValue - healDropChance) / weaponDropChance);
                    DropWeapon(weaponIndex, position);

                    // Update the last weapon drop time
                    lastWeaponDropTime = Time.time;
                }
            }
        }
    }

    /// <summary>
    /// Drops the specified weapon at the given position.
    //<param name="weaponIndex">Index of the weapon to drop.</param>
    // <param name="position">The position where the weapon should be dropped.</param>
    /// </summary>
    private void DropWeapon(int weaponIndex, Vector2 position)
    {
        switch (weaponIndex)
        {
            case 0:
                Instantiate(weapon1Prefab, position, Quaternion.identity);
                break;
            case 1:
                Instantiate(weapon2Prefab, position, Quaternion.identity);
                break;
            case 2:
                Instantiate(weapon3Prefab, position, Quaternion.identity);
                break;
            default:
                Debug.LogWarning("Invalid weapon index: " + weaponIndex);
                break;
        }
    }
}
