using Pooling;
using Scriptable_Objects;
using UnityEngine;

namespace LaneObjects
{
    public class Cloud : MovableLaneObject
    {
        public override MovableLaneObject PullFromPool() => CloudPool.Instance.Get();

        public override void SetupFromData(LaneObjectData data, float laneSpeed, int direction, Transform despawnPoint)
        {
            base.SetupFromData(data, laneSpeed, direction, despawnPoint);
        
            if (data is CloudData cData)
            {
                // Create Image
                if (SpriteRenderer && cData.cloudSprites is { Count: > 0 })
                {
                    int randomIndex = Random.Range(0, cData.cloudSprites.Count);
                    SpriteRenderer.sprite = cData.cloudSprites[randomIndex];
                }
            }
        }

        protected override void ReturnToPool()
        {
            IsActive = false;
            CloudPool.Instance.Return(this);
        }
    }
}