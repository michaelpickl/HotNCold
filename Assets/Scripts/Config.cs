using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    public static int participantId;
    public int participantIdEditor;

    public static string handSetting;
    public string handSettingEditor;

    public static bool isTutorial;
    public bool isTutorialEditor;


    private void Awake()
    {
        participantId = participantIdEditor;
        handSetting = handSettingEditor;
        isTutorial = isTutorialEditor;
    }
}
