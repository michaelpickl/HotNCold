using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using TMPro;

public class LogManager : MonoBehaviour
{
    //public TextMeshProUGUI debuggingText;
    private  string graphicCsPath;
    private  string thermalComfortPath;

    private  string puzzlePath;
    private  string pizzaPath;
    private  string headPath;
    private  string targetPath;
    
    private  DateTime lastHeadLogTime = DateTime.Now;
    private  int headLogIntervalInMs = 100;

    private string handSetting;

    public void Awake()
    {
        Debug.Log(Config.participantId);
        handSetting = Config.handSetting;

        graphicCsPath = "./Logs/graphicCs/" + Config.participantId + "_" + handSetting + ".csv";
        thermalComfortPath = "./Logs/thermalcomfort/" + Config.participantId + "_" + handSetting + ".csv";
        puzzlePath = "./Logs/puzzle/" + Config.participantId + "_" + handSetting + ".csv";
        pizzaPath = "./Logs/pizza/" + Config.participantId + "_" + handSetting + ".csv";
        headPath = "./Logs/head/" + Config.participantId + "_" + handSetting + ".csv";
        targetPath = "./Logs/target/" + Config.participantId + "_" + handSetting + ".csv";
    }

    public void AddPizzaLog(float score)
    {
        string newLine = Config.participantId + "\t";
        newLine += score;
        newLine += Environment.NewLine;
        
        if (!File.Exists(pizzaPath))
        {
            var csvHeader = "participantId\tpizzaScore" + Environment.NewLine;
            File.WriteAllText(pizzaPath, csvHeader);
        }
        File.AppendAllText(pizzaPath, newLine);
    }

    public  void AddGraphicCsLog(float sliderValue, int count)
    {
        var participantId = Config.participantId;

        var csv = new StringBuilder();

        string newLine = participantId + "\t";
        newLine += handSetting + "\t";
        newLine += sliderValue;
        newLine += count;
        csv.AppendLine(newLine);

        string directoryPath = Path.GetDirectoryName(graphicCsPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (!File.Exists(graphicCsPath))
        {
            var csvHeader = "participantId\thands\tscore\tcount" + Environment.NewLine;
            File.WriteAllText(graphicCsPath, csvHeader);
        }
        File.AppendAllText(graphicCsPath, csv.ToString());
    }

    public void AddThermalComfortLog(string question, int score)
    {
        string newLine = Config.participantId + "\t";
        newLine += handSetting + "\t";
        newLine += question + "\t";
        newLine += score;
        newLine += Environment.NewLine;

        string directoryPath = Path.GetDirectoryName(thermalComfortPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        if (!File.Exists(thermalComfortPath))
        {
            var csvHeader = "participantId\thands\tquestion\tscore" + Environment.NewLine;
            File.WriteAllText(thermalComfortPath, csvHeader);
        }
        File.AppendAllText(thermalComfortPath, newLine);
    }

    public  void AddPuzzleLog(string type)
    {
        string newLine = Config.participantId + "\t";
        newLine += handSetting + "\t";
        newLine += type + "\t";
        newLine += GameTimeManager.Instance.CurrentGameTime;
        newLine += Environment.NewLine;

        string directoryPath = Path.GetDirectoryName(puzzlePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        if (!File.Exists(puzzlePath))
        {
            var csvHeader = "participantId\thands\ttype\tgameTime" + Environment.NewLine;
            File.WriteAllText(puzzlePath, csvHeader);
        }
        File.AppendAllText(puzzlePath, newLine);
    }

    public  void AddHeadLog(Transform headTransform)
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
        
        
        string newLine = Config.participantId + "\t";
        newLine += handSetting + "\t";
        newLine += headTransform.position.x + "\t";
        newLine += headTransform.position.y + "\t";
        newLine += headTransform.position.z + "\t";
        newLine += xRot + "\t";
        newLine += yRot + "\t";
        newLine += zRot + "\t";
        newLine += timeNow;
        newLine += GameTimeManager.Instance.CurrentGameTime;
        newLine += Environment.NewLine;

        string directoryPath = Path.GetDirectoryName(headPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        if (!File.Exists(headPath))
        {
            var csvHeader = "participantId\thands\txPos\tyPos\tzPos\txRot\tyRot\tzRot\ttimestamp\tgameTime" + Environment.NewLine;
            File.WriteAllText(headPath, csvHeader);
        }
        File.AppendAllText(headPath, newLine);

        lastHeadLogTime = timeNow;
    }

    // public  void AddTargetLog(TargetLogType targetLogType)
    // {
    //     DateTime timeNow = DateTime.Now;
        
    //     string newLine = Config.participantId + "\t";
    //     newLine += handSetting + "\t";
    //     newLine += targetLogType + "\t";
    //     newLine += timeNow;
    //     newLine += Environment.NewLine;

    //     if (!File.Exists(targetPath))
    //     {
    //         var csvHeader = "participantId\thands\ttype\ttimestamp" + Environment.NewLine;
    //         File.WriteAllText(targetPath, csvHeader);
    //     }
    //     File.AppendAllText(targetPath, newLine);
    // }

    private  float GetFormattedRotation(float eulerAngle)
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
