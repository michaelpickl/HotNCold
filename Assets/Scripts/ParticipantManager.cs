using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticipantManager : MonoBehaviour
{
    public static int participantId;
    public int participantIdEditor;

    private void Awake()
    {
        participantId = participantIdEditor;
    }
}
