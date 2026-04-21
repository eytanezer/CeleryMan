using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Effects;
using Managment;

[Serializable]

public class Item : MonoBehaviour
{
    public enum ItemCategory { Throwable, Putable, Consumable, Objective }    
    // Data structure exposed in the Inspector to configure effects and their durations
    [Serializable]
    public struct EffectDefinition
    {
        public enum EffectType { Stun, Reverse, SpeedUp, SpeedDown, Armor } // Add new effect types here as the game expands
        public EffectType effectType;
        public float duration;
    }
    
    [Header("Item Settings")]
    [SerializeField] private ItemCategory category;
    
    [Tooltip("Relevant only if category is Throwable")]
    [SerializeField] private float speed = 15f;
    
    [Tooltip("How long before the item destroys itself (if it doesn't hit anything)")]
    [SerializeField] private float lifeTime = 3f;

    [Header("Effects To Apply")]
    // The list of effects configured in the Unity Inspector
    [SerializeField] private List<EffectDefinition> definedEffects = new List<EffectDefinition>();
    
    [Tooltip("How many seconds the item stays in hand before vanishing (0 = infinite)")]
    [SerializeField] private float holdTimeLimit = 5f;
    
    public float HoldTimeLimit => holdTimeLimit;
    
    
    private PlayerMovement.PlayerSlot _ownerSlot;
    public ItemCategory Category => category;
    public void Use(Vector3 direction, PlayerMovement.PlayerSlot owner, PlayerEffectManager ownerManager)
    {
        _ownerSlot = owner;

        switch (category)
        {
            case ItemCategory.Throwable:
                // Propels the object forward
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb) rb.linearVelocity = direction * speed;
                
                StartCoroutine(AutoDestroyRoutine());
                break;

            case ItemCategory.Putable:
                // Stays on the ground where it was placed
                StartCoroutine(AutoDestroyRoutine());
                break;

            case ItemCategory.Consumable:
                // Instantly applies all effects to the user and destroys the item
                ApplyAllEffects(ownerManager);
                Destroy(gameObject);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Consumables don't wait for physical triggers
        if (category == ItemCategory.Consumable) return; 

        PlayerMovement hitPlayer = other.GetComponent<PlayerMovement>();
        if (hitPlayer)
        {
            // Prevent the player from being hit by their own projectile or trap
            if (hitPlayer.playerSlot == _ownerSlot) return; 

            PlayerEffectManager effectManager = other.GetComponent<PlayerEffectManager>();
            if (effectManager)
            {
                ApplyAllEffects(effectManager);
                Destroy(gameObject); // Destroy the item after successful impact
            }
        }
    }

    private IEnumerator AutoDestroyRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void ApplyAllEffects(PlayerEffectManager targetManager)
    {
        // Iterate through all the effect definitions configured in the Inspector
        foreach (EffectDefinition def in definedEffects)
        {
            Effect newEffect = def.effectType switch
            {
                EffectDefinition.EffectType.Stun => new StunEffect(def.duration),
                EffectDefinition.EffectType.Reverse => new ReverseEffect(def.duration),
                EffectDefinition.EffectType.SpeedUp => new SpeedUpEffect(def.duration),
                EffectDefinition.EffectType.SpeedDown => new SpeedDownEffect(def.duration),
                EffectDefinition.EffectType.Armor => new ArmorEffect(def.duration),
                _ => null
            };

            if (newEffect != null) targetManager.AddEffect(newEffect);
        }
    }
    public override string ToString()
    {
        string definedEffectsString = "";
        foreach (EffectDefinition effect in definedEffects)
        {
            definedEffectsString += effect + " ";
        }
        return category+" "+definedEffectsString;
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