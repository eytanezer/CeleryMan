using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicCameraZoom : MonoBehaviour
{
    [Header("Players To Track")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [Header("Tracking Settings")]
    [Tooltip("How fast the camera follows the midpoint between the players")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Distance Thresholds (For Zoom)")]
    [Tooltip("The minimum distance between players (triggers maximum zoom-in)")]
    [SerializeField] private float minDistance = 5f;
    [Tooltip("The maximum distance between players (triggers maximum zoom-out)")]
    [SerializeField] private float maxDistance = 40f;

    [Header("Zoom Settings (FOV / Size)")]
    [Tooltip("Zoom level when close (smaller number = more zoomed in)")]
    [SerializeField] private float mostZoomedIn = 5f;
    [Tooltip("Zoom level when far apart (larger number = more zoomed out)")]
    [SerializeField] private float mostZoomedOut = 20f;
    
    [Tooltip("How fast and smooth the camera changes the zoom level")]
    [SerializeField] private float zoomSpeed = 3f;

    private Camera _cam;
    private Vector3 _initialOffset;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (player1 && player2)
        {
            // Calculate the midpoint at the start of the game
            Vector3 startCenter = (player1.position + player2.position) / 2f;
            
            // Save the initial distance and angle of the camera from the starting midpoint
            _initialOffset = transform.position - startCenter;
        }
    }

    private void LateUpdate()
    {
        if (!player1 || !player2) return;

        // Tracking Logic
        // Calculate the current midpoint
        Vector3 centerPoint = (player1.position + player2.position) / 2f;
        
        // The target position for the camera (midpoint + initial offset)
        Vector3 targetPosition = centerPoint + _initialOffset;
        
        // Smooth movement towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        // Zoom Logic
        float currentDistance = Vector3.Distance(player1.position, player2.position);
        float distancePercentage = Mathf.InverseLerp(minDistance, maxDistance, currentDistance);
        float targetZoom = Mathf.Lerp(mostZoomedIn, mostZoomedOut, distancePercentage);

        if (_cam.orthographic)
        {
            _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }
        else
        {
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
        }
    }
}