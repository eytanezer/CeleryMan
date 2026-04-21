using System.Collections;
using System.Collections.Generic;
using Management;
using Managment;
using UnityEngine;

namespace Effects
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerEffectManager : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private PlayerVisualManager _visuals;
        public bool HasArmor { get; set; } = false;

        private readonly Dictionary<System.Type, Coroutine> _activeCoroutines = new Dictionary<System.Type, Coroutine>();
        private readonly Dictionary<System.Type, Effect> _activeEffects = new Dictionary<System.Type, Effect>();

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _visuals = GetComponentInChildren<PlayerVisualManager>();
        }

        public void AddEffect(Effect newEffect)
        {
            if (newEffect == null) return;
            System.Type effectType = newEffect.GetType();
            if (_visuals) _visuals.SetStatusVisual(effectType, true);

            if (_activeCoroutines.ContainsKey(effectType))
            {
                StopCoroutine(_activeCoroutines[effectType]);
                _activeCoroutines[effectType] = StartCoroutine(RefreshEffectRoutine(_activeEffects[effectType], newEffect.Duration));
            }
            else
            {
                _activeEffects[effectType] = newEffect;
                _activeCoroutines[effectType] = StartCoroutine(EffectRoutine(newEffect));
            }
        }

        private IEnumerator EffectRoutine(Effect effect)
        {
            effect.Apply(_playerMovement);
            yield return new WaitForSeconds(effect.Duration);
            effect.Remove(_playerMovement);

            // End visual when effect ends
            if (_visuals) _visuals.SetStatusVisual(effect.GetType(), false);

            _activeCoroutines.Remove(effect.GetType());
            _activeEffects.Remove(effect.GetType());
        }

        private IEnumerator RefreshEffectRoutine(Effect existingEffect, float newDuration)
        {
            yield return new WaitForSeconds(newDuration);
            existingEffect.Remove(_playerMovement);

            // Reset visual when gets effect again
            if (_visuals) _visuals.SetStatusVisual(existingEffect.GetType(), false);

            _activeCoroutines.Remove(existingEffect.GetType());
            _activeEffects.Remove(existingEffect.GetType());
        }
    }
}