using System;
using UnityEngine;

public class PlaceMarker : MonoBehaviour
{

    [Header("Players' Marks")]
    [SerializeField] GameObject[] playerMarks;

    [Header("Checker Info")]
    [SerializeField] GameObject checkerObject;
    [SerializeField] Material[] checkerMaterials;

    [Header("Sound Effects")]
    [SerializeField] AudioSource placeSound;
    [SerializeField] AudioSource errorSound;

    GameEvent gameEventScript;

    GameObject currentChecker;

    Vector3 checkerPosition;

    Quaternion checkerRotation;

    public bool isSuccessPlay;

    private int markerIndex;

    private int cubeLayer = 1 << 7;

    private int markerLayer = 1 << 6;

    // Start is called before the first frame update
    void Start()
    {
        isSuccessPlay = false;

        gameEventScript = gameObject.GetComponent<GameEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        placeSound.volume = gameEventScript.soundVolume / 100f;
        errorSound.volume = gameEventScript.soundVolume / 100f;

        if (gameEventScript.gameMode == 0 && gameEventScript.gameState == 1)
        {
            markerIndex = gameEventScript.currentPlayer;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, cubeLayer))
            {
                Vector3 hitNormal = hit.normal;

                checkerPosition = gameObject.transform.position + 0.51f * hitNormal;
                checkerRotation = Quaternion.LookRotation(hitNormal) * Quaternion.AngleAxis(-90, Vector3.left);


                if (hit.collider.gameObject.tag == "Cube")
                {
                    GameObject cube = hit.collider.gameObject;

                    checkerPosition = cube.transform.position + 0.51f * hitNormal;
                    checkerRotation = Quaternion.LookRotation(hitNormal) * Quaternion.AngleAxis(-90, Vector3.left);

                    if (currentChecker == null)
                    {
                        currentChecker = Instantiate(checkerObject, checkerPosition, checkerRotation);
                    }
                    else
                    {
                        currentChecker.transform.position = checkerPosition;
                        currentChecker.transform.rotation = checkerRotation;
                    }

                    Vector3 newOrigin = hit.point + ray.direction * 0.01f;
                    RaycastHit secondHit;
                    if (Physics.Raycast(newOrigin, -ray.direction, out secondHit, 1, markerLayer))
                    {
                        currentChecker.GetComponent<MeshRenderer>().material = checkerMaterials[1];

                        if (Input.GetMouseButtonDown(0))
                            errorSound.Play();

                        return;
                    }
                    else
                    {
                        currentChecker.GetComponent<MeshRenderer>().material = checkerMaterials[0];
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 markPosition = cube.transform.position + 0.55f * hitNormal;
                        Quaternion markRotation = Quaternion.LookRotation(hitNormal);

                        GameObject playerMark = Instantiate(playerMarks[markerIndex], markPosition, markRotation);
                        if (markerIndex == 1) playerMark.transform.Rotate(0, 0, 45f, Space.Self);
                        playerMark.transform.SetParent(cube.transform);

                        cube.GetComponent<Cube>().marks.Add(playerMark);

                        placeSound.Play();

                        isSuccessPlay = true;
                    }
                }
            }

            else
            {
                if (currentChecker != null)
                    Destroy(currentChecker);
            }
        }
        else
        {
            if (currentChecker != null)
                Destroy(currentChecker);
        }
    }
}

