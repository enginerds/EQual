using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour
{
    public string selectedScene;
    public int selectorOption;

    public GameObject loaderScreen,selectorScreen, userIDScreen, WrongUserIDScreen, alreadyPlayed;
    public Text loaderText,notMatchingText;
    public string[] scenesAvailable = { "Szenario 2", "Szenario 4" };
    public string[] scenesNames = { "EQual_I_Scenario_T2", "EQual_I_Scenario_T4" };
    public Dropdown sceneDropDownBox;

    public InputField userIDTxt, userIDConfirmTxt;

    public string userID, userIDConfirmtxt;

    /// <summary>
    /// for pressing the Tab key while on the innitial screen
    /// </summary>
    public GameObject[] tabObjs;
    private int tabCtr = 0;

    // Start is called before the first frame update
    void Start()
    {
        alreadyPlayed.SetActive(false);

        selectorOption = 0;
        if (sceneDropDownBox != null)
        {
            for (int i = 0; i < scenesAvailable.Length; i++)
            {
                Dropdown.OptionData option1 = new Dropdown.OptionData(scenesAvailable[i]);
                sceneDropDownBox.options.Add(option1);

            }
            sceneDropDownBox.RefreshShownValue();
            sceneDropDownBox.captionText.text = sceneDropDownBox.options[0].text;
            sceneDropDownBox.onValueChanged.AddListener(delegate { OnChangeOfDropDownValue(); });
            selectorScreen.SetActive(false);
            loaderScreen.SetActive(false);
        }
        if (LogDB.instance.PlayerID == "")
        {
            userIDScreen.SetActive(true);
        }
        else
        {
            userIDScreen.SetActive(false);
            selectorScreen.SetActive(true);
        }
        if (userIDTxt != null)        
            userIDTxt.onValueChanged.AddListener(delegate { OnChangeOfUserIDValue(); });        

        if (userIDConfirmTxt != null)        
            userIDConfirmTxt.onValueChanged.AddListener(delegate { OnChangeOfUserIDConfirmValue(); }); 
        
        tabObjs[tabCtr].GetComponent<InputField>().Select();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (++tabCtr > tabObjs.Length - 1) tabCtr = 0;

            if (tabObjs[tabCtr].GetComponent<InputField>())
                tabObjs[tabCtr].GetComponent<InputField>().Select();
            else
                tabObjs[tabCtr].GetComponent<Button>().Select();
        }
    }

    public void ExitGame() {
    #if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
    #else		
        Application.Quit();
    #endif

    }


    /* User ID 222005data json format
     * file name : userIDs.json
     *
     * {
     *  "TotalUsers:3,
        "UserIDs":[
			{
        	  	"UserID":"123445"
			},
            {
        	  	"UserID":"243445"
			},
            {
        	  	"UserID":"654434"
			}
         ]
        }
     *
     *
     */

    public void OnChangeOfDropDownValue()
    {
        selectorOption = sceneDropDownBox.value;
        sceneDropDownBox.RefreshShownValue();
    }

    public void OnChangeOfUserIDValue()
    {
        userID = userIDTxt.text;
       if(notMatchingText!=null) if(notMatchingText.gameObject.activeInHierarchy) notMatchingText.gameObject.SetActive(false);
    }
    public void OnChangeOfUserIDConfirmValue()
    {
        userIDConfirmtxt = userIDConfirmTxt.text;
        if (notMatchingText != null) if (notMatchingText.gameObject.activeInHierarchy) notMatchingText.gameObject.SetActive(false);
    }
  
    public void OnContinueToSceneSelectorOnClick()
    {
        if(userID !="") 
        {
            //
            // check for a Valid UserID
            //
            bool isValidUserID = false;
            foreach(int validUserID in LogDB.instance.validIDs) {
                isValidUserID = (int.Parse(userID) == validUserID) || userID.Substring(0, 1) == "0";
                if (isValidUserID) break;
            }

            //
            // if player did not enter a Valid UserID, show Warning Screen, else keep going
            //
            if (!isValidUserID)
            {
                WrongUserIDScreen.SetActive(true);
                userIDScreen.SetActive(false);
            }
            else
            {
                if (userID == userIDConfirmtxt)
                {
                    selectorScreen.SetActive(true);
                    userIDScreen.SetActive(false);
                    PlayerPrefs.SetString("CurrentUser", userID);
                    LogDB.instance.PlayerID = userID;
                    if (notMatchingText != null) if (notMatchingText.gameObject.activeInHierarchy) notMatchingText.gameObject.SetActive(false);
                }
                else
                     if (notMatchingText != null) if (!notMatchingText.gameObject.activeInHierarchy) notMatchingText.gameObject.SetActive(true);
            }
        }
        else
        {
            if (userID == null || userID == "")
            {
                Guid guid = Guid.NewGuid();
                userID = guid.ToString("N");
                selectorScreen.SetActive(true);
                userIDScreen.SetActive(false);
                Debug.Log("Scenario Selector : Temp User Id is created at start, this will be replaced by the user id that the user enters at login");
                LogDB.instance.PlayerID = userID;
                if (notMatchingText != null) if (notMatchingText.gameObject.activeInHierarchy) notMatchingText.gameObject.SetActive(false);
            }
        }
    }

    public void OnLoadSceneOnClick()
    {
        // set the scenario Root for the selected scenario in log so all the logging of that scenario comes under this root in the player log file
        LogDB.instance.SetScenarioMasterLogObject(scenesAvailable[selectorOption]);
        selectorScreen.SetActive(false);
        loaderText.text = "Laden von " + scenesAvailable[selectorOption] + "...";
        loaderScreen.SetActive(true);
        // PlayerPrefs.SetInt("allowChangeSelectionFeature", (allowChangeSelectionFeature.isOn)?1:0);
        // LogDB.instance.allowChangeSelectionFeature = allowChangeSelectionFeature.isOn; // set the Change Slection Feature ON or OFF for all the Scenarios in this session (until they change it again)
        PlayerPrefs.SetInt("allowChangeSelectionFeature", 1); // Client asked this to be ON always, so Users will not be able to select it but it will be ON by default. and it is no more 3 times, it is only 1 time.
        LogDB.instance.allowChangeSelectionFeature = true; // set the Change Slection Feature ON or OFF for all the Scenarios in this session (until they change it again)
       
        Debug.Log(LogDB.instance.PlayerID.Substring(0, 1));
        // if scenario (scenesNames[selectorOption]) has NOT yet been played
        if (!PlayerPrefs.HasKey(scenesNames[selectorOption]) || LogDB.instance.PlayerID.Substring(0,1) == "0")
        {            
            LogDB.instance.SetCurrentScenario(scenesNames[selectorOption]);
            SceneManager.LoadScene(scenesNames[selectorOption], LoadSceneMode.Single);
        } 
        else // don't allow player to re-play a scenario
        {
            loaderScreen.SetActive(false);
            alreadyPlayed.SetActive(true);
        }
    }

    public void AlreadyPlayedOnClick()
    {
        Debug.Log("already played, but going back (to try another scene)...");
        alreadyPlayed.SetActive(false);
        selectorScreen.SetActive(true);
    }
}
