using UnityEngine;

public class ObjectBounds : MonoBehaviour
{
    [Header("Boundary References")]
    public GameObject lowerBoundObject; 
    public GameObject upperBoundObject;

    [Header("Objects to Constrain")]
    public GameObject[] objectsToKeepInBounds;

    private float minX, maxX, minZ, maxZ;

    void LateUpdate()
    {
        if (!lowerBoundObject || !upperBoundObject) return;

        // 1. Calculate the current boundaries
        CalculateBounds();

        // 2. Apply constraints to each object
        foreach (GameObject obj in objectsToKeepInBounds)
        {
            if (obj)
            {
                ConstrainObject(obj);
            }
        }
    }

    void CalculateBounds()
    {
        Vector3 pos1 = lowerBoundObject.transform.position;
        Vector3 pos2 = upperBoundObject.transform.position;

        minX = Mathf.Min(pos1.x, pos2.x);
        maxX = Mathf.Max(pos1.x, pos2.x);
        minZ = Mathf.Min(pos1.z, pos2.z);
        maxZ = Mathf.Max(pos1.z, pos2.z);
    }

    void ConstrainObject(GameObject obj)
    {
        Vector3 currentPos = obj.transform.position;

        // Clamp the X and Y within our calculated bounds
        float clampedX = Mathf.Clamp(currentPos.x, minX, maxX);
        float clampedZ = Mathf.Clamp(currentPos.z, minZ, maxZ);

        obj.transform.position = new Vector3(clampedX, currentPos.y, clampedZ);
    }
    
    // Optional: Draw the bounds in the editor for easier debugging
    private void OnDrawGizmos()
    {
        if (lowerBoundObject && upperBoundObject)
        {
            CalculateBounds();
            Gizmos.color = Color.green;
            Vector3 center = new Vector3((minX + maxX) / 2,0, (minZ + maxZ) / 2);
            Vector3 size = new Vector3(maxX - minX,0.1f, maxZ - minZ);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
