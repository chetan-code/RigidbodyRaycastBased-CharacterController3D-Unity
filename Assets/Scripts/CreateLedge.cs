using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CreateLedge : MonoBehaviour
{
    [SerializeField][Dropdown("ledgeLayers")]
    private string ledgeLayer;
    [SerializeField]
    private GameObject singleStep;
    [SerializeField]
    private float ledgeHeight = 10;
    [SerializeField]
    private float distanceBetweenSteps = 1;
    [SerializeField]
    private float colliderWidthX = 0.25f;
    [SerializeField]
    private float colliderWidthZ = 1.0f;
    [SerializeField]
    private List<Transform> ledgeList;
    //Im fine guyssssssssssss..Thanks


    private string[] ledgeLayers = { "Ledge", "Select Correct Layer" };

    [Button("Create Ledge")]
    public void OnCreateLedge() {
        if (singleStep == null)
        {
            Debug.LogError("Gameobject singleStep Is Null!");
        }
        else {
            ClearLedge();
            CreateBoxCollider();
            int numberOfSteps = Mathf.RoundToInt(ledgeHeight / distanceBetweenSteps);
            ledgeList = new List<Transform>();
            for (int i = 0; i < numberOfSteps; i++)
            {
                GameObject go = Instantiate(singleStep);
                go.transform.parent = transform;
                go.transform.position = transform.position + (i *transform.up * distanceBetweenSteps);
                ledgeList.Add(go.transform);
            }
            gameObject.layer = LayerMask.NameToLayer(ledgeLayer) - 1;
        }
    }
    [Button("Clear Ledge")]
    public void ClearLedge() {
        foreach (Transform t in ledgeList) {
            DestroyImmediate(t.gameObject);
        }
        ledgeList.Clear();
        DestroyImmediate(gameObject.GetComponent<BoxCollider>());
    }

    public void CreateBoxCollider() {
        if (GetComponent<BoxCollider>() != null) {
            DestroyImmediate(gameObject.GetComponent<BoxCollider>());
        }
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.size = new Vector3(colliderWidthX,ledgeHeight,colliderWidthZ);
        collider.center = new Vector3(0f,((ledgeHeight/2) - colliderWidthX -(colliderWidthX/2)),0f);
    }

}
