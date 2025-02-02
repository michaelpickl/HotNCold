using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class QuestionnaireSliderController : MonoBehaviour 
{

    //public PinchSlider pinchSlider;
    public Slider pinchSlider;
    public float distanceToCamera = .7f;

    public void ShowAndReset()
    {
        var cameraTransform = Camera.main.transform;
        var currentTransform = transform;
        
        //pinchSlider.SliderValue = .5f;
        pinchSlider.value = .5f;
        gameObject.SetActive(true);
        
        currentTransform.position = cameraTransform.position + cameraTransform.forward * distanceToCamera;

        //currentTransform.SetLocalY(cameraTransform.position.y);
        Vector3 newLocalPosition = currentTransform.localPosition;
        newLocalPosition.y = cameraTransform.position.y;
        currentTransform.localPosition = newLocalPosition;

        currentTransform.rotation = new Quaternion(currentTransform.rotation.x, cameraTransform.rotation.y,
            currentTransform.rotation.z, cameraTransform.rotation.w);
    }
}
