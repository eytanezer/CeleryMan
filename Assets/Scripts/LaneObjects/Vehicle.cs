using ManagmentScripts.SoundScripts;
using Pooling;
using Scriptable_Objects;
using UnityEngine;

namespace LaneObjects
{
    public class Vehicle : MovableLaneObject
    {
        public float StunDurationWhenHit { get; private set; }
        private Management.SoundScripts.AudioSourcePoolable audioSource;

        public override MovableLaneObject PullFromPool() => VehiclePool.Instance.Get();

        public override void SetupFromData(LaneObjectData data, float laneSpeed, int direction, Transform despawnPoint)
        {
            base.SetupFromData(data, laneSpeed, direction, despawnPoint);
        
            // Read data from Scriptable Object
            if (data is VehicleData vData)
            {
                StunDurationWhenHit = vData.stunDurationWhenHit;
                
                // Get new sprite
                if (SpriteRenderer && vData.frontSprites.Count > 0 && vData.backSprites.Count > 0)
                {
                    int randomIndex = Random.Range(0, vData.frontSprites.Count);
                    SpriteRenderer.sprite = direction == -1 ? vData.frontSprites[randomIndex] : vData.backSprites[randomIndex];
                }
                
                if (vData.engineSounds != null && SoundManager.Instance != null)
                {
                    audioSource = ManagmentScripts.SoundScripts.SoundManager.Instance.PlayLoopingSoundFX(vData.engineSounds, transform, 1f);
                }
            }
        }

        protected override void ReturnToPool()
        {
            IsActive = false;
            VehiclePool.Instance.Return(this);
        }
        
        // so that the sounds return to pool when reset
        private void OnDisable()
        {
            if (audioSource != null)
            {
                Management.SoundScripts.AudioPool.Instance.Return(audioSource);
                audioSource = null;
            }
        }
    }
}