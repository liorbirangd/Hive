﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;


public class Planet : MonoBehaviour
{
    private static int IDCount = 0;

    //parameters
    [SerializeField] private HiveController.Hive hiveType = HiveController.Hive.Neutral;
    public HiveController.Hive HiveType { get { return hiveType; } }
    [SerializeField] private bool isQueen = false;
    [SerializeField] private int startingStrength = 6;
    [SerializeField] private PlanetSize planetSize = PlanetSize.Small;

    [SerializeField] private float strengthIncome = 2;
    [SerializeField] private int maxConnections = 2;
    [SerializeField] private float speed = 5f;
    [SerializeField] [Range(5f, 20f)] private float captureRange = 30;
    [SerializeField] [Range(100f, 300f)] private float visibilityRange = 150;
    [SerializeField] [Range(1, 5)] private int maximumActiveCaptures = 2;

    public int PlanetID { get; private set; } = 0;

    //state variables
    private float strength = 0;
    //private List<Capture> planetsInCaptureInteraction = new List<Capture>();
    private List<Capture> captureLinks = new List<Capture>();
    private List<Link> activeLinks = new List<Link>();
    private List<Reinforcement> reinforceLinks = new List<Reinforcement>();

    private float captureImunity;
    public bool IsImune { get { return captureImunity <= 0; } }
    

    //references
    private TextMeshPro _strengthDisplay = null;
    private SpriteRenderer _spriteRenderer = null;
    private Coroutine strenghtGrowthCoroutine;
    private HiveController HiveRef //dynamic HiveControllerReference based on controllingHive
    {
        get
        {
            if (hiveType == HiveController.Hive.Player) return HiveController.Player;
            else if (hiveType == HiveController.Hive.Enemy) return HiveController.Enemy;
            else return null;
        }
    }

    void Start()
    {
        PlanetID = Planet.IDCount;//set id
        PlanetID++;//increase id count
        _strengthDisplay = Instantiate(ParamManager.Instance._StrengthDisplayPrefab);//create strength display
        _spriteRenderer = GetComponent<SpriteRenderer>();//sprite renderer reference
        strength = startingStrength;//st starting strength
        strenghtGrowthCoroutine = StartCoroutine(GenerateStrength());//start strength and save reference
        SetStartInHive();
        captureImunity = ParamManager.Instance.CaptureImunityTime;
    }
    void Update()
    {
        UpdateStrengthDisplay();
    }
    private void SetStartInHive()//add to hive and set color
    {
        if (isQueen&&HiveRef)
                HiveRef.Queen = this;
        ChangeHive(hiveType);
    }

    private void UpdateStrengthDisplay()// update text and pin to planet
    {
        _strengthDisplay.text = strength + "";
        _strengthDisplay.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }
    IEnumerator GenerateStrength()//increase strength
    {
        while (true)
        {
            if (GameManager.Instance.IsPaused)//pause game
                yield return new WaitUntil(() => !GameManager.Instance.IsPaused);
            yield return new WaitForSeconds(ParamManager.Instance.StrengthUpdateRate);//delay between strength ticks
            strength += CalculateDeltaStrength();
            foreach (Capture c in planetsInCaptureInteraction)//for each capture interaction planet count contribution of capture
            {
                c.strengthCaptured += c.planet.strength > 0 ? ParamManager.Instance.CaptureStrengthOutcome
                    : ParamManager.Instance.ZeroStrengthReducedOutcome;//count less contribution if other has 0 strength
            }
            if (strength <= 0)//if reach zero or below get captured
            {
                strength = 0;
                GetCaptured();
            }
        }
    }


    private float CalculateDeltaStrength()// determines strengrh change per strengt tick
    {
        float deltaStrengt = CalculateStrengthIncome() - CalculateStrengthOutcome();
        return deltaStrengt;
    }
    private float CalculateStrengthIncome()//planet's passive income
    {
        if(HiveRef)
            return strengthIncome;
        return 0;
    }
    private float CalculateStrengthOutcome()//capture cost for each capture interaction
    {
        int outcome = 0;
        int zeroPlanets = planetsInCaptureInteraction.Where(capture => capture.planet.strength <= 0).Count();//count planets with 0 strength
        outcome += zeroPlanets * ParamManager.Instance.ZeroStrengthReducedOutcome;//planets with 0 strength contribute less to outcome
        outcome += ParamManager.Instance.CaptureStrengthOutcome * (planetsInCaptureInteraction.Count() - zeroPlanets);//planets with strength contribute to outcopme
        return outcome;
    }
   
    public void AttemptCapture(Planet captured)//start capture of other planet
    {
        //check if this planet in hive and if other planet is not in this hive
        if (hiveType != HiveController.Hive.Neutral && hiveType != captured.hiveType)
        {
            Capture newLink = new Capture(this, captured);
            if (!captureLinks.Any(c => c.CompareTo(newLink))) //if not already in capture link list
            {
                captureLinks.Add(newLink);// add capture link to list
                captured.UnderCapture(newLink);//tell other planet they are under capture
            }
        }
    }
    private void UnderCapture(Capture newLink)//become under capture by another planet
    {
        if (captureLinks.Any(c => c.CompareTo(newLink))) //if not already in capture interaction with other
        {
            captureLinks.Add(newLink);// add capture link to list
        }
    }
    private void GetCaptured()//get captured by the hive that captured for the most time.
    {
        if (captureLinks.Where(c => c.target == this).Any(c => c.origin.hiveType != HiveController.Hive.Neutral))//if any non neutral capturer in link that this is the target
        {
            //determine hive that captured this planet for the longest among active links
            int playerCaptureDuration = 0;
            int enemyCaptureDuration = 0;
            foreach (Capture link in captureLinks.Where(c => c.target == this).Where(c => c.isActive))//count links where this is the target and are active links
            {
                if (link.origin.hiveType == HiveController.Hive.Player)//count strength taken by player
                {
                    playerCaptureDuration += link.strengthCaptured;
                }
                else if (link.origin.hiveType == HiveController.Hive.Enemy)//count strength taken by enemy
                {
                    enemyCaptureDuration += link.strengthCaptured;
                }
            }
            if ((playerCaptureDuration <= 0 && enemyCaptureDuration <= 0) || playerCaptureDuration == enemyCaptureDuration)// if no duration or equal duration count all links including non active
            {
                foreach (Capture link in captureLinks.Where(c => c.target == this).Where(c => !c.isActive))//count links where this is the target and are not active links
                {
                    if (link.origin.hiveType == HiveController.Hive.Player)//count strength taken by player
                    {
                        playerCaptureDuration += link.strengthCaptured;
                    }
                    else if (link.origin.hiveType == HiveController.Hive.Enemy)//count strength taken by enemy
                    {
                        enemyCaptureDuration += link.strengthCaptured;
                    }
                }
            }
            //check for winner if no duration or equal player wins
            
            if (playerCaptureDuration >= enemyCaptureDuration)//player win if equal
                ChangeHive( HiveController.Hive.Player);
            else
                ChangeHive(HiveController.Hive.Enemy);
            FinishCaptureInteraction();
        }
    }

    private void FinishCaptureInteraction()//remove other from interactions list
    {
        List<Capture> remove = captureLinks.Where(c => c.origin == this || (c.target == this && c.origin.HiveType!=this.HiveType)).ToList();//make a list of links to remove
        List<Capture> convert = captureLinks.Where(c => c.target == this && c.origin.HiveType == this.HiveType).ToList();//make list of links to convert to reinforcement
        captureLinks.RemoveAll(c => c.origin == this || (c.origin != this && c.origin.hiveType != this.hiveType));
        Capture c = planetsInCaptureInteraction.Where(capture => capture.planet == other).ElementAt(0);//find Capture
        planetsInCaptureInteraction.Remove(c);//remove from interaction list
    }
    public bool IsWithinCaptureRange(Planet captured)
    {
        return (Vector2.Distance(transform.position, captured.transform.position) <= captureRange);
    }

    private void ChangeHive(HiveController.Hive hive)
    {
        if (HiveRef)//remove from current hive
            HiveRef.RemovePlanet(this);
        hiveType = hive;
        if (HiveRef)//check if in hive
        {
            HiveRef.AddPlanet(this);
            _spriteRenderer.color = HiveRef.HiveColor;
        }
        else
        {
            _spriteRenderer.color = ParamManager.Instance.NeutralColor;
        }

    }
    public void HighlightPlanet()
    {
        if (HiveRef)
            _spriteRenderer.color = HiveRef.HiveHighlightColor;
        else
            _spriteRenderer.color = ParamManager.Instance.NeutralHighlightColor;
    }
    public void RemoveHighlightPlanet()
    {
        if (HiveRef)
            _spriteRenderer.color = HiveRef.HiveColor;
        else
            _spriteRenderer.color = ParamManager.Instance.NeutralColor;
    }
    

    public enum PlanetSize
    {
        Small=0,
        Medium=1,
        Big=2
    }
}
