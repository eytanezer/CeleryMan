using System.Collections;
using System.Collections.Generic;
using Effects;
using LaneObjects;
using Management;
using Managment;
using ManagmentScripts.SoundScripts;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the movement of the players
/// 
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    private const float SpeedYMultiplier = 0.75f;
    private static readonly int GotRanOver = Animator.StringToHash("GotRanOver");
    private static readonly int BackToIdle = Animator.StringToHash("BackToIdle");
    private static readonly int Speed1 = Animator.StringToHash("Speed");
    private static readonly int BeatUp = Animator.StringToHash("BeatUp");

    //Player Controller Settings
    public enum PlayerSlot
    {
        Player1,
        Player2
    }

    public PlayerSlot playerSlot;
    private Vector3 _spawnPosition;
    private SharedInputManager _inputManager;
    private InputAction _moveAction;
    private InputAction _fireAction;
    private float _movementStatus = 1;
    private bool _gotRanOver = false;
    private ObjectiveTracker _tracker;

    [Header("Objectives Inventory")] [SerializeField]
    private List<Item> collectedObjectives = new List<Item>();

    [Header("Combat Settings")] [SerializeField]
    private Item itemPrefab;
    [SerializeField] private AudioClip beatUpSound;

    [SerializeField] private float spawnDistance = 1f; //Distance for creating item
    private Vector3 _lastFacingDirection = Vector3.forward;

    [Header("Movement Settings")] [SerializeField]
    private float speed = 5;

    [SerializeField] private float hitCoolTime = 1f;
    private float coolTime;
    

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    // Components
    private Rigidbody _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    // private PlayerVisualManager _playerVisualManager;
    
    private float cameraXDegree = 18f;
    private float yCorrection;
    private Vector3 _startPosition;

    // private static float _topOfScreen; // could be used for bounds
    private ParticleSystem _particles;
    private Coroutine _itemExpirationCoroutine;

    private bool GotBeatUp = false;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _spawnPosition = transform.position;
        _particles = GetComponentInChildren<ParticleSystem>(true);
        _animator = GetComponentInChildren<Animator>();
        _tracker = GetComponent<ObjectiveTracker>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
       // _playerVisualManager  = GetComponentInChildren<PlayerVisualManager>();
        
        
        if (!_animator) Debug.LogError("Animator not found on player or its children!");
    }

    void Start()
    {
        _inputManager = FindFirstObjectByType<SharedInputManager>();

        if (!_inputManager)
        {
            Debug.LogError("SharedInputManager not found in the scene.");
            return;
        }

        // Assign actions based on player slot
        switch (playerSlot)
        {
            case PlayerSlot.Player1:
                _moveAction = _inputManager.p1Move;
                _fireAction = _inputManager.p1Fire;
                break;
            case PlayerSlot.Player2:
                _moveAction = _inputManager.p2Move;
                _fireAction = _inputManager.p2Fire;
                break;
        }

        _fireAction.performed += OnFire;


        float angleInRadians = cameraXDegree * Mathf.Deg2Rad;
        yCorrection = 1f / Mathf.Sin(angleInRadians);
    }

    void OnDestroy()
    {
        _fireAction.performed -= OnFire;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.currentState != GameManager.GameState.Gameplay) return;

        Vector2 raw = _moveAction.ReadValue<Vector2>();

        Vector3 moveDirection = new Vector3(raw.y * yCorrection * SpeedYMultiplier, 0, -1 * raw.x);
        if (moveDirection != Vector3.zero)
        {
            _lastFacingDirection = moveDirection.normalized;
        }

        _rb.linearVelocity = moveDirection * (speed * _movementStatus);
        if (!_gotRanOver)
            _animator.SetFloat(Speed1, _rb.linearVelocity.magnitude);

        if (raw.x > 0)
        {
            _spriteRenderer.flipX = true; // If move right, flip the sprite
        }
        else if (raw.x < 0)
        {
            _spriteRenderer.flipX = false; // If move left, keep the sprite
        }
    }

    public void EquipItem(Item newItemPrefab, Sprite sprite)
    {
        itemPrefab = newItemPrefab;
        var textureModule = _particles.textureSheetAnimation;
        textureModule.SetSprite(0, sprite);
        _particles.gameObject.SetActive(true);
        _particles.Play();

        if (_itemExpirationCoroutine != null)
        {
            StopCoroutine(_itemExpirationCoroutine); // עוצרים טיימר קודם אם היה
        }

        if (itemPrefab.HoldTimeLimit > 0)
        {
            _itemExpirationCoroutine = StartCoroutine(ItemExpirationRoutine(itemPrefab.HoldTimeLimit));
        }
    }

    private IEnumerator ItemExpirationRoutine(float timeLimit)
    {
        // מחכים את הזמן שהוגדר
        yield return new WaitForSeconds(timeLimit);

        // --- הזמן נגמר! מנקים לשחקן את היד ---
        if (itemPrefab)
        {
            // 1. מכבים את האפקטים (החלקיקים/ספרייט)
            _particles.Stop();
            _particles.gameObject.SetActive(false);

            // 2. מעדכנים את הויזואל של השחקן (כמו שהשותף שלך עשה באיסוף)
            PlayerVisualManager visuals = GetComponentInChildren<PlayerVisualManager>();
            if (visuals)
            {
                visuals.SetInventoryVisual(itemPrefab.Category, false);
            }

            // 3. מוחקים את החפץ עצמו
            itemPrefab = null;

            Debug.Log("Item expired in hand!");
        }
    }

    public void CollectObjective(Item newObjectivePrefab)
    {
        // Update Objective list
        if (!collectedObjectives.Contains(newObjectivePrefab) && _tracker.OnObjectiveCollected(newObjectivePrefab))
            collectedObjectives.Add(newObjectivePrefab);
    }
    
    public void ClearObjectives()
    {
        collectedObjectives.Clear();
        _tracker.RemoveAllObjectives();
    }

    void OnFire(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.currentState != GameManager.GameState.Gameplay) return;

        if (!itemPrefab || speed == 0) return;

        Vector3 spawnPos = transform.position + (_lastFacingDirection * spawnDistance);

        Item newProjectile = Instantiate(itemPrefab, spawnPos, Quaternion.Euler(0f, 90f, 0f));

        PlayerEffectManager myManager = GetComponent<PlayerEffectManager>();

        newProjectile.Use(_lastFacingDirection, playerSlot, myManager);
        // if (_itemExpirationCoroutine != null)
        // {
        //     StopCoroutine(_itemExpirationCoroutine);
        // }
        //
        // itemPrefab = null;// Empty the hand
        // _particles.gameObject.SetActive(false);
        // if (myManager)
        //     myManager.GetComponentInChildren<PlayerVisualManager>().SetInventoryVisual(newProjectile.Category, false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Check if we hit a vehicle
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            // Get the vehicle component safely
            Vehicle vehicle = collision.GetComponent<Vehicle>();
            if (vehicle)
                // Apply the stun effect based on the vehicle's specific duration
                if (!_gotRanOver)
                    StartCoroutine(HitByVehicle(vehicle.StunDurationWhenHit));
        }
    }

    private IEnumerator HitByVehicle(float stunDuration)
    {
        Debug.Log("HitByVehicle for " + stunDuration + " seconds");
        // Stop movement and trigger the hit animation
        _animator.SetTrigger(GotRanOver);
        _gotRanOver = true;
        _movementStatus = 0;
        ClearObjectives();


        // Wait for the exact duration defined by the vehicle
        yield return new WaitForSeconds(stunDuration);

        // Recover: back to idle animation and restore movement
        Debug.Log("Player recovering. Back to Idle.");
        if(!GotBeatUp)
            _animator.SetTrigger(BackToIdle);
        _movementStatus = 1;

        yield return new WaitForSeconds(hitCoolTime);
        _gotRanOver = false;
    }

    [Tooltip("Teleports the player back to their original spawn position")]
    public void ResetToSpawn()
    {
        // Reset position
        transform.position = _spawnPosition;

        // If the player has a Rigidbody, kill any leftover movement/momentum
        if (_rb)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        if (_animator) _animator.SetBool(GotRanOver, false);
        // ClearObjectives();
    }

    private void StartGame()
    {
        ClearObjectives();
        ResetToSpawn();
    }

    public bool HasAllObjectives()
    {
        if (playerSlot == PlayerSlot.Player2)
            return collectedObjectives.Count >= 1; // Woman
        return collectedObjectives.Count >= 3; // Man
    }

    public void TriggerBeatUpAnimation()
    {
        _rb.linearVelocity = Vector3.zero;
        GotBeatUp = true;
        if (_animator)
        {
            _animator.SetTrigger(BeatUp);
        }
        SoundManager.Instance.PlaySoundFXClip(beatUpSound, transform, 0.5f, 3);
        StartCoroutine(StopBeatUpAnimation());
    }

    private IEnumerator StopBeatUpAnimation()
    {
        yield return new WaitForSeconds(3f); // Assuming the beat up animation is 3 seconds long

        if (_animator)
        {
            _animator.SetBool(BeatUp, false);
            _animator.SetTrigger(BackToIdle);
        }

        GotBeatUp = false;
    }

    void OnEnable()
    {
        Cheats.OnResetPlayersPosition += ResetToSpawn;
        GameManager.OnGameStarted += StartGame;
    }

    void OnDisable()
    {
        Cheats.OnResetPlayersPosition -= ResetToSpawn;
        GameManager.OnGameStarted -= StartGame;
    }
}