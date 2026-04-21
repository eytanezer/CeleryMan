using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectiveTracker : MonoBehaviour
{
    [System.Serializable]
    public class ObjectiveSlot
    {
        public Item targetItemPrefab; //What item
        public Image uiIcon; //Floating image
        public Sprite uiIconPlaceHolder;
    }

    [Header("UI Setup")] [SerializeField] private List<ObjectiveSlot> objectiveSlots;

    void Start()
    {
        foreach (ObjectiveSlot slot in objectiveSlots)
        {
            if (!slot.targetItemPrefab || !slot.uiIcon) continue;

            ObjectiveItemData data = slot.targetItemPrefab.GetComponent<ObjectiveItemData>();
            if (data)
            {
                slot.uiIcon.sprite = data.outlineSprite;
            }
        }
    }

    public bool OnObjectiveCollected(Item collectedItemPrefab)
    {
        foreach (ObjectiveSlot slot in objectiveSlots)
        {
            // Find the slot for the prefab
            if (slot.targetItemPrefab == collectedItemPrefab)
            {
                ObjectiveItemData data = collectedItemPrefab.GetComponent<ObjectiveItemData>();
                if (data)
                {
                    slot.uiIcon.sprite = data.collectedSprite;
                    return true;
                }
                break;
            }
        }
        return false;
    }

    public void RemoveAllObjectives()
    {
        Debug.Log("Removing all objectives");
        foreach (ObjectiveSlot slot in objectiveSlots)
        {
            slot.uiIcon.sprite = slot.uiIconPlaceHolder;
        }
        
    }
}