using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button settingButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button returnButton;
    [SerializeField] Button openCubeMap;
    [SerializeField] Button closeCubeMap;
    [SerializeField] Button playerMarker;
    [SerializeField] Button playerRotate;
    [SerializeField] Button settingBackButton;
    [SerializeField] Button settingRulesButton;
    [SerializeField] Button settingContactButton;
    [SerializeField] Button settingQuitButton;
    [SerializeField] Button rulesBackButton;
    [SerializeField] Button rulesHomeButton;
    [SerializeField] Button contactBackButton;
    [SerializeField] Button contactHomeButton;
    [SerializeField] Button pauseButton;

    [Header("Rotation Mode Buttons")]
    [SerializeField] Button mouseDragButton;
    [SerializeField] Button arrowButton;

    [Header("UI Panels")]
    [SerializeField] GameObject startingScene;
    [SerializeField] GameObject cubeMap;
    [SerializeField] GameObject playerChoice;
    [SerializeField] GameObject endingScene;
    [SerializeField] GameObject point;

    [Header("Setting Scenes")]
    [SerializeField] private List<GameObject> settingScenes;

    [Header("Texts")]
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI playerChoiceGuide;
    public TextMeshProUGUI playerScore;

    [Header("Slider")]
    [SerializeField] private Slider soundSlider = null;
    [SerializeField] private TextMeshProUGUI soundSliderText = null;
    [SerializeField] private Slider sensitivitySlider = null;
    [SerializeField] private TextMeshProUGUI sensitivitySliderText = null;

    [Header("Input Field")]
    [SerializeField] private TMP_InputField limitInput = null;

    [Header("Images")]
    [SerializeField] List<Sprite> markImages;

    [Header("Sound Effects")]
    [SerializeField] AudioSource uiSound;
    [SerializeField] AudioSource modeSound;

    GameEvent gameEventScript;

    int cPlayer;
    int pPlayer;

    ColorBlock colorBlock;
    ColorBlock cb;

    private void Start()
    {
        gameEventScript = FindAnyObjectByType<GameEvent>();

        startButton.onClick.AddListener(OnGameStart);
        restartButton.onClick.AddListener(OnGameRestart);
        returnButton.onClick.AddListener(OnReturnHome);

        quitButton.onClick.AddListener(QuitGame);

        closeCubeMap.onClick.AddListener(OnCloseCubeMap);
        openCubeMap.onClick.AddListener(OnOpenCubeMap);

        playerMarker.onClick.AddListener(delegate { ChangeGameMode(0); });
        playerRotate.onClick.AddListener(delegate { ChangeGameMode(1); });

        sensitivitySlider.onValueChanged.AddListener(SensitivitySlider);
        soundSlider.onValueChanged.AddListener(SoundSlider);

        limitInput.onEndEdit.AddListener(OnLimitChange);
        limitInput.text = "50";

        settingButton.onClick.AddListener(delegate { SceneTransition(0, 1); });
        settingBackButton.onClick.AddListener(delegate { SceneTransition(1, 0); });
        settingRulesButton.onClick.AddListener(delegate { SceneTransition(1, 2); });
        settingContactButton.onClick.AddListener(delegate { SceneTransition(1, 3); });
        rulesBackButton.onClick.AddListener(delegate { SceneTransition(2, 1); });
        rulesHomeButton.onClick.AddListener(delegate { SceneTransition(2, 0); });
        contactBackButton.onClick.AddListener(delegate { SceneTransition(3, 1); });
        contactHomeButton.onClick.AddListener(delegate { SceneTransition(3, 0); });
        pauseButton.onClick.AddListener(delegate { SceneTransition(0, 1); gameEventScript.gameState = 4; });
        settingQuitButton.onClick.AddListener(QuitGame);

        mouseDragButton.onClick.AddListener(delegate { RotationMode(0); });
        arrowButton.onClick.AddListener(delegate { RotationMode(1); });

        startingScene.SetActive(true);
        openCubeMap.gameObject.SetActive(false);
        cubeMap.SetActive(false);
        point.SetActive(false);
        endingScene.SetActive(false);

        playerChoice.SetActive(false);

        for (int i = 1; i < settingScenes.Count; i++)
        {
            settingScenes[i].SetActive(false);
        }

        cPlayer = gameEventScript.currentPlayer;
        pPlayer = cPlayer;

        colorBlock = playerMarker.colors;
        colorBlock.normalColor = Color.white;
        playerMarker.colors = colorBlock;

        colorBlock = playerRotate.colors;
        colorBlock.normalColor = Color.gray;
        playerRotate.colors = colorBlock;

        cb = mouseDragButton.colors;
        cb.normalColor = new Color(72f, 72f, 0f);
        mouseDragButton.colors = cb;

        cb = mouseDragButton.colors;
        cb.normalColor = Color.white;
        arrowButton.colors = cb;
    }

    private void Update()
    {
        cPlayer = gameEventScript.currentPlayer;
        if (pPlayer != cPlayer)
        {
            playerMarker.gameObject.GetComponent<Image>().sprite = markImages[cPlayer];
            pPlayer = cPlayer;

            colorBlock = playerMarker.colors;
            colorBlock.normalColor = Color.white;
            playerMarker.colors = colorBlock;

            colorBlock = playerRotate.colors;
            colorBlock.normalColor = Color.gray;
            playerRotate.colors = colorBlock;

            playerChoiceGuide.text = "Choose any available face (green) to place marker";
        }

        if (gameEventScript.gameState == 2)
        {
            endingScene.SetActive(true);
            point.SetActive(false);
            playerChoice.SetActive(false);
            cubeMap.SetActive(false);
            openCubeMap.gameObject.SetActive(false);
            winnerText.text = "Player " + gameEventScript.winnerPlayer.ToString() + " won!!";
        }

        else if (gameEventScript.gameState == 3)
        {
            endingScene.SetActive(true);
            point.SetActive(false);
            playerChoice.SetActive(false);
            cubeMap.SetActive(false);
            openCubeMap.gameObject.SetActive(false);
            winnerText.text = "Draw!!\nLimit End";
        }

        else if (gameEventScript.gameState == 4)
        {
            playerChoice.SetActive(false);
            point.SetActive(false);
            cubeMap.SetActive(false);
            openCubeMap.gameObject.SetActive(false);
        }

        playerScore.text = gameEventScript.playerWin[0].ToString() + " : " + gameEventScript.playerWin[1].ToString();

        uiSound.volume = gameEventScript.soundVolume / 100f;
        modeSound.volume = gameEventScript.soundVolume / 100f;
    }

    private void OnGameStart()
    {
        playerChoice.SetActive(true);
        openCubeMap.gameObject.SetActive(true);
        point.SetActive(true);

        gameEventScript.gameState = 1;

        uiSound.Play();

        startingScene.SetActive(false);
    }

    private void OnReturnHome()
    {
        endingScene.SetActive(false);
        startingScene.SetActive(true);

        gameEventScript.gameState = 0;
    }

    private void OnOpenCubeMap()
    {
        if (!cubeMap.activeSelf)
            cubeMap.SetActive(true);

        if (openCubeMap.IsActive())
            openCubeMap.gameObject.SetActive(false);

        uiSound.Play();
    }

    private void OnCloseCubeMap()
    {
        if (!openCubeMap.IsActive())
            openCubeMap.gameObject.SetActive(true);

        if (cubeMap.activeSelf)
            cubeMap.SetActive(false);

        uiSound.Play();
    }

    private void ChangeGameMode(int gm)
    {
        gameEventScript.gameMode = gm;

        if (gm == 0)
        {
            colorBlock = playerMarker.colors;
            colorBlock.normalColor = Color.white;
            playerMarker.colors = colorBlock;

            colorBlock = playerRotate.colors;
            colorBlock.normalColor = Color.gray;
            playerRotate.colors = colorBlock;

            playerChoiceGuide.text = "Choose any available face (green) to place marker";
        }

        else if (gm == 1)
        {
            colorBlock = playerMarker.colors;
            colorBlock.normalColor = Color.gray;
            playerMarker.colors = colorBlock;

            colorBlock = playerRotate.colors;
            colorBlock.normalColor = Color.white;
            playerRotate.colors = colorBlock;

            playerChoiceGuide.text = "Rotate a row of cube";
        }

        modeSound.Play();
    }

    private void OnGameRestart()
    {
        gameEventScript.gameState = 1;
        gameEventScript.gameMode = 0;
        gameEventScript.currentPlayer = 0;

        endingScene.SetActive(false);
        playerChoice.SetActive(true);
        cubeMap.SetActive(false);
        openCubeMap.gameObject.SetActive(true);
        point.SetActive(true);

        uiSound.Play();
    }

    private void SoundSlider(float value)
    {
        soundSliderText.text = value.ToString("0");
        gameEventScript.soundVolume = value;

        uiSound.Play();
    }

    private void SensitivitySlider(float value)
    {
        sensitivitySliderText.text = value.ToString("0");
        gameEventScript.sensitivity = value;

        uiSound.Play();
    }

    private void SceneTransition(int startSceneIndex, int endSceneIndex)
    {
        if (gameEventScript.gameState == 4 && endSceneIndex == 0)
        {
            foreach (GameObject settingScene in settingScenes)
            {
                settingScene.SetActive(false);
            }
            gameEventScript.gameState = 1;

            playerChoice.SetActive(true);
            openCubeMap.gameObject.SetActive(true);
            point.SetActive(true);
        }
        else
        {
            settingScenes[endSceneIndex].SetActive(true);
            settingScenes[startSceneIndex].SetActive(false);
        }

        uiSound.Play();
    }

    private void RotationMode(int mode)
    {
        if (mode == 0)
        {
            mouseDragButton.enabled = false;
            cb = mouseDragButton.colors;
            cb.normalColor = new Color(72f, 72f, 0f);
            mouseDragButton.colors = cb;

            arrowButton.enabled = true;
            cb = mouseDragButton.colors;
            cb.normalColor = Color.white;
            arrowButton.colors = cb;

            gameEventScript.controlTypeMouse = true;
        }
        else if (mode == 1)
        {
            mouseDragButton.enabled = true;
            cb = mouseDragButton.colors;
            cb.normalColor = Color.white;
            mouseDragButton.colors = cb;

            arrowButton.enabled = false;
            cb = mouseDragButton.colors;
            cb.normalColor = new Color(72f, 72f, 0f);
            arrowButton.colors = cb;

            gameEventScript.controlTypeMouse = false;
        }

        uiSound.Play();
    }

    private void OnLimitChange(string value)
    {
        int limit;
        bool isNumber = int.TryParse(value, out limit);

        if (isNumber)
        {
            if (limit % 2 == 0)
                gameEventScript.limit = limit;
            else
            {
                limit++;
                gameEventScript.limit = limit;
                limitInput.text = gameEventScript.limit.ToString();
            }
        }
        else
        {
            limitInput.text = gameEventScript.limit.ToString();
        }

        uiSound.Play();
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
