using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
public class PlayerActionDataHandler : MonoBehaviour
{

    private string playerID;
    private string playerAction, playerComment;
    



    private FileHandler fileHandler;
    private JSONObject playerDataInJsonFormat;
    private string playerDataInStringFormat;

    public string PlayerDataInStringFormat
    {
        set
        {
            playerDataInStringFormat = value;
            fileHandler.SaveTextFile(playerDataInStringFormat);
           
        }
    }


    public string PlayerID
    {
        set
        {
            playerID = value;    
            Init();
           
        }
    }

    public string PlayerAction
    {
        set
        {
            playerAction = value;
            
            SaveData("player_action", value.ToString());
        }
    }

    public string PlayerCommnet
    {
        set
        {
            playerComment = value;

            SaveData("player_Comment", value.ToString());
        }
    }


    void Init()
    {
        fileHandler = new FileHandler();
        playerDataInJsonFormat = new JSONObject();
    }


    private void SaveData(string nodeName, string value)
    {
        playerDataInJsonFormat.Add(nodeName, value);
        PlayerDataInStringFormat = playerDataInJsonFormat.ToString();
    }

}
