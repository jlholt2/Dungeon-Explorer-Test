using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(OWPlayerController))]
public class OWPlayerInput : MonoBehaviour
{
    public bool acceptInputs = true;
    
    private bool notDetectingInputs = false; // An alternative input check reserved for when there is no gamepad present.

    OWPlayerController controller;
    OWPlayerLook look;

    [SerializeField] float deadzone = 0.00002f;

    private void Awake()
    {
        controller = GetComponent<OWPlayerController>();
        look = GetComponent<OWPlayerLook>();
    }

    private void Update()
    {
        var gamepad = Gamepad.current;
        if(gamepad == null)
        {
            notDetectingInputs = true;
        }
        else
        {
            notDetectingInputs = false;
        }

        if (acceptInputs && !notDetectingInputs)
        {
            if (gamepad.leftStick.ReadValue().y > deadzone) controller.MoveForward();
            if (gamepad.leftStick.ReadValue().y < -deadzone) controller.MoveBackward();
            if (gamepad.leftStick.ReadValue().x < -deadzone) controller.MoveLeft();
            if (gamepad.leftStick.ReadValue().x > deadzone) controller.MoveRight();
            if (gamepad.rightStick.ReadValue().y > deadzone) look.LookUp();
            if (gamepad.rightStick.ReadValue().y < -deadzone) look.LookDown();
            if (gamepad.rightStick.ReadValue().y < deadzone && gamepad.rightStick.ReadValue().y > -deadzone) look.LookNeutral();
            if (gamepad.leftShoulder.isPressed) controller.RotateLeft();
            if (gamepad.rightShoulder.isPressed) controller.RotateRight();
            if (gamepad.buttonSouth.isPressed) look.LookZoom();
            if (!gamepad.buttonSouth.isPressed) look.LookNoZoom();
            if (controller.DetectEvent())
            {
                // check if event auto activates, and if so, activate the event
                // otherwise, the player can press a button to trigger the event with currentEvent.DoTrigger();
                if (gamepad.buttonSouth.wasPressedThisFrame)
                {
                    if (controller.StandingEvent != null)
                    {
                        if (!controller.StandingEvent.autoActivate && controller.StandingEvent.activationRange == EventActivationRange.StandOn)
                        {
                            StandingEventTrigger();
                        }
                    }
                    if (controller.FrontEvent != null)
                    {
                        if (!controller.FrontEvent.autoActivate && controller.FrontEvent.activationRange == EventActivationRange.Touch)
                        {
                            FrontEventTrigger();
                        }
                    }
                }
            }
        }
    }

    private void StandingEventTrigger()
    {
        controller.StandingEvent.AddEventsToQueue();
    }
    private void FrontEventTrigger()
    {
        controller.FrontEvent.AddEventsToQueue();
    }
}
