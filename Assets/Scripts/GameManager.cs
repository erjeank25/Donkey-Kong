using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int lives; // Vidas
    private int score; // Puntuacion

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        NewGame();
    }

    private void NewGame()
    {
        lives = 3; // Vidas iniciales
        score = 0; // Marcador inicial
        Time.timeScale = 1f; // Descongela el juego

        RechargeLevel(1);
    }

    private void RechargeLevel(int level)
    {
        Camera camera = Camera.main;

        if (camera != null)
        {
            camera.cullingMask = 0;
        }

        Invoke(nameof(LoadScene), 1f);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(1);
    }

    public void GameComplete()
    {
        score += 1000; // Si se completa, se suman estos puntos
        Time.timeScale = 0f; // Congela el juego
    }

    public void GameFailed()
    {
        lives--; // Si muere, se le resta una vida

        if (lives <= 0)
        {
            NewGame();
        }
        else
        {
            // Recargar nivel
            RechargeLevel(0);           
        }
    }
}