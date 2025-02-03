
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestionnaireTimeManager : MonoBehaviour
{

    public GameObject questionnaireParentObject;
    public QuestionnaireSliderController graphicCsSlider;

    public int firstShowSeconds;
    public int secondShowSeconds;
    public int thirdShowSeconds;

    public GameObject questions;
    public GameObject finish;

    private bool firstTimeShown = false;
    private bool secondTimeShown = false;
    private bool thirdTimeShown = false;

    private float timestampAtSceneLoad;

    void Start()
    {
        questionnaireParentObject.SetActive(false);
        timestampAtSceneLoad = Time.time;
        EventManager.StartListening(Const.Events.GraphicCsSubmitted, OnQuestionnaireSubmitted);
        EventManager.StartListening(Const.Events.TutorialCompleted, OnTutorialCompleted);
    }
    
    void Update()
    {
        if(!Config.isTutorial)
        {
            var timeDelta = GameTimeManager.Instance.CurrentGameTime;//Time.time - timestampAtSceneLoad;

            if (!firstTimeShown && timeDelta >= firstShowSeconds)
            {
                firstTimeShown = true;
                graphicCsSlider.ShowAndReset();
                GameTimeManager.Instance.PauseGameTime();
            }
            
            if (!secondTimeShown && timeDelta >= secondShowSeconds)
            {
                secondTimeShown = true;
                graphicCsSlider.ShowAndReset();
                GameTimeManager.Instance.PauseGameTime();
            }
            
            if (!thirdTimeShown && timeDelta >= thirdShowSeconds)
            {
                thirdTimeShown = true;
                graphicCsSlider.ShowAndReset();
                GameTimeManager.Instance.PauseGameTime();
            }
        }
    }

    private void Reset()
    {
        firstTimeShown = false;
        secondTimeShown = false;
        thirdTimeShown = false;
        timestampAtSceneLoad = Time.time;
    }

    private void OnQuestionnaireSubmitted()
    {
        Debug.Log("RESUME GAME");
        if(!thirdTimeShown)
        {
            questionnaireParentObject.SetActive(false);
            GameTimeManager.Instance.ResumeGameTime();
        }
        else{
            questions.SetActive(false);
            finish.SetActive(true);
        }
    }

    private void OnTutorialCompleted()
    {
        questionnaireParentObject.SetActive(true);
        questions.SetActive(false);
        finish.SetActive(true);
    }
}
