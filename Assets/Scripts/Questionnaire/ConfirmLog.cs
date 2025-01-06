using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ConfirmLog : MonoBehaviour
{

    public QuestionnaireSliderController sliderController;
    public GameObject questionnaireParentObject;

    private bool isLeftButtonPressed;
    private bool isRightButtonPressed;
    public void OnConfirmClicked()
    {
        LogManager.AddGraphicCsLog(sliderController.pinchSlider.value);
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
