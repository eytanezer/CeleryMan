using LaneObjects;
using Managment;
using UnityEngine;

namespace Pooling
{
    public class VehiclePool : SimplePool<Vehicle>
    {
        private void OnEnable()
        {
            // Subscribe to the cheat event
            Cheats.OnResetVehiclesSpawning += ReturnAllActive;
            GameManager.OnGameReset += ReturnAllActive;
        }

        private void OnDisable()
        {
            // ALWAYS unsubscribe to prevent memory leaks/null reference errors
            Cheats.OnResetVehiclesSpawning -= ReturnAllActive;
            GameManager.OnGameReset -= ReturnAllActive;
        }

        private void ReturnAllActive()
        {
            // Loop through all child objects attached to this Pool's Transform
            foreach (Transform child in transform)
            {
                // If the object is currently active in the scene
                if (child.gameObject.activeInHierarchy)
                {
                    // Get the component of type T (e.g., Vehicle) and return it
                    if (child.TryGetComponent<Vehicle>(out var pooledObject))
                    {
                        Return(pooledObject);
                    }
                }
            }
        }
    }
}