using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    private static string graphicCsPath = "./Logs/graphicCs/" + ParticipantManager.participantId + ".csv";
    private static string thermalComfortPath = "./Logs/thermalcomfort/" + ParticipantManager.participantId + ".csv";
    private static string pizzaPath = "./Logs/pizza/" + ParticipantManager.participantId + ".csv";
    private static string headPath = "./Logs/head/" + ParticipantManager.participantId + ".csv";
    private static string targetPath = "./Logs/target/" + ParticipantManager.participantId + ".csv";
    
    private static DateTime lastHeadLogTime = DateTime.Now;
    private static int headLogIntervalInMs = 100;

    public static string handSetting;

    public static void AddPizzaLog(float score)
    {
        string newLine = ParticipantManager.participantId + "\t";
        newLine += score;
        newLine += Environment.NewLine;
        
        if (!File.Exists(pizzaPath))
        {
            var csvHeader = "participantId\tpizzaScore" + Environment.NewLine;
            File.WriteAllText(pizzaPath, csvHeader);
        }
        File.AppendAllText(pizzaPath, newLine);
    }

    public static void AddGraphicCsLog(float sliderValue)
    {
        var participantId = ParticipantManager.participantId;

        var csv = new StringBuilder();

        string newLine = participantId + "\t";
        newLine += handSetting + "\t";
        newLine += sliderValue;
        csv.AppendLine(newLine);

        if (!File.Exists(graphicCsPath))
        {
            var csvHeader = "participantId\thands\tscore" + Environment.NewLine;
            File.WriteAllText(graphicCsPath, csvHeader);
        }
        File.AppendAllText(graphicCsPath, csv.ToString());
    }

    public static void AddThermalComfortLog(string question, int score)
    {
        string newLine = ParticipantManager.participantId + "\t";
        newLine += handSetting + "\t";
        newLine += question + "\t";
        newLine += score;
        newLine += Environment.NewLine;
        
        if (!File.Exists(thermalComfortPath))
        {
            var csvHeader = "participantId\thands\tquestion\tscore" + Environment.NewLine;
            File.WriteAllText(thermalComfortPath, csvHeader);
        }
        File.AppendAllText(thermalComfortPath, newLine);
    }

    public static void AddHeadLog(Transform headTransform)
    {
        
        DateTime timeNow = DateTime.Now;

        TimeSpan deltaTime = timeNow - lastHeadLogTime;

        if (deltaTime.Milliseconds < headLogIntervalInMs)
        {
            return;
        }
        
        float xRot = GetFormattedRotation(headTransform.eulerAngles.x);
        float yRot = GetFormattedRotation(headTransform.eulerAngles.y);
        float zRot = GetFormattedRotation(headTransform.eulerAngles.z);
        
        
        string newLine = ParticipantManager.participantId + "\t";
        newLine += handSetting + "\t";
        newLine += headTransform.position.x + "\t";
        newLine += headTransform.position.y + "\t";
        newLine += headTransform.position.z + "\t";
        newLine += xRot + "\t";
        newLine += yRot + "\t";
        newLine += zRot + "\t";
        newLine += timeNow;
        newLine += Environment.NewLine;
        
        if (!File.Exists(headPath))
        {
            var csvHeader = "participantId\thands\txPos\tyPos\tzPos\txRot\tyRot\tzRot\ttimestamp" + Environment.NewLine;
            File.WriteAllText(headPath, csvHeader);
        }
        File.AppendAllText(headPath, newLine);

        lastHeadLogTime = timeNow;
    }

    public static void AddTargetLog(TargetLogType targetLogType)
    {
        DateTime timeNow = DateTime.Now;
        
        string newLine = ParticipantManager.participantId + "\t";
        newLine += handSetting + "\t";
        newLine += targetLogType + "\t";
        newLine += timeNow;
        newLine += Environment.NewLine;

        if (!File.Exists(targetPath))
        {
            var csvHeader = "participantId\thands\ttype\ttimestamp" + Environment.NewLine;
            File.WriteAllText(targetPath, csvHeader);
        }
        File.AppendAllText(targetPath, newLine);
    }

    private static float GetFormattedRotation(float eulerAngle)
    {
        if(eulerAngle <= 180f)
        {
            return eulerAngle;
        }

        return eulerAngle - 360f;
    }
    
    public enum TargetLogType
    {
        SHOT,
        HIT
    }
}
