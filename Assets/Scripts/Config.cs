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

    public static int startImageIndex;
    public int startImageIndexEditor;


    private void Awake()
    {
        participantId = participantIdEditor;
        handSetting = handSettingEditor;
        isTutorial = isTutorialEditor;
        startImageIndex = startImageIndexEditor;
    }
}
