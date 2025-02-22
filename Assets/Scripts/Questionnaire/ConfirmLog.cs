using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConfirmLog : MonoBehaviour
{
    public LogManager logManager;

    public QuestionnaireSliderController sliderController;
    public GameObject questionnaireParentObject;

    private bool isLeftButtonPressed;
    private bool isRightButtonPressed;
    //public TextMeshProUGUI debuggingText;

    public static int confirmCounter = 0;

    public void OnConfirmClicked()
    {
        logManager.AddGraphicCsLog(sliderController.pinchSlider.value, confirmCounter);
        confirmCounter++;
        Debug.Log("Confirm: " + sliderController.pinchSlider.value);
        EventManager.TriggerEvent(Const.Events.GraphicCsSubmitted);
    }

    public void OnLeftButtonPressBegin()
    {
        isLeftButtonPressed = true;
        
        if (IsBothButtonsPressed())
        {
            OnConfirmClicked();
        }
    }

    public void OnLeftButtonPressEnd()
    {
        isLeftButtonPressed = false;
    }
    
    public void OnRightButtonPressBegin()
    {
        isRightButtonPressed = true;
        
        if (IsBothButtonsPressed())
        {
            OnConfirmClicked();
        }
    }

    public void OnRightButtonPressEnd()
    {
        isRightButtonPressed = false;
    }

    private bool IsBothButtonsPressed()
    {
        return isLeftButtonPressed && isRightButtonPressed;
    }
}
