using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour
{
    [Header("Game Parameters")]
    public int gameState;
    public int gameMode;
    public int currentPlayer;
    public int currentTurn;
    public List<int> playerWin;
    public int winnerPlayer;
    private int prevGameState;

    [Header("Game Setting")]
    public float sensitivity;
    public float soundVolume;
    public bool controlTypeMouse;
    public int limit;

    [Header("Game Objects")]
    public GameObject _rubik;

    [Header("Sound Effects")]
    [SerializeField] AudioSource winSound;
    [SerializeField] AudioSource drawSound;
    [SerializeField] AudioSource backgroundMusic;

    PlaceMarker placeMarkerScript;
    RotateCubes rotateCubeScript;
    CubeMap cubeMap;

    // Start is called before the first frame update
    void Start()
    {
        gameState = 0;
        prevGameState = 0;

        gameMode = 0;

        currentPlayer = 0;

        currentTurn = 0;

        controlTypeMouse = true;

        playerWin = new List<int>{ 0, 0 };

        placeMarkerScript = FindObjectOfType<PlaceMarker>();
        rotateCubeScript = FindObjectOfType<RotateCubes>();
        cubeMap = FindObjectOfType<CubeMap>();

        backgroundMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            // Start menu scene
            case 0:
                _rubik.transform.Rotate(0.05f, -0.02f, 0.03f, Space.World);

                if ((prevGameState == 3 || prevGameState == 2) && gameState == 0)
                {
                    backgroundMusic.Play();
                    RemoveXOChildren(_rubik.transform);

                    playerWin = new List<int> { 0, 0 };

                    currentTurn = 0;
                }

                prevGameState = gameState;
                break;

            // Gameplay scene
            case 1:
                if (prevGameState == 0 && gameState == 1)
                {
                    _rubik.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                    List<GameObject> childrenToDestroy = new List<GameObject>();

                    foreach (Transform child in _rubik.transform)
                    {
                        if (child.name.StartsWith("x") || child.name.StartsWith("o"))
                        {
                            childrenToDestroy.Add(child.gameObject);
                        }
                    }

                    foreach (GameObject child in childrenToDestroy)
                    {
                        Destroy(child);
                    }

                    backgroundMusic.Stop();

                    prevGameState = gameState;
                }
                else if ((prevGameState == 2 || prevGameState == 3) && gameState == 1)
                {
                    RemoveXOChildren(_rubik.transform);
                    prevGameState = gameState;
                }
                    

                if (placeMarkerScript.isSuccessPlay || rotateCubeScript.isSuccessPlay)
                {

                    placeMarkerScript.isSuccessPlay = false;
                    rotateCubeScript.isSuccessPlay = false;

                    for (int i = 0; i < 6; i++)
                    {
                        List<int> board = cubeMap.UpdateMap(i);
                        winnerPlayer = CheckWinner(board);

                        if (winnerPlayer != 0)
                        {
                            playerWin[winnerPlayer - 1]++;
                            gameState = 2;
                            return;
                        }
                    }

                    currentTurn++;
                    currentPlayer = (currentPlayer + 1) % 2;
                    gameMode = 0;
                }

                if (currentTurn == limit)
                {
                    gameState = 3;
                    return;
                }
                break;

            // Gameover (player won) scene
            case 2:
                if (prevGameState != gameState)
                    winSound.Play();

                prevGameState = gameState;
                currentTurn = 0;
                break;

            // Gameover (draw) scene
            case 3:
                if (prevGameState != gameState)
                    drawSound.Play();

                prevGameState = gameState;
                currentTurn = 0;
                break;

            // Setting scene
            case 4:
                prevGameState = gameState;
                break;

            default:
                Debug.LogError("[GameEvent.cs] Undefined Game State!");
                break;
        }

        backgroundMusic.volume = soundVolume / 100f;
    }

    private int CheckWinner(List<int> board)
    {
        int[][] winningCombinations = new int[][]
        {
            new int[] { 0, 1, 2 }, // Row 1
            new int[] { 3, 4, 5 }, // Row 2
            new int[] { 6, 7, 8 }, // Row 3
            new int[] { 0, 3, 6 }, // Column 1
            new int[] { 1, 4, 7 }, // Column 2
            new int[] { 2, 5, 8 }, // Column 3
            new int[] { 0, 4, 8 }, // Diagonal 1
            new int[] { 2, 4, 6 }  // Diagonal 2
        };

        foreach (var combo in winningCombinations)
        {
            int a = board[combo[0]], b = board[combo[1]], c = board[combo[2]];

            if (a != 0 && a == b && b == c)
            {
                return a;
            }
        }

        return 0;
    }

    private void RemoveXOChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // If the child is named "x" or "o", destroy it
            if (child.name.StartsWith("x") || child.name.StartsWith("o"))
            {
                Destroy(child.gameObject);
            }
            else
            {
                if (child.GetComponent<Cube>() != null)
                    child.GetComponent<Cube>().marks = new List<GameObject>();

                RemoveXOChildren(child);
            }
        }
    }
}
