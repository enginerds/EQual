using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StudentReaction : UnityEvent<string> { }


[System.Serializable]
public class ResetDecision : UnityEvent { }

public class UserInterfaceManager : MonoBehaviour
{

    #region Private Variables
    private void Start()
    {
        //RandomlySelectStudyMatrial();
        //ProbabilityBasedStudyMaterial();
    }


    #endregion

    #region Public Variables

    public GamePlayManager gamePlayManager;
    public DataManager dataManager;


    public GameObject startPanel;
    public GameObject studentInfoPanel;
    public GameObject questionPanel;
    public GameObject teacherActionPanel;
    public GameObject decisionPanel;
    public GameObject multiChoicePanel;
    public GameObject commentPanel;
    public GameObject askingTeacherPanel;
    public GameObject loadingPanel;
    public GameObject initLoadingPanel;
    public GameObject changeSelectionDetailsGroup;
    public GameObject knowYourClassPanel;
    public GameObject studentsInBreakPanel;
    public GameObject introPanel;
    //
    public GameObject tempPanel;
    public Text tempText;

    public Text studentName, age, gender, ethinicity, specialDisabled, levelOfAchivment;
    public InputField commentInputField;

    [SerializeField]
    public StudentReaction studentReaction;

    [SerializeField]
    public ResetDecision resetDecision;

    private string teacherSelectedOption;
    public string teacherSelectedQuestion;

    public int noOfNoCounts = 0;


    public bool multiChoicePanelAnswered = false;
    #endregion

    #region Private Functions



    private void OnEnable()
    {
        if (changeSelectionDetailsGroup != null)
        {
            bool allowThis = (PlayerPrefs.GetInt("allowChangeSelectionFeature") == 1) ? true : false;
            //    if (allowThis)
            if (changeSelectionDetailsGroup != null) changeSelectionDetailsGroup.SetActive(true);
            //     else
            //         changeSelectionDetailsGroup.SetActive(false);
        }

    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            /* if (hit.transform.gameObject.tag != "child") {
                 PopulateStudentInfo(null);
             }*/
            if (hit.transform.gameObject.tag != "child")
            {
                PopulateStudentInfo(null);
                studentInfoPanel.SetActive(false);
            }
            else
            {
                studentInfoPanel.SetActive(true);
            }

            // ensure you picked right object
        }
    }

    #endregion

    #region Public Functions

    #endregion

    public void PopulateStudentInfo(StudentData studentData)
    {
        if (studentData != null)
        {
            studentName.text = " " + studentData.Name;
            age.text = " " + studentData.Age;
            gender.text = " " + studentData.Gender;
            ethinicity.text = " " + studentData.Ethnicity;
            specialDisabled.text = " " + studentData.SpecialNeed;
            levelOfAchivment.text = " " + studentData.LevelOfAchiement;
        }
        else
        {
            studentName.text = " ";
            age.text = " ";
            gender.text = " ";
            ethinicity.text = " ";
            specialDisabled.text = " ";
            levelOfAchivment.text = " ";
        }

    }

    public void ShowOrHideQuestionPanel(bool value)
    {
        if (questionPanel != null) questionPanel.SetActive(value);
        if (value)
        {
            if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetInitialSituationValueForKey("start2", gamePlayManager.GetCurrentTime());
        }
        else
        {
            if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetInitialSituationValueForKey("end2", gamePlayManager.GetCurrentTime());
        }
        gamePlayManager.isStudentsInteractable = !value;
    }

    public void ShowOrHideTeacherActionPanel(bool value)
    {
        if (teacherActionPanel != null) teacherActionPanel.SetActive(value);
        gamePlayManager.isStudentsInteractable = !value;

    }

    public void ShowOrHideDecisionPanel(bool value)
    {
        if (decisionPanel != null) decisionPanel.SetActive(value);
        gamePlayManager.isStudentsInteractable = !value;

    }

    public void ShowOrHideMultiChoicePanel(bool value)
    {
        if (multiChoicePanel != null) multiChoicePanel.SetActive(value);

    }


    public void ShowOrHideInitLoadingPanel(bool value)
    {
        if (initLoadingPanel != null) initLoadingPanel.SetActive(value);
        gamePlayManager.isStudentsInteractable = !value;

    }


    public void ShowOrHideIntroPanel(bool value)
    {
        if (introPanel != null) introPanel.SetActive(value);
        if (value)
        {
            if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetPlayerActionsValueForKey("IntroPopupWindowShownAt", gamePlayManager.GetCurrentTime());
        }
        else
        {
            if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetPlayerActionsValueForKey("IntroPopupWindowAcceptedAt", gamePlayManager.GetCurrentTime());
        }
    }

    public void ShowOrHideKnowYourClassPanel(bool value)
    {
        if (knowYourClassPanel != null) knowYourClassPanel.SetActive(value);
        if (value)
        {
            if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetGettingToKnowYourClassValueForKey("start1", gamePlayManager.GetCurrentTime());
        }
        else
        {
            if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetGettingToKnowYourClassValueForKey("end1", gamePlayManager.GetCurrentTime());
        }
        gamePlayManager.isStudentsInteractable = value; // students info should be active when this panel is shown so the player can check it

    }
    public void ShowOrHideStudentsInBreakPanel(bool value)
    {
        if (studentsInBreakPanel != null) studentsInBreakPanel.SetActive(value);
        gamePlayManager.isStudentsInteractable = !value; // students info should not be active when this panel is shown so the player can check it

    }

    public void ShowOrHideCommentPanel(bool value)
    {
        if (commentPanel != null) commentPanel.SetActive(value);
    }
    public void ShowOrHideAskingTeacherPanel(bool value)
    {
        if (askingTeacherPanel != null) askingTeacherPanel.SetActive(value);
        gamePlayManager.isStudentsInteractable = !value;

    }

    public void ShowOrHideLoadingPanel(bool value)
    {

        if (loadingPanel != null) loadingPanel.SetActive(value);

    }


    public void ShowOrHideTempPanel(bool value)
    {
        if (tempPanel != null) tempPanel.SetActive(value);
        gamePlayManager.isStudentsInteractable = !value;

    }
    public void SetTempText(string value) { tempText.text = value; }

    #region Button Functions

    // not using this function anymore, the start button will directly call the StartScenarioOne function from ScenarioOneManager (refered from GamePlayManager)
    public void StartButtonFunction()
    {
        //  gamePlayManager.CreatePlayerID();
        if (startPanel != null) startPanel.SetActive(false);
    }

    public void TeacherActionToggle(string value)
    {
        teacherSelectedOption = value;
    }
    public void TeacherActionToggleQuestionText(Text value)
    {
        teacherSelectedQuestion = value.text;
    }

    public void TeacherActionButton()
    {
        ShowOrHideTeacherActionPanel(false);
        print("teacher selected the option " + teacherSelectedOption);

        /*
         * if(gamePlayManager.GetComponent<ScenarioFourManager>() != null)
        {
            // cheapest hack for some sutpid unity editor not accepting mutend variable problem
            gamePlayManager.GetComponent<ScenarioFourManager>().StudentReaction(teacherSelectedOption);
        }
        else if (gamePlayManager.GetComponent<ScenarioElevenManager>() != null)
        {
            // cheapest hack for some sutpid unity editor not accepting mutend variable problem
            gamePlayManager.GetComponent<ScenarioElevenManager>().StudentReaction(teacherSelectedOption);
        }
   /*     else if (gamePlayManager.GetComponent<ScenarioTwelveManager>() != null)
        {
            // cheapest hack for some sutpid unity editor not accepting mutend variable problem
            gamePlayManager.GetComponent<ScenarioTwelveManager>().StudentReaction(teacherSelectedOption);
        }*/
        studentReaction.Invoke(teacherSelectedOption);


    }

    string teacherExitDecision, teacherMultiChoiceAnswer;
    public void TeacherExitDecisionToggle(string value)
    {
        teacherExitDecision = value;
    }


    public void TeacherExitMultiChoiceAnswerToggle(string value)
    {
        teacherMultiChoiceAnswer = value;
        Debug.Log("the multichoice Answer given is " + teacherMultiChoiceAnswer);
    }

    public void TeacherExitMultiChoiceAnswerButton()
    {
        Debug.Log("the multichoice Answer accepted is " + teacherMultiChoiceAnswer);
        if (teacherMultiChoiceAnswer != "")
        {
            ShowOrHideMultiChoicePanel(false);
            multiChoicePanelAnswered = true;
        }
    }

    public void InitiateShowMultiChoiceAnswerPanelAndContinueExitScenario()
    {

        gamePlayManager.playerActionDataHandler.PlayerCommnet = commentInputField.text;
        string commentText = (commentInputField.text.Contains("Kommentare (Wahlweise)")) ? "" : commentInputField.text;

        //   if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("Model", teacherMultiChoiceAnswer);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASComment", commentText);

        ShowOrHideDecisionPanel(false);

        StartCoroutine(ShowMutiChoiceAnswerPanelAndContinueExit());
    }

    private IEnumerator ShowMutiChoiceAnswerPanelAndContinueExit()
    {
        ShowOrHideMultiChoicePanel(true);
        yield return new WaitUntil(() => multiChoicePanelAnswered);
        Debug.Log("the multichoice Answer is " + teacherMultiChoiceAnswer);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("Model", teacherMultiChoiceAnswer);
        yield return null;
        StartCoroutine(LoadSceneLoader());


    }


    public void DoExitDecision()
    {

        Debug.LogError("DOOOO EXIT A");

        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASTimeFinished", gamePlayManager.GetCurrentTime());

        bool allowThis = true; //(PlayerPrefs.GetInt("allowChangeSelectionFeature") == 1) ? true : false;
        Debug.LogError(" ALLOW THIS : " + allowThis);
        if (allowThis)
        {

            // If Allow change Selection feature is enabled then do allow them to select for 1 time max.
            string value = teacherExitDecision;

            Debug.Log(value);


            if (noOfNoCounts < 1)
            {
                Debug.LogError("DOOOO EXIT B");
                if (value == "Yes")
                {
                    InitiateShowMultiChoiceAnswerPanelAndContinueExitScenario();
                }
                else
                {
                    Debug.LogError("DOOOO EXIT C");
                    noOfNoCounts++;

                    Debug.Log(noOfNoCounts);

                    resetDecision.Invoke();
                }
            }
            else
            {
                InitiateShowMultiChoiceAnswerPanelAndContinueExitScenario();
            }
        }
        else
        {
            // If Allow change Selection feature is not enabled then save the comment, Log and go to next scene
            // Update: this condition is no more valid. 
            InitiateShowMultiChoiceAnswerPanelAndContinueExitScenario();
        }

    }

    public void TeacherExitDecisionButton()
    {

        ShowOrHideDecisionPanel(false);
        multiChoicePanelAnswered = false;
        DoExitDecision();


        // As a New Multichoice Answer Panel is to be presented after each scenario finish aspect, the finish code is shifted to a new function (DoExitDecision) which will be called from ShowMultichoicePanel Co-Routine.

    }

    public void ShiftScene(int value)
    {
        SceneManager.LoadScene(value);
    }

    private IEnumerator LoadSceneLoader()
    {
        yield return new WaitForSeconds(0.1f);

        if (gamePlayManager.LOG_ENABLED) Debug.LogFormat("(UserInterfaceManager) | LoadSceneLoader() | completed scenario (now leaving): {0}", LogDB.instance.currentScenario);
        if (gamePlayManager.LOG_ENABLED) PlayerPrefs.SetInt(LogDB.instance.currentScenario, 1);

        SceneManager.LoadScene("ScenarioLoader", LoadSceneMode.Single);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        /*
        #if (UNITY_EDITOR)
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
        */
    }

    public void CommentSaveButton()
    {

        Application.Quit();
    }
    #endregion

}
