using System.Collections;
using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] private float fallSpeed = 5f;
    
    [Tooltip("The height at which the item is considered to have hit the ground")]
    [SerializeField] private float groundYLevel = 0f; 
    
    [Header("Despawn Settings")]
    [Tooltip("How long the item will stay on the ground before disappearing if not collected")]
    [SerializeField] private float despawnTime = 10f;
    
    private bool _hasLanded = false;

    private void Update()
    {
        // If we haven't landed yet, continue moving down
        if (!_hasLanded)
        {
            // Downward movement (on the Y axis)
            transform.Translate(Vector3.down * (fallSpeed * Time.deltaTime), Space.World);

            // Check if we reached the ground
            if (transform.position.y <= groundYLevel)
            {
                LandingSequence();
            }
        }
    }
    
    private void LandingSequence()
    {
        _hasLanded = true;
        
        // Snap the item exactly to the ground level
        transform.position = new Vector3(transform.position.x, groundYLevel, transform.position.z);
        
        // Start the timer to despawn the item from the ground if ignored
        StartCoroutine(DespawnRoutine());
    }

    private IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }
}