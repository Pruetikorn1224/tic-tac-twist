using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{

    [SerializeField] GameEvent gameEventScript;

    [SerializeField] int maximumMark;

    [SerializeField] Material blackMaterial;

    [SerializeField] GameObject checker;

    [SerializeField] List<Material> checkerMaterials;

    public List<GameObject> marks = new List<GameObject>();

    private Material originalMaterial;

    private void Start()
    {
        //gameObject.layer = 1 << 7;
        originalMaterial = gameObject.GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        if (marks.Count == maximumMark)
        {
            gameObject.GetComponent<MeshRenderer>().material = blackMaterial;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = originalMaterial;
        }
    }
}
