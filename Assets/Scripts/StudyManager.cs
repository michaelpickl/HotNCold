using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StudyManager : MonoBehaviour
{
    public LogManager logManager;

    public GameObject leftHand;
    public GameObject rightHand;
    public Material iceMaterial;
    public Material fireMaterial;
    public Material neutralMaterial;

    public Material ghostMaterial;

    //public TextMeshProUGUI debuggingText;

    void Start()
    {
        AssignHandMaterial();
    }

    private void Update()
    {
        if (Camera.main is null) return;

        logManager.AddHeadLog(Camera.main.transform);
    }

    public void AssignHandMaterial() {
        Material material = neutralMaterial;
        if(Config.isTutorial)
        {
            material = ghostMaterial;
        }
        else if(Config.handSetting == "fire")
        {
            material = fireMaterial;
        }
        else if(Config.handSetting == "ice")
        {
            material = iceMaterial;
        }

        if (leftHand != null) {
            Renderer leftRenderer = leftHand.GetComponent<Renderer>();
            if (leftRenderer != null && leftRenderer.materials.Length >= 2) {
                Material[] materials = leftRenderer.materials; 
                materials[1] = material; 
                leftRenderer.materials = materials; 
            }
        }
        if (rightHand != null) {
            Renderer rightRenderer = rightHand.GetComponent<Renderer>();
            if (rightRenderer != null && rightRenderer.materials.Length >= 2) {
                Material[] materials = rightRenderer.materials; 
                materials[1] = material; 
                rightRenderer.materials = materials; 
            }
        }
    }
}
