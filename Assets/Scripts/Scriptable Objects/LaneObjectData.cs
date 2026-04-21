using UnityEngine;

namespace Scriptable_Objects
{
    public abstract class LaneObjectData : ScriptableObject
    {
        [Header("Base Settings")]
        public float speed;
    }
}