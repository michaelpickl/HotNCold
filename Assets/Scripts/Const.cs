using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const
{
    public static class Events
    {
        public const string TargetHit = "TARGET_HIT";
        public const string GraphicCsSubmitted = "GRAPHIC_CS_SUBMITTED";
        public const string TutorialCompleted = "TUTORIAL_COMPLETED";
        public const string TutorialSliderCompleted = "TUTORIAL_SLIDER_COMPLETED";
        public const string BallThrown = "BALL_THROWN";
        public const string SceneConditionFulfilled = "SCENE_CONDITION_FULFILLED";
    }

    public static class Scenes
    {
        public const string IceWorld = "IceWorld";
        public const string FireWorld = "FireWorld";
        public const string NeutralWorld = "NeutralScene";
        public const string Intro = "Intro";
        public const string IceLightingScene = "IceLightingScene";
        public const string FireLightingScene = "FireLightingScene";
        public const string DefaultLightingScene = "DefaultLightingScene";
        public const string NeutralLightingScene = "NeutralLightingScene";
    }
}
