using System.Collections;
using Management.SoundScripts;
using Managment;
using ManagmentScripts.SoundScripts;
using UnityEngine;

namespace LaneObjects
{
    public class InstructionTruck : MonoBehaviour
    {
        [Header("Positions")]
        [SerializeField] private Transform startPoint;   // Off-screen Right
        [SerializeField] private Transform centerPoint;  // Middle of the screen
        [SerializeField] private Transform endPoint;     // Off-screen Left
        
        [SerializeField][Tooltip("Time to wait at the center before allowing dismissal")] private float waitTimeAtCenter = 30f;
        [SerializeField] private float driveSpeed = 10f;
    
        [Header("Audio")]
        public AudioClip truckEngineSound; // הסאונד של המשאית
        private AudioSourcePoolable _currentAudio;
        
        
        //gameManager will read this to know when to listen for the second movement
        public bool IsWaitingForInput { get; private set; }

        void Start()
        {
            transform.position = startPoint.position;
        }

        public IEnumerator DriveIn()
        {
            if (truckEngineSound != null && SoundManager.Instance != null)
            {
                _currentAudio = SoundManager.Instance.PlayLoopingSoundFX(truckEngineSound, transform, 1f);
            }
            
            StartCoroutine(DriveToPosition(centerPoint.position, true));
            
            if (_currentAudio != null) _currentAudio.Source.volume = 1f; // lower volume while waiting
            yield return new WaitForSeconds(waitTimeAtCenter);
            if (_currentAudio != null) _currentAudio.Source.volume = 1f;
            StartCoroutine(DriveToPosition(endPoint.position, false));
        }

        // public void DriveOut()
        // {
        //     StartCoroutine(DriveToPosition(endPoint.position, false));
        // }

        private IEnumerator DriveToPosition(Vector3 targetPosition, bool waitAfterReaching)
        {
            IsWaitingForInput = false;
            
            // drive towards the target until we are very close
            while (Vector3.Distance(transform.position, targetPosition) >1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, driveSpeed * Time.deltaTime);
                yield return null;
            }
            
            transform.position = targetPosition; // snap perfectly to the target
        
            IsWaitingForInput = waitAfterReaching;

            if (!waitAfterReaching)
            {
                StopAndReturnSound();
                GameManager.Instance.StartGameplay();
            }
        }
        
        private void StopAndReturnSound()
        {
            if (_currentAudio != null)
            {
                AudioPool.Instance.Return(_currentAudio);
                _currentAudio = null;
            }
        }
        
        private void OnDisable()
        {
            StopAndReturnSound();
        }
    }
}