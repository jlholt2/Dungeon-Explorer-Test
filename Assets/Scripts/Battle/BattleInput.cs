using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleInput : MonoBehaviour
{
    public bool controlLock = false;

    [SerializeField] bool tapJump = false;

    BattleActor battlePlayer;
    BattleMovement battleMovement;

    // Analog input readers
    float prevUp;
    float prevRight;
    [SerializeField] float currentUp = 0f;
    [SerializeField] float currentRight = 0f;

    [SerializeField] float deadzone = 0.00002f;

    private void Awake()
    {
        battlePlayer = GetComponent<BattleActor>();
        battleMovement = GetComponent<BattleMovement>();
    }

    private void Update()
    {
        var gamepad = Gamepad.current;
        if(gamepad == null)
        {
            controlLock = true;
        }
        else
        {
            controlLock = false;
        }

        prevUp = currentUp;
        prevRight = currentRight;

        if (!controlLock)
        {
            currentUp = gamepad.leftStick.ReadValue().y;
            currentRight = gamepad.leftStick.ReadValue().x;
        }
        else
        {
            currentUp = 0f;
            currentRight = 0f;
        }
        
        // Deadzone
        if (currentUp < deadzone && currentUp > 0 || currentUp > -deadzone && currentUp < 0)
        {
            currentUp = 0f;
        }
        if (currentRight < deadzone && currentRight > 0 || currentRight > -deadzone && currentRight < 0)
        {
            currentRight = 0f;
        }

        if (!GameManager.instance.battleManager.battleOn)
        {
            controlLock = true;
        }
        if (!controlLock)
        {
            battleMovement.HorInput = -currentRight;
            battleMovement.VerInput = currentUp;

            // Tap Jump Code
            bool tapJumpInput = false;
            bool tapJumpDownInput = false;
            if (tapJump)
            {
                if (currentUp > 0)
                {
                    tapJumpInput = true;
                }
                if (prevUp <= 0.1f && currentUp >= 0.9f)
                {
                    tapJumpDownInput = true;
                }
            }
            if (gamepad.buttonSouth.wasPressedThisFrame || tapJumpDownInput)
            {
                battleMovement.JumpInputDown = true;
            }
            else
            {
                battleMovement.JumpInputDown = false;
            }
            if (gamepad.buttonSouth.isPressed || tapJumpInput)
            {
                battleMovement.JumpInput = true;
            }
            else
            {
                battleMovement.JumpInput = false;
                battleMovement.JumpInputDown = false;
            }

            // Tap Horizontal
            if (prevRight < deadzone && currentRight >= 0.9f || prevRight > -deadzone && currentRight <= -0.9f)
            {
                battleMovement.TapHoriz = true;
            }
            else
            {
                battleMovement.TapHoriz = false;
            }

            // Guard
            if (gamepad.rightTrigger.ReadValue() > 0)
            {
                battleMovement.GuardInput = true;
            }
            else
            {
                battleMovement.GuardInput = false;
            }
        }
        else
        {
            battleMovement.HorInput = 0;
            battleMovement.VerInput = 0;
            battleMovement.JumpInput = false;
            battleMovement.GuardInput = false;
        }
    }
}
