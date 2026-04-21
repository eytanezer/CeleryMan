using Management;
using Managment;
using ManagmentScripts.SoundScripts;
using UnityEngine;

/// <summary>
/// An object placed in the world that grants a new Item to the player who touches it.
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [Tooltip("The Prefab of the item this pickup will give to the player")] [SerializeField]
    private Item itemToGrant;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player && itemToGrant)
        {
            if (itemToGrant.Category == Item.ItemCategory.Objective)
            {
                player.CollectObjective(itemToGrant); // Adding Objective items
            }
            else
            {
                player.EquipItem(itemToGrant, GetComponent<SpriteRenderer>().sprite);
                PlayerVisualManager visuals = other.GetComponentInChildren<PlayerVisualManager>();
                if (visuals) visuals.SetInventoryVisual(itemToGrant.Category, true);
            }

            Debug.Log(player.playerSlot + " has got " + itemToGrant);
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        GameManager.OnGameOver += Cleanup;
    }

    // חשוב מאוד! אם החפץ נאסף על ידי שחקן ומושמד, הוא חייב להפסיק להאזין
    // אחרת יוניטי תזרוק שגיאות של NullReference בסוף המשחק
    private void OnDisable()
    {
        GameManager.OnGameOver -= Cleanup;
    }

    // הפונקציה שתופעל ברגע שה-GameManager צועק "סוף משחק"
    private void Cleanup()
    {
        Destroy(gameObject);
    }
}