using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using StarterAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    [SerializeField] private TMP_Text coinsText;

    [Header("Pause")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private FirstPersonController fpController;
    [SerializeField] private ActionManager actionController;

    [Header("Win")]
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private PlayerInput playerInput;

    [Header("Inspector")]
    [SerializeField] private GameObject inspectorRoot;
    [SerializeField] private GameObject inspectorPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private Image iconImage;

    private bool isPaused = false;
    private bool endGameActive = false;
    private bool _inspecting;

    private void Awake()
    {
        Instance = this;
        if (inspectorRoot != null) inspectorRoot.SetActive(false);
        if (inspectorPanel != null) inspectorPanel.SetActive(true);
    }

    IEnumerator Start()
    {
        while (ProgressManager.Instance == null)
            yield return null;

        ProgressManager.Instance.OnCoinsChanged += UpdateCoins;
        UpdateCoins(ProgressManager.Instance.Coins);
    }

    void OnDisable()
    {
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.OnCoinsChanged -= UpdateCoins;
    }

    void Update()
    {
        if (endGameActive) return;

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if(endGameActive) return;

        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        if (isPaused)
        {
            Time.timeScale = 0f;
            fpController.enabled = false;
            actionController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            fpController.enabled = true;
            actionController.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void UpdateCoins(int coins)
    {
        if (coinsText != null)
            coinsText.text = "Fire Balls: " + coins;
    }

    public void TriggerEndGame()
    {
        if (endGameActive) return;
        endGameActive = true;

        isPaused = false;
        if (pauseMenu != null) pauseMenu.SetActive(false);

        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        if (winCanvas != null)
        {
            winCanvas.SetActive(true);

            var cg = winCanvas.GetComponentInChildren<CanvasGroup>(true);
            if (cg != null) cg.alpha = 1f;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return null;

        if (playerInput != null) playerInput.enabled = false;

        if (fpController != null) fpController.enabled = false;
        if (actionController != null) actionController.enabled = false;

        AudioManager.Instance.PlayVictoryMusic();

        Time.timeScale = 0f;
    }

    public bool IsInspecting => _inspecting;

    public void OpenInspector(CollectableInfo data)
    {
        if (data == null) return;
        _inspecting = true;
        if (inspectorRoot != null) inspectorRoot.SetActive(true);
        if (inspectorPanel != null) inspectorPanel.SetActive(true);

        if (titleText != null) titleText.text = data.displayName;
        if (descText != null) descText.text = data.description;
        if (iconImage != null)
        {
            iconImage.enabled = data.icon != null;
            iconImage.sprite = data.icon;
        }

        if (fpController != null) fpController.enabled = false;
        //if (actionController != null) actionController.enabled = false;
    }

    public void CloseInspector()
    {
        _inspecting = false;
        if (inspectorRoot != null) inspectorRoot.SetActive(false);
        if (fpController != null) fpController.enabled = true;
        //if (actionController != null) actionController.enabled = true;
    }

    public void ToggleInspector(CollectableInfo data)
    {
        if (_inspecting) CloseInspector();
        else OpenInspector(data);
    }

    public void LoadLevel(string levelName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelName);
    }

    public void QuitLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
