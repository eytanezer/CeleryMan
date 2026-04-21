using System.Collections.Generic;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "GameData/Vehicle", fileName = "New VehicleData")]
    public class VehicleData : LaneObjectData
    {
        [Header("Vehicle Logic")]
        public float stunDurationWhenHit;
        [Header("Visual Pool")]
        public List<Sprite> frontSprites;
        public List<Sprite> backSprites;
        
        public AudioClip engineSounds;
    }
}