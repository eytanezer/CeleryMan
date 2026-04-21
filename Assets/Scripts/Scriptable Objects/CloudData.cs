using System.Collections.Generic;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "GameData/Cloud", fileName = "New CloudData")]
    public class CloudData : LaneObjectData
    {
        [Header("Visual Pool")]
        [Tooltip("Add all your different cloud sprites here")]
        public List<Sprite> cloudSprites;
    }
}