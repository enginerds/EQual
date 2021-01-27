using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FileHandler
{
    string PATH =  Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/PlayerData.json";

    public void SaveTextFile(string text) {
        File.WriteAllText(PATH, text);
        Debug.Log(PATH);
    }

}
