using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using LocalPlayer;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscMenuController : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO playerStats;
    [Space]
    [SerializeField] private GameObject content;
    [Space]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button closeButton;
    
    private bool _escMenuOpen = false;

    private void Awake()
    {
        OnSensitivityChanged(5);
        
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        quitButton.onClick.AddListener(ReturnToMainMenu);
        closeButton.onClick.AddListener(CloseEscMenu);
    }

    private void OnDestroy()
    {
        sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
        quitButton.onClick.RemoveListener(ReturnToMainMenu);
        closeButton.onClick.RemoveListener(CloseEscMenu);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            ToggleEscMenu();
    }

    private void OnSensitivityChanged(float value)
    {
        playerStats.MouseSensitivity = new Vector2(value, value);
    }
    
    private void ToggleEscMenu()
    {
        _escMenuOpen = !_escMenuOpen;
        
        content.SetActive(_escMenuOpen);

        Cursor.visible = _escMenuOpen;
        Cursor.lockState = _escMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private async void ReturnToMainMenu()
    {
        quitButton.interactable = false;
        closeButton.interactable = false;

        // Shutdown the NetworkRunner
        if (NetworkManager.Instance?.NetworkRunner != null)
        {
            await NetworkManager.Instance.NetworkRunner.Shutdown();
        }

        // Clean up the NetworkManager instance
        if (NetworkManager.Instance != null)
        {
            Destroy(NetworkManager.Instance.gameObject);
        }

        // Load the main menu scene

        await Task.Delay(100);
        SceneManager.LoadScene("Menu");
        Resources.UnloadUnusedAssets();
    }

    private void CloseEscMenu()
    {
        ToggleEscMenu();
    }
}
