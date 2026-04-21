using System.Collections;
using System.Collections.Generic;
using Managment;
using UnityEngine;
using UnityEngine.Events;

public class CatchMechanic : MonoBehaviour
{
    [Header("Target Settings")] [Tooltip("The tag of the target character (e.g., Husband)")] [SerializeField]
    private string targetTag = "Husband";

    [Header("Game Over Events")] [Tooltip("Events to trigger when the target is caught AND the wife has her objective")]
    public UnityEvent onCatch;

    private PlayerMovement _myPlayer;

    private void Awake()
    {
        _myPlayer = GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check if hit male
        if (other.CompareTag(targetTag))
        {
            PlayerMovement targetPlayer = other.GetComponent<PlayerMovement>();

            if (_myPlayer.HasAllObjectives()) // If female has all objectives
            {
                StartCoroutine(TriggerGameOver(targetPlayer)); // Game over
            }
            else // If not, respawn
            {
                Debug.Log("Wife caught the husband but doesn't have the objective! Resetting positions.");
                _myPlayer.ResetToSpawn();
                targetPlayer.ResetToSpawn();
            }
        }
    }

    private IEnumerator TriggerGameOver(PlayerMovement targetPlayer)
    {
        Debug.Log("Target Caught with Objective! Game Over. Wife Wins.");
        //onCatch.Invoke();
        GameManager.Instance.PrepareForEnding();
        _myPlayer.TriggerBeatUpAnimation();
        targetPlayer.TriggerBeatUpAnimation();
        targetPlayer.transform.position = _myPlayer.transform.position;
        yield return new WaitForSeconds(3f); 
        _myPlayer.ResetToSpawn();
        targetPlayer.ResetToSpawn();
        GameManager.Instance.EndGame(2);
    }
}