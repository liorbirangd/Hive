﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    private MouseInteractable currentHover;
    private Planet currentClickedPlanet;

    private void CheckforMouseHover()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());//get mouse position
        mousePosition.z = -5;
        Ray ray = new Ray(mousePosition, Vector3.forward);
        List<RaycastHit2D> hits = new List<RaycastHit2D>(Physics2D.GetRayIntersectionAll(ray));//raycast to hit planets
        if (hits.Any(hit => hit.collider.CompareTag(ParamManager.Instance.FOGMASKTAG)))//check if planet is inside visability fog mask
        {
            RaycastHit2D hover = (hits.FirstOrDefault(hit => hit.collider.GetComponent<Planet>()));//check if hit Planet
            if(!hover)
                hover = (hits.FirstOrDefault(hit => hit.collider.GetComponent<MouseInteractable>()));//check if hit IMouseHover
            if (hover)//if not null trigger mouseHover
            {
                OnMouseHover(hover.collider.GetComponent<MouseInteractable>());
                return;
            }
        }
        OnMouseHover(null);//if not inside mask or not on IMouseHover remove last hover
    }
    private void OnMouseHover(MouseInteractable hover)//when hover over planet or link
    {
        if (hover != currentHover)//if hover something other then current or hover null
        {
            if (currentHover)//if previously hover something unhover
                currentHover.UnHoverObject();
            if (hover) //if planet isnt null and is valid
            {
                currentHover = hover;//set current to new hover or null
                currentHover.HoverObject();
            }
            else
                currentHover = null;
        }
    }
    public void OnClickObject(InputAction.CallbackContext context)//when left click on planet
    {
        if (context.performed&& !GameManager.Instance.IsPaused)
        {
            if (currentClickedPlanet)//if already have clicked planet
            {
                if (currentHover && currentHover is Planet)//if click target planet
                {
                    if (currentHover == currentClickedPlanet)//if clicked current clicked -- do nothing
                    {
                        //Unclick();
                    }
                    else if (((Planet)currentHover).HiveType != HiveController.Hive.Player)// if hovering non-player planet start capture
                    {
                        //Debug.Log("Capture---Origin: " + currentClickedPlanet.GetInstanceID()+"--> Target:"+currentHover.GetInstanceID());
                        HiveController.Player.CapturePlanet(currentClickedPlanet, ((Planet)currentHover));
                        Unclick();
                    }
                    else// if hovering player planet start reinforce
                    {
                        //Debug.Log("Reinforce---Origin: " + currentClickedPlanet.GetInstanceID() + "--> Target:" + currentHover.GetInstanceID());
                        HiveController.Player.ReinforcePlanet(currentClickedPlanet, ((Planet)currentHover));
                        Unclick();
                    }
                }
            }
            else //if not already clicked make hovered clicked
            {
                if (currentHover && currentHover is Planet &&
                    ((Planet)currentHover).HiveType == HiveController.Hive.Player)//if click player planet set current hovered to clicked
                {
                    currentClickedPlanet = (Planet)currentHover;
                    currentClickedPlanet.ClickObject();
                    AudioManager.Instance.OnClickPlanet();
                }
                else
                    currentClickedPlanet = null;
            }
        }
    }
    private void Unclick()
    {
        if (currentClickedPlanet)
        {
            currentClickedPlanet.UnClickObject();
            currentClickedPlanet = null;
        }
    }
    public void UnclickLostPlanet(Planet planet)
    {
        if (currentClickedPlanet&&currentClickedPlanet==planet)
        {
            Unclick();
        }
    }
    public void OnCancelLink(InputAction.CallbackContext context)//when right click on planet or link
    {
        if (context.performed&&!GameManager.Instance.IsPaused)
        {
            if (currentHover)//if currently hovering something
            {
                if (currentHover is Link)//if click on link remove this link
                    HiveController.Player.RemoveLink((Link)currentHover);
                else if (currentHover is Planet)//if click on planet
                {
                    Planet planet = (Planet)currentHover;
                    if (planet.HiveType == HiveController.Hive.Player)//if click on player planet remove all links from origin
                        HiveController.Player.RemoveAllLinksOfPlanet(planet);
                    else//if click on non player remove all links towards target
                    {
                        Debug.Log("Remove to target");
                        HiveController.Player.RemoveAllLinksToEnemy(planet);
                    }
                }
            }
            else// if right click on empty
            {
                if (currentClickedPlanet)//if clicked planet unclick current
                {
                    currentClickedPlanet.UnClickObject();
                    currentClickedPlanet = null;
                }
            }
        }
    }

    void Update()
    {
        if (!GameManager.Instance.IsPaused)
        {
            CheckforMouseHover();
        }
    }
}

