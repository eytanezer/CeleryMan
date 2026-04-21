using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// used for dual control for the two players
/// </summary>
public class SharedInputManager : MonoBehaviour
{
    [Header("Assign your Input Actions Asset here")]
    public InputActionAsset actions;

    // Expose action references for each player
    [HideInInspector] public InputAction p1Move, p1Fire;
    [HideInInspector] public InputAction p2Move, p2Fire;

    void Awake()
    {
        var map = actions.FindActionMap("Gameplay", throwIfNotFound: true);

        // Player 1 — WASD scheme bindings
        p1Move = map.FindAction("P1_Move", throwIfNotFound: true);
        p1Fire = map.FindAction("P1_Fire", throwIfNotFound: true);

        // Player 2 — Arrow key scheme bindings
        p2Move = map.FindAction("P2_Move", throwIfNotFound: true);
        p2Fire = map.FindAction("P2_Fire", throwIfNotFound: true);

        // Enable all — no device pairing needed
        p1Move.Enable(); p1Fire.Enable();
        p2Move.Enable(); p2Fire.Enable();
    }

    void OnDestroy()
    {
        p1Move.Disable(); p1Fire.Disable();
        p2Move.Disable(); p2Fire.Disable();
    }
}