using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StudyManager : MonoBehaviour
{
    public int actionSceneDurationInSeconds;
    public int neutralSceneDurationInSeconds;

    public GameObject fireBall;
    public GameObject iceBall;

    //public MagicGestureDetection magicGestureDetection;

    public Light sceneLight;

    //public StudyHandManager studyHandManager;

    public Material iceSkybox;
    public Material fireSkybox;
    public Material neutralSkybox;
    //private IMixedRealitySceneSystem mixedRealitySceneSystem;

    private SceneSetting currentSceneSetting = SceneSetting.INTRO_WORLD;
    private HandSetting currentHandSetting = HandSetting.NEUTRAL_HANDS;

    private SceneSetting lastActionSceneSetting;
    private HandSetting lastActionHandSetting;
    
    // both bool variables need to be true in order for the next scene to load
    private bool canSwapSceneByTime;  // true when the time for the scene has elapsed
    private bool canSwapSceneByCondition = true; // true when the scene condition has been met (e.g. completed survey in neutral scene)
    
    public enum SceneSetting
    {
        ICE_WORLD,
        FIRE_WORLD,
        NEUTRAL_WORLD,
        INTRO_WORLD
    }

    public enum HandSetting
    {
        ICE_HANDS,
        FIRE_HANDS,
        NEUTRAL_HANDS
    }
    
    public List<SceneSetting> sceneOrder;
    public List<HandSetting> handOrder;
    private int nextSceneIndex;

    private void Awake()
    {
        //mixedRealitySceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        //mixedRealitySceneSystem.OnSceneLoaded += OnSceneLoaded;
        EventManager.StartListening(Const.Events.TutorialCompleted, ForceLoadNextScene);
        EventManager.StartListening(Const.Events.SceneConditionFulfilled, OnSceneConditionFulfilled);
    }

    private void Update()
    {
        if (Camera.main is null) return;
        
        LogManager.AddHeadLog(Camera.main.transform);
    }

    private void InitNextSceneCoroutine()
    {
        var delayInSeconds = 0f;

        var sceneSetting = sceneOrder[nextSceneIndex];
        
        if (sceneSetting == SceneSetting.FIRE_WORLD || sceneSetting == SceneSetting.ICE_WORLD)
        {
            delayInSeconds = actionSceneDurationInSeconds;
        } 
        else if (sceneSetting == SceneSetting.NEUTRAL_WORLD)
        {
            delayInSeconds = neutralSceneDurationInSeconds;
        }

        StartCoroutine(NextSceneByTimeCoroutine(delayInSeconds));
    }

    public IEnumerator NextSceneByTimeCoroutine(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        canSwapSceneByTime = true;

        if (canSwapSceneByTime && canSwapSceneByCondition)
        {
            LoadNextScene();
        }
    }

    private void OnSceneConditionFulfilled()
    {
        canSwapSceneByCondition = true;

        if (canSwapSceneByTime && canSwapSceneByCondition)
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        nextSceneIndex++;
        
        if (nextSceneIndex >= sceneOrder.Count)
        {
            // TODO implement end game
            Debug.Log("game finished pog");
            return;
        }

        DetermineSceneAndLoad();
    }

    private void DetermineSceneAndLoad()
    {
        var sceneSetting = sceneOrder[nextSceneIndex];

        canSwapSceneByTime = false; // reset scene timer flag

        if (sceneSetting == SceneSetting.FIRE_WORLD)
        {
            SceneManagerLoadNextScene(Const.Scenes.FireWorld);
            sceneLight.intensity = .1f;
            //SceneProvider.sceneSetting = SceneSetting.FIRE_WORLD;
            //SceneProvider.lastActionSceneSetting = SceneSetting.FIRE_WORLD;
        } 
        else if (sceneSetting == SceneSetting.ICE_WORLD)
        {
            SceneManagerLoadNextScene(Const.Scenes.IceWorld);
            sceneLight.intensity = .8f;
            //SceneProvider.sceneSetting = SceneSetting.ICE_WORLD;
            //SceneProvider.lastActionSceneSetting = SceneSetting.ICE_WORLD;
        }
        else if (sceneSetting == SceneSetting.NEUTRAL_WORLD)
        {
            SceneManagerLoadNextScene(Const.Scenes.NeutralWorld);
            canSwapSceneByCondition = false;
            sceneLight.intensity = 1f;
            //SceneProvider.sceneSetting = SceneSetting.NEUTRAL_WORLD;
        }
    }

    private void SceneManagerLoadNextScene(string sceneName)
    {
        //mixedRealitySceneSystem.LoadContent(sceneName, LoadSceneMode.Single);
    }

    private  void LoadLightingScene(string sceneName)
    {
        //mixedRealitySceneSystem.SetLightingScene(sceneName);
    }

    public void ForceLoadNextScene()
    {
        nextSceneIndex++;
        
        if (nextSceneIndex >= sceneOrder.Count)
        {
            // TODO implement end game
            Debug.Log("game finished pog");
            return;
        }
        
        DetermineSceneAndLoad();
    }

    void OnSceneLoaded(String sceneName)
    {
        //magicGestureDetection.DestroyAllMagicBalls();
        
        if (sceneName == Const.Scenes.Intro)
        {
            RenderSettings.skybox = neutralSkybox;
            return;
        }
        
        // only run this method for my own scenes
        if (sceneName != Const.Scenes.NeutralWorld && sceneName != Const.Scenes.IceWorld && sceneName != Const.Scenes.FireWorld)
        {
            return;
        }

        //magicGestureDetection.enabled = sceneName != "NeutralScene";

        LoadHandMesh();
        InitNextSceneCoroutine();
        SetFittingLightingScene(sceneName);
    }

    private void SetFittingLightingScene(string sceneName)
    {
        if (sceneName == Const.Scenes.FireWorld)
        {
            RenderSettings.skybox = fireSkybox;
            // mixedRealitySceneSystem.SetLightingScene(Const.Scenes.FireLightingScene);
        }
        else if (sceneName == Const.Scenes.IceWorld)
        {
            RenderSettings.skybox = iceSkybox;
            // mixedRealitySceneSystem.SetLightingScene(Const.Scenes.IceLightingScene);
        }
        else if (sceneName == Const.Scenes.NeutralWorld)
        {
            RenderSettings.skybox = neutralSkybox;
            // mixedRealitySceneSystem.SetLightingScene(Const.Scenes.NeutralLightingScene);
        }
    }

    private void LoadHandMesh()
    {

        var handSetting = handOrder[nextSceneIndex];

        // if (handSetting == HandSetting.FIRE_HANDS)
        // {
        //     studyHandManager.ActivateFireHands();
        //     studyHandManager.SetMagicBall(fireBall);
        //     SceneProvider.handSetting = HandSetting.FIRE_HANDS;
        //     SceneProvider.lastActionHandSetting = HandSetting.FIRE_HANDS;
        // }
        // else if (handSetting == HandSetting.ICE_HANDS)
        // {
        //     studyHandManager.ActivateIceHands();
        //     studyHandManager.SetMagicBall(iceBall);
        //     SceneProvider.handSetting = HandSetting.ICE_HANDS;
        //     SceneProvider.lastActionHandSetting = HandSetting.ICE_HANDS;
        // }
        // else if (handSetting == HandSetting.NEUTRAL_HANDS)
        // {
        //     studyHandManager.ActivateNeutralHands();
        //     SceneProvider.handSetting = HandSetting.NEUTRAL_HANDS;
        // }
    }
}
