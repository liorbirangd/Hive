﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
public class ParamManager : MonoBehaviour
{
    public static ParamManager Instance;//singelton

    public bool nonPlayerMaskActive = false;

    [Header("Tags")]
    
    [SerializeField] private string playerTag = "Player";
    public string PLAYERTAG { get { return playerTag; } }

    [SerializeField] private string enemyTag = "Enemy";
    public string ENEMYTAG { get { return enemyTag; } }

    [SerializeField] private string planetTag = "Planet";
    public string PLANETTAG { get { return planetTag; } }
    
    [SerializeField] private string linkTag = "Planet";
    public string LINKTAG { get { return linkTag; } }
    
    [SerializeField] private string fogMaskTag = "FogMask";
    public string FOGMASKTAG { get { return fogMaskTag; } }
    
    [SerializeField] private string fogTag = "Fog";
    public string FOGTAG { get { return fogTag; } }
    
    [SerializeField] private string backgroundTag = "Background";
    public string BACKGROUNDTAG { get { return backgroundTag; } }
    
    [SerializeField] private string backgroundStarsTag = "Stars";
    public string BACKGROUNDSTARSTAG { get { return backgroundStarsTag; } }

    [SerializeField] private string capturePoolerTag = "Capture";
    public string CAPTUREPOOLTAG { get { return capturePoolerTag; } }
     
    [SerializeField] private string reinforcementPoolerTag = "Reinforcement";
    public string REINFORCEMENTPOOLTAG { get { return reinforcementPoolerTag; } }
    
    [SerializeField] private string orbitTag = "Orbit";
    public string ORBITTAG { get { return orbitTag; } }
    
    [SerializeField] private string hihghlightTag = "Highlight";
    public string HIGHLIGHTTAG { get { return hihghlightTag; } }
    
    [SerializeField] private string borderTopLeft = "Border_Top_Left";
    public string BORDERTOPLEFT { get { return borderTopLeft; } }
    
    [SerializeField] private string borderBottomRight = "Border_Bottom_Right";
    public string BORDERBOTTOMRIGHT { get { return borderBottomRight; } }
    
    [SerializeField] private string levelButtonTag = "LevelButton";
    public string LEVELBUTTONTAG { get { return levelButtonTag; } }
    
    [SerializeField] private string colorSampleTag = "ColorSample";
    public string COLORSAMPLETAG { get { return colorSampleTag; } }

    [Header("Scene Names")]
    [SerializeField] private string levelSceneName = "Level";
    public string LEVELSCENENAME { get { return levelSceneName; } }
    
    [SerializeField] private string levelTutorialSceneName = "LevelTutorial";
    public string LevelTutorialSceneName { get { return levelTutorialSceneName; } }
    
    [SerializeField] private string gameOverSceneName = "GameOverScreen";
    public string GAMEOVERSCENENAME { get { return gameOverSceneName; } }
    
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    public string MAINMENUSCENENAME { get { return mainMenuSceneName; } }
    
    [Header("Animation Triggers")]
    [SerializeField] private string planetReversseAnimationBool = "isReverse";
    public string PlanetReversseAnimationBool { get { return planetReversseAnimationBool; } }
    [SerializeField] private string highlightEnableBool = "isHighlight";
    public string HighlightEnableBool { get { return highlightEnableBool; } }
    [Header("Mixer Parameters")]
    [SerializeField] private string gameVolumeName = "MusicVolume";
    public string GameVolumeName { get { return gameVolumeName; } }
    [SerializeField] private string scarySoundName = "ScarySoundVolume";
    public string ScarySoundName { get { return scarySoundName; } }
     [SerializeField] private string sfxSoundName = "SFXVolume";
    public string SfxSoundName { get { return sfxSoundName; } }

    [Header("Texts")]
    [SerializeField] private string winText = "Assimilation Comlete";
    public string WinText { get { return winText; } }
    [SerializeField] private string looseText = "Assimilation Failed";
    public string LooseText { get { return looseText; } }

    [Header("Prefabs")]
    [SerializeField] private TextMeshPro strengthDisplayPrefab;
    public TextMeshPro StrengthDisplayPrefab { get { return strengthDisplayPrefab; } }
    [Header("Materials")]
    [SerializeField] private Material linkActiveMaterial;
    public Material LinkActiveMaterial { get { return linkActiveMaterial; } }
    [SerializeField] private Material linkInactiveMaterial;
    public Material LinkInactiveMaterial { get { return linkInactiveMaterial; } }
    [Header("Images")]
    [SerializeField] private Sprite pauseButtonSprite;
    public Sprite PauseButtonSprite { get { return pauseButtonSprite; } }
    [SerializeField] private Sprite unpauseButtonSprite;
    public Sprite UnpauseButtonSprite { get { return unpauseButtonSprite; } }

    [Space(3)]
    [Header ("Global Parameters")]

    [SerializeField] private Color playerColor = new Color(0x76, 0xAD, 0x75);
    public Color PlayerColor { get { return playerColor; }set { playerColor = value; } }

    [SerializeField] private Color enemyColor = new Color(0xB7, 0x52, 0x5C);
    public Color EnemyColor { get { return enemyColor; } set { enemyColor = value; } }

    [SerializeField] private Color neutralColor = new Color(0x44, 0x37, 0x38);
    public Color NeutralColor { get { return neutralColor; } set { neutralColor = value; } }

    [SerializeField] private Color playerHighlightColor = new Color(0x76, 0xAD, 0x75);
    public Color PlayerHighlightColor { get { return playerHighlightColor; } set { playerHighlightColor = value; } }

    [SerializeField] private Color enemyHighlightColor = new Color(0xB7, 0x52, 0x5C);
    public Color EnemyHighlightColor { get { return enemyHighlightColor; } set { enemyHighlightColor = value; } }

    [SerializeField] private Color neutralHighlightColor = new Color(0x44, 0x37, 0x38);
    public Color NeutralHighlightColor { get { return neutralHighlightColor; } set { neutralHighlightColor = value; } }

    [Header ("Capture Parameters")]
    [SerializeField] private float strengthUpdateRate = 0.8f;
    public float StrengthUpdateRate { get { return strengthUpdateRate; } }

    [SerializeField] private float captureStrengthOutcome = 3;
    public float CaptureStrengthOutcome { get { return captureStrengthOutcome; } }
           
    [SerializeField] private float zeroStrengthReducedOutcome = 1;
    public float ZeroStrengthReducedOutcome { get { return zeroStrengthReducedOutcome; } }

    [SerializeField] private float captureImunityTime = 3f;
    public float CaptureImunityTime { get { return captureImunityTime; } }

    [SerializeField] private float reinforceBonus = 1f;
    public float ReinforceBonus { get { return reinforceBonus; } }
    
    [SerializeField] private float reinforceCost = 0.5f;
    public float ReinforceCost { get { return reinforceCost; } }
    [SerializeField] private float strengthCap = 100f;
    public float StrengthCap { get { return strengthCap; } }

    [Header ("Planet Parameters")]
    public PlanetSizeParameters[] planetSizeSet =new PlanetSizeParameters[3];
    

    [Header("Planet Animations")]
    [SerializeField][Range(0,1f)] private float animationMinSpeed = 0.3f;
    public float AnimationMinSpeed { get { return animationMinSpeed; } }
     
    [SerializeField][Range(0,1f)] private float animationMaxSpeed = 0.8f;
    public float AnimationMaxSpeed { get { return animationMaxSpeed; } }

    [SerializeField] public List<RuntimeAnimatorController> planetAnimations;

    [SerializeField] private int completeLeveles = 3;
    public float CompleteLeveles { get { return completeLeveles; } }



    private void Awake()
    {
        SetSingelton();
    }
    private void SetSingelton()
    {
        if (Instance != null && Instance != this)// implement singelton
        {
            if (Application.isPlaying)
                Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            if(Application.isPlaying)
                DontDestroyOnLoad(this.gameObject);
        }
    }
    private void OnValidate()
    {
        SetSingelton();
    }
    [Serializable]
    public class PlanetSizeParameters
    {
        public Planet.PlanetSize size;
        public float strengthIncome = 2;
        public int maxActiveLinks = 2;
        public float orbitCycleTime = 5f;
        public float captureRange = 30;
        public float visibilityRange = 150;
        public float planetScale = 1;
    }
}
