using System;
using Pooling;
using Scriptable_Objects;
using UnityEngine;

namespace LaneObjects
{
    public abstract class MovableLaneObject : MonoBehaviour, IPoolable
    {
        protected SpriteRenderer SpriteRenderer;
        private float _speed;
        private float _endPointZValue;
        protected bool IsActive = true;

        protected virtual void Awake()
        {
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public abstract MovableLaneObject PullFromPool();

        public virtual void SetupFromData(LaneObjectData data, float laneSpeed, int direction, Transform despawnPoint)
        {
            _endPointZValue = Math.Abs(despawnPoint.position.z);
            _speed = (data.speed + laneSpeed) * direction;
        }

        protected virtual void Update()
        {
            if (!IsActive) return;
            Move();
            if (HasReachedEnd()) ReturnToPool();
        }

        private void Move() => transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
        private bool HasReachedEnd() => Math.Abs(transform.position.z) > _endPointZValue + 1;
        public void Reset() => IsActive = true;
        protected abstract void ReturnToPool();
    }
}