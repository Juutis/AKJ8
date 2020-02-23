using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button continueButton;

    [SerializeField]
    private GameObject canvas;


    [SerializeField]
    private Text messageTxt;
    public void StartButtonClick() {
        DisableAll();
        LevelManager.main.StartGameForReal();
        //LevelManager.main.LoadNextLevel();
    }
    public void ContinueButtonClick() {
        DisableAll();
        LevelManager.main.UnPause();
    }
    public void ExitButtonClick() {
        DisableAll();
        Application.Quit();
        Debug.Log("Exit!");
    }
    public void RestartButtonClick() {
        DisableAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowMessage(string message) {
        messageTxt.text = message;
    }

    public void DisableAll() {
        restartButton.interactable = false;
        exitButton.interactable = false;
        startButton.interactable = false;
        continueButton.interactable = false;
    }

    public void EnableAll() {
        restartButton.interactable = true;
        exitButton.interactable = true;
        startButton.interactable = true;
        continueButton.interactable = true;
    }

    public void ShowDeath(string message) {
        HideAllButtons();
        ShowMessage(message);
        Show();
        ShowButton(restartButton);
        ShowButton(exitButton);
    }

    public void ShowStart(string message) {
        HideAllButtons();
        ShowMessage(message);
        Show();
        ShowButton(startButton);
    }

    public void ShowEnd(string message) {
        HideAllButtons();
        ShowMessage(message);
        Show();
        ShowButton(restartButton);
        ShowButton(exitButton);
    }

    public void ShowPause(string message) {
        HideAllButtons();
        ShowMessage(message);
        Show();
        ShowButton(restartButton);
        ShowButton(continueButton);
        ShowButton(exitButton);
    }

    public void Show() {
        canvas.SetActive(true);
        EnableAll();
    }
    public void Hide() {
        canvas.SetActive(false);
    }

    public void HideAllButtons() {
        HideButton(continueButton);
        HideButton(restartButton);
        HideButton(exitButton);
        HideButton(startButton);
    }

    public void HideButton(Button button) {
        button.gameObject.SetActive(false);
    }

    public void ShowButton(Button button) {
        button.gameObject.SetActive(true);
    }

}
