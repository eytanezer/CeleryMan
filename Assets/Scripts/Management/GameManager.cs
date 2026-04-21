using System;
using LaneObjects;
using Management.UI_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managment
{
    public class GameManager : MonoSingleton<GameManager>
    {
        
        public enum GameState
        {
            Title,
            Instructions,
            Gameplay,
            GameOver
        }
        public static event Action OnGameStarted;
        public static event Action OnGameReset;
        public static event Action OnGameOver; 
        public static event Action OnTitleScreen;
        
        [Header("Game Flow State")]
        public GameState currentState = GameState.Title;

        [Header("References")]
        public GameObject titleScreenUI;
        public InstructionTruck instructionTruck;
        
        [Header("Ending Screens")]
        public GameObject endingScreen1; // e.g., Player 1 Wins
        public GameObject endingScreen2;
        
        private TitleScreenFader _titleScreenFaderTS;
        private TitleScreenFader _titleScreenFaderES1;
        private TitleScreenFader _titleScreenFaderES2;

        private void Awake()
        {
            
        }
        
        private void Start()
        {
            _titleScreenFaderTS = titleScreenUI.GetComponent<TitleScreenFader>();
            _titleScreenFaderES1 = endingScreen1.GetComponent<TitleScreenFader>();
            _titleScreenFaderES2 = endingScreen2.GetComponent<TitleScreenFader>();
            
            _titleScreenFaderTS.StartFade(1);
            OnTitleScreen?.Invoke();
        }

        private void OnEnable()
        {
            Cheats.OnResetGame += resetGame;
            Cheats.OnQuit += QuitGame;
       
        }
    
        private void OnDisable()
        {
            Cheats.OnResetGame -= resetGame;
            Cheats.OnQuit -= QuitGame;
        }
    
        public void StartInstructions()
        {
            if (currentState == GameState.Title)
            {
                _titleScreenFaderTS.StartFade(0);
                currentState = GameState.Instructions;
                StartCoroutine(instructionTruck.DriveIn());
            }
        }

        // public void DismissInstructions()
        // {
        //     Debug.Log("Dismiss Instructions");
        //     if (currentState == GameState.Instructions && instructionTruck.IsWaitingForInput)
        //     {
        //         instructionTruck.DriveOut();
        //     }
        // }
        
        public void StartGameplay()
        {
            currentState = GameState.Gameplay;
            Debug.Log("GAME STARTED! Spawners and Players can now move.");
            OnGameStarted?.Invoke();
        }

        // public void EndGame(int endingType)
        // {
        //     if (currentState != GameState.Gameplay) return;
        //
        //     currentState = GameState.GameOver;
        //     Debug.Log("GAME OVER! Showing ending screen: " + endingType);
        //
        //     // Turn on the correct UI based on who won
        //     if (endingType == 1) endingScreen1.SetActive(true);
        //     else if (endingType == 2) endingScreen2.SetActive(true);
        // }
        //
        
        public void PrepareForEnding()
        {
            if (currentState != GameState.Gameplay) return;
        
            currentState = GameState.GameOver;
            Debug.Log("Game frozen for ending sequence.");
        }
        
        public void EndGame(int endingType)
        {
            Debug.Log("GAME OVER! Showing ending screen: " + endingType);
            OnGameOver?.Invoke();
            if (endingType == 1) _titleScreenFaderES1.StartFade(1);
            else if (endingType == 2) _titleScreenFaderES2.StartFade(1);
        }
        
        public void RestartFromEnding()
        {
            if (_titleScreenFaderES1.isActiveAndEnabled) _titleScreenFaderES1.StartFade(0);
            if (_titleScreenFaderES2.isActiveAndEnabled) _titleScreenFaderES2.StartFade(0);

            OnGameReset?.Invoke();
            StartGameplay(); 
        }
    
    
        public void QuitGame()
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor
            // so we use this instead
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // Close the game!
        Application.Quit();
#endif
        }

  


        private void resetGame()
        {
            // Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }
}
