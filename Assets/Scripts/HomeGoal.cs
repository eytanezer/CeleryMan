using Managment;
using UnityEngine;

public class HomeGoal : MonoBehaviour
{
    void Start()
    {
        if (!GetComponent<BoxCollider>())
        {
            BoxCollider newCollider = gameObject.AddComponent<BoxCollider>();
            newCollider.isTrigger = true;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player && player.playerSlot == PlayerMovement.PlayerSlot.Player1)// Male
        {
            if (player.HasAllObjectives())
            {
                Debug.Log("Husband Wins!");
                GameManager.Instance.PrepareForEnding();
                GameManager.Instance.EndGame(1);
            }
        }
    }
}