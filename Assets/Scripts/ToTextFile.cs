using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;

public class ToTextFile : MonoBehaviour
{
    private string _currentTime;
    private string _txtDocumentName;
    private StringBuilder _vrLog;

    // Start is called before the first frame update
    void Start()
    {
        _vrLog = new StringBuilder();

        // Get the current time
        System.DateTime current = System.DateTime.Now;

        // Format the time as a string
        _currentTime = string.Format("{0:00}_{1:00}_{2:00}_{3:00}_{4:00}", current.Day, current.Month, current.Year, current.Hour, current.Minute);
        
        // used for saving to text file
        // Directory.CreateDirectory(Application.persistentDataPath  + "/Logs/");
        // CreateTextFile();
    }

    public void AppendLine(string text)
    {
        // Check if the line already exists in the log
        if (_vrLog.ToString().Contains(text))
        {
            return;
        }
        
        _vrLog.AppendLine(text);
    }

    public string GetLogDate()
    {
        return _currentTime;
    }

    public string GetVRLog()
    {
        return _vrLog.ToString();
    }

    // public void CreateTextFile()
    // {
    //     _txtDocumentName = Application.persistentDataPath  + "/Logs/" + _currentTime + ".txt"; // Set the document name

    //     if (File.Exists(_txtDocumentName))
    //     {
    //         AppendToDocument("das ist ein test pls");
    //         return;
    //     }

    //     try
    //     {
    //         File.WriteAllText(_txtDocumentName, "DAS IST EINE FILE VON: " + _currentTime);
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.LogError("Error writing file: " + ex.Message);
    //     }
    // }

    // public void AppendToDocument(string text)
    // {
    //     File.AppendAllText(_txtDocumentName, text + "\n");
    // }
}
