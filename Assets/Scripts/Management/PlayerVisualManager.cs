using System;
using Managment;
using UnityEngine;

namespace Management
{
    public class PlayerVisualManager : MonoBehaviour
    {
        [Header("Master Control")]
        [Tooltip("Drag a parent object here that contains ALL player visuals (Mesh, Items, Effects)")]
        public GameObject visualRoot;
        
        [Header("Inventory Visuals (Holding Item)")]
        public GameObject holdingThrowableVisual;
        public GameObject holdingTrapVisual;
        public GameObject holdingConsumableVisual;

        [Header("Status Visuals (Under Effect)")]
        public GameObject stunVisual;
        public GameObject reverseVisual;
        public GameObject slowVisual;
        public GameObject speedVisual;
        public GameObject shieldVisual;

        public void SetInventoryVisual(Item.ItemCategory category, bool active)
        {
            switch (category)
            {
                case Item.ItemCategory.Throwable: holdingThrowableVisual?.SetActive(active); break;
                case Item.ItemCategory.Putable: holdingTrapVisual?.SetActive(active); break;
                case Item.ItemCategory.Consumable: holdingConsumableVisual?.SetActive(active); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }

        public void SetStatusVisual(Type effectType, bool active)
        {
            switch (effectType.Name)
            {
                case "StunEffect":
                    stunVisual?.SetActive(active);
                    break;
                case "ReverseEffect":
                    reverseVisual?.SetActive(active);
                    break;
                case "SpeedDownEffect":
                    slowVisual?.SetActive(active);
                    break;
                case "SpeedUpEffect":
                    speedVisual?.SetActive(active);
                    break;
                case "ArmorEffect":
                    shieldVisual?.SetActive(active);
                    break;
            }
        }
        
        private void SetAllVisualsActive(bool active)
        {
            if (visualRoot != null)
            {
                visualRoot.SetActive(active);
            }
            else
            {
                Debug.LogWarning("Visual Root is missing on PlayerVisualManager!");
            }
        }

        public void ActivateVisuals()
        {
            SetAllVisualsActive(true);
        }

        public void DeactivateVisuals()
        {
            SetAllVisualsActive(false);
        }
        
        void OnEnable()
        {
            GameManager.OnGameStarted += ActivateVisuals;
            GameManager.OnGameOver += DeactivateVisuals;
            GameManager.OnTitleScreen += DeactivateVisuals;
        }

        void OnDisable()
        {
            GameManager.OnGameStarted -= ActivateVisuals;
            GameManager.OnGameOver -= DeactivateVisuals;
            GameManager.OnTitleScreen -= DeactivateVisuals;
        }
    }
}