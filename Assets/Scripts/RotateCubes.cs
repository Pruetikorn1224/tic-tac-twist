using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCubes : MonoBehaviour
{

    [SerializeField] AudioSource rotateSound;

    [SerializeField] Transform pivot;

    public bool isSuccessPlay;

    GameEvent gameEventScript;

    Camera mainCamera;

    GameObject rubik;

    GameObject selectedCube;

    List<GameObject> selectedRow = new List<GameObject>();

    Vector2 initialPos;
    Vector2 finalPos;

    Vector3 hitNormal;

    bool isDragging;

    private int cubeLayer = 1 << 7;

    private void Start()
    {
        gameEventScript = gameObject.GetComponent<GameEvent>();
        mainCamera = FindObjectOfType<Camera>();
        rubik = GameObject.Find("Rubik");

        isSuccessPlay = false;
    }

    private void Update()
    {
        if (gameEventScript.gameState == 1 && gameEventScript.gameMode == 1)
        {
            if (Input.GetMouseButtonDown(0) && !isDragging)
            {
                StartDragging();
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                ReleaseDragging();
            }
        }

        rotateSound.volume = gameEventScript.soundVolume / 100f;
    }

    private void StartDragging()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cubeLayer))
        {
            if (hit.collider.gameObject.tag == "Cube")
            {
                selectedCube = hit.collider.gameObject;

                hitNormal = hit.normal;

                initialPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                isDragging = true;
            }
        }
    }

    private void ReleaseDragging()
    {
        finalPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        Vector3 dragVector = ScreenToWorldDragVector(finalPos - initialPos);
        Vector3 direction = FindDirection(hitNormal, dragVector);

        //Debug.Log("Hit" + hitNormal.ToString());
        //Debug.Log("Direction" + direction.ToString());
        RotateCubeRow(selectedCube.transform.position, hitNormal, direction);

        isDragging = false;
    }

    Vector3 ScreenToWorldDragVector(Vector2 screenDragVector)
    {
        Vector3 cameraRight = mainCamera.transform.right;
        Vector3 cameraUp = mainCamera.transform.up;

        Vector3 worldDragVector = (screenDragVector.x * cameraRight) + (screenDragVector.y * cameraUp);

        return worldDragVector;
    }

    Vector3 FindDirection(Vector3 normal, Vector3 inputVector)
    {
        Vector3 closestDirection = Vector3.zero;

        Vector3[] validDirections;

        if (normal == Vector3.forward || normal == Vector3.back)
        {
            validDirections = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down };
        }
        else if (normal == Vector3.up || normal == Vector3.down)
        {
            validDirections = new Vector3[] { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
        }
        else if (normal == Vector3.right || normal == Vector3.left)
        {
            validDirections = new Vector3[] { Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        }
        else
        {
            Debug.LogError("[RotateCubes.cs] Normal is not aligned with world axes!");
            return Vector3.zero;
        }

        Vector3 projectedVector = Vector3.ProjectOnPlane(inputVector, normal);
        float maxDotProduct = -Mathf.Infinity;

        foreach (Vector3 direction in validDirections)
        {
            float dotProduct = Vector3.Dot(projectedVector.normalized, direction);
            if (dotProduct > maxDotProduct)
            {
                maxDotProduct = dotProduct;
                closestDirection = direction;
            }
        }

        return closestDirection;
    }

    private void RotateCubeRow(Vector3 cubePosition, Vector3 hitNormal, Vector3 direction)
    {

        float dotProduct = Vector3.Dot(hitNormal.normalized, direction.normalized);
        if (Mathf.Abs(dotProduct) > 0.0001f)
        {
            Debug.Log("hit: " + hitNormal.ToString() + "\ndirection: " + direction.ToString());
            Debug.LogError("[RotateCubes.cs] Hit normal and direction vectors should not be aligned!");
        }


        Vector3 cross = Vector3.Cross(hitNormal, direction).normalized;

        Vector3 center = new Vector3(Mathf.Abs(cross.x) * cubePosition.x, Mathf.Abs(cross.y) * cubePosition.y, Mathf.Abs(cross.z) * cubePosition.z);
        Vector3 extents = new Vector3(1f, 1f, 1f) - new Vector3(Mathf.Abs(cross.x), Mathf.Abs(cross.y), Mathf.Abs(cross.z)) * 0.9f;

        Collider[] hits = Physics.OverlapBox(center, extents);
        if (hits.Length == 9)
        {
            for (int i = 0; i < 9; i++)
            {
                selectedRow.Add(hits[i].gameObject);
                hits[i].gameObject.transform.SetParent(pivot);
            }
            StartCoroutine(RotateOverTime(cross, 0.5f));
        }
        else
        {
            Debug.Log("center: " + center.ToString() + "\nextent: " + extents.ToString() + "\nLenght: " + hits.Length.ToString());
            Debug.LogError("[RotateCubes.cs] There should be 9 detected cubes only!");
        }
    }

    private IEnumerator RotateOverTime(Vector3 byAxis, float duration)
    {

        Quaternion initialRotation = pivot.rotation;
        Quaternion targetRotation = Quaternion.AngleAxis(90, byAxis);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            pivot.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
            
        }
        pivot.rotation = targetRotation;
        rotateSound.Play();

        foreach (GameObject rotatedCube in selectedRow)
        {
            rotatedCube.transform.SetParent(rubik.transform);
        }

        selectedRow = new List<GameObject>();
        isSuccessPlay = true;
        pivot.transform.rotation = Quaternion.identity;
    }
}
