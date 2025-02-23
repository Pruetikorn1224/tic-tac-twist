using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CubeMap : MonoBehaviour
{

    [Header("Ray Transforms")]
    // 0 - up
    // 1 - down
    // 2 - right
    // 3 - left
    // 4 - front
    // 5 - back
    [SerializeField] List<Transform> tRays;

    [Header("Marks Images")]
    [SerializeField] List<Sprite> markImages;
    [SerializeField] Image targetImage;

    [Header("Cube Map UI")]
    // 0 - up
    // 1 - down
    // 2 - right
    // 3 - left
    // 4 - front
    // 5 - back
    public List<Transform> fSides;

    Sprite originalSprite;

    private int layerMask = 1 << 6;

    // Start is called before the first frame update
    void Start()
    {
        originalSprite = targetImage.sprite;
    }

    private void Update()
    {
        
    }

    /// <summary>
    /// Update a cube map UI
    /// </summary>
    /// <param name="sideIndex">An index or string of face required for an update (0:up, 1:down, 2:right, 3:left, 4:front, 5:back)</param>
    /// <returns>An array of mark indices on the face</returns>
    public List<int> UpdateMap(int sideIndex)
    {
        Transform side = fSides[sideIndex];
        Transform raySide = tRays[sideIndex];

        List<int> face = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0};

        Vector3 rayForward = raySide.forward;
        Vector3 rayOrigin = raySide.position;

        int i = 0;
        for (int a = 1; a > -2; a--)
        {
            for (int b = 1; b > -2; b--)
            {
                Vector3 ray = Vector3.zero;
                if (rayForward == Vector3.forward || rayForward == Vector3.back)
                {
                    int p = rayForward == Vector3.forward ? -1 : 1;
                    ray = new Vector3(b * p, a, rayOrigin.z);
                }
                else if (rayForward == Vector3.right || rayForward == Vector3.left)
                {
                    int p = rayForward == Vector3.right ? 1 : -1;
                    ray = new Vector3(rayOrigin.x, a, b * p);
                }
                else if (rayForward == Vector3.up || rayForward == Vector3.down)
                {
                    int p = rayForward == Vector3.up ? -1 : 1;
                    ray = new Vector3(b * p, rayOrigin.y, -a * p);
                }
                RaycastHit hit;

                if (Physics.Raycast(ray, raySide.forward, out hit, 1, layerMask))
                {
                    Debug.DrawRay(ray, raySide.forward * hit.distance, Color.yellow);
                    //faceHit.Add(hit.collider.gameObject);
                    //Debug.Log(hit.collider.gameObject.name);

                    if (hit.collider.gameObject.name[0] == 'x')
                    {
                        face[i] = 2;
                        side.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = markImages[1];
                    }
                    else if (hit.collider.gameObject.name[0] == 'o')
                    {
                        face[i] = 1;
                        side.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = markImages[0];
                    }

                    
                }
                else
                {
                    Debug.DrawRay(ray, raySide.forward, Color.green);

                    face[i] = 0;
                    side.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = originalSprite;
                }

                i++;
            }
        }

        return face;
    }

    public List<int> UpdateMap(string sideIndex)
    {
        int index = 0;
        switch (sideIndex)
        {
            case "up":
                index = 0;
                break;
            case "down":
                index = 1;
                break;
            case "right":
                index = 2;
                break;
            case "left":
                index = 3;
                break;
            case "front":
                index = 4;
                break;
            case "back":
                index = 5;
                break;
            default:
                Debug.LogError("[CubeMap.cs] Unable to find the side with this input!");
                break;
        }
        Transform side = fSides[index];
        Transform raySide = tRays[index];

        List<int> face = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        Vector3 rayForward = raySide.forward;
        Vector3 rayOrigin = raySide.position;

        int i = 0;
        for (int a = 1; a > -2; a--)
        {
            for (int b = 1; b > -2; b--)
            {
                Vector3 ray = Vector3.zero;
                if (rayForward == Vector3.forward || rayForward == Vector3.back)
                {
                    int p = rayForward == Vector3.forward ? -1 : 1;
                    ray = new Vector3(b * p, a, rayOrigin.z);
                }
                else if (rayForward == Vector3.right || rayForward == Vector3.left)
                {
                    int p = rayForward == Vector3.right ? 1 : -1;
                    ray = new Vector3(rayOrigin.x, a, b * p);
                }
                else if (rayForward == Vector3.up || rayForward == Vector3.down)
                {
                    int p = rayForward == Vector3.up ? -1 : 1;
                    ray = new Vector3(b * p, rayOrigin.y, -a * p);
                }
                RaycastHit hit;

                if (Physics.Raycast(ray, raySide.forward, out hit, 1, layerMask))
                {
                    Debug.DrawRay(ray, raySide.forward * hit.distance, Color.yellow);
                    //faceHit.Add(hit.collider.gameObject);
                    //Debug.Log(hit.collider.gameObject.name);

                    if (hit.collider.gameObject.name[0] == 'x')
                    {
                        face[i] = 2;
                        side.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = markImages[1];
                    }
                    else if (hit.collider.gameObject.name[0] == 'o')
                    {
                        face[i] = 1;
                        side.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = markImages[0];
                    }
                }
                else
                {
                    Debug.DrawRay(ray, raySide.forward, Color.green);

                    face[i] = 0;
                    side.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = originalSprite;
                }

                i++;
            }
        }

        return face;
    }
}
