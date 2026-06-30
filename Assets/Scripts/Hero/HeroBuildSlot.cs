using UnityEngine;

public class HeroBuildSlot : MonoBehaviour
{
    [Header("State")]
    public bool hasHero = false;

    [Header("Parent")]
    public Transform heroContainer;

    [Header("Cost")]
    public int heroCost = 30;

    public bool TryPlaceHero(GameObject heroPrefab, Projectile projectilePrefab, Transform projectileContainer)
    {
        if (hasHero)
        {
            Debug.Log(gameObject.name + " already has a hero!");
            return false;
        }

        if (heroPrefab == null)
        {
            Debug.LogError("Hero Prefab is missing!");
            return false;
        }

        // Trừ tiền TRƯỚC khi tạo hero
        MapGameManager gameManager = FindFirstObjectByType<MapGameManager>();

        if (gameManager != null)
        {
            bool paid = gameManager.SpendGold(heroCost);

            if (!paid)
            {
                Debug.Log("Not enough gold to place hero!");
                return false;
            }
        }
        else
        {
            Debug.LogError("MapGameManager not found!");
            return false;
        }

        // Đủ tiền rồi mới tạo hero
        GameObject newHero = Instantiate(
            heroPrefab,
            transform.position,
            Quaternion.identity
        );

        newHero.name = heroPrefab.name + "_Placed";

        if (heroContainer != null)
        {
            newHero.transform.SetParent(heroContainer);
        }

        HeroAutoAttack heroAttack = newHero.GetComponentInChildren<HeroAutoAttack>();

        if (heroAttack != null)
        {
            heroAttack.projectilePrefab = projectilePrefab;
            heroAttack.projectileContainer = projectileContainer;
        }
        else
        {
            Debug.LogError("HeroAutoAttack is missing on spawned hero!");
        }

        hasHero = true;

        Debug.Log("Placed hero on " + gameObject.name + ". Cost: " + heroCost);

        return true;
    }
}