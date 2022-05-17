using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JumpAbility { None = 0, DoubleJump = 1, Float = 2, Blink = 3, Roll = 4 }
public class BattleMovement : MonoBehaviour
{
    protected BattleActor battleActor;
    protected BattleMidpoint midpoint;

    [Header ("Static Stats")]
    [SerializeField] protected float groundMaxSpeed;
    [SerializeField] protected float airMaxSpeed;
    [SerializeField] protected float groundAccel;
    [SerializeField] protected float airAccel;
    [SerializeField] protected float groundFriction;
    [SerializeField] protected float airFriction;
    [SerializeField] protected float leaveGroundMax;

    [Header("Momentum")]
    [SerializeField] protected bool directionLocked = false;
    [SerializeField] protected bool influenceMomentum = true;

    [SerializeField] protected int dir = -1;
    [SerializeField] protected bool ignoreMaxSpeed;
    [SerializeField] protected float currentXSpeed;
    [SerializeField] protected float currentYSpeed;
    float prevY;

    Vector3 pos { get { return transform.localPosition; } }

    [HideInInspector] public float HorInput;
    [HideInInspector] public float VerInput;
    [HideInInspector] public bool TapHoriz;
    [HideInInspector] public bool JumpInput;
    [HideInInspector] public bool JumpInputDown;
    [HideInInspector] public bool GuardInput;
    [HideInInspector] public bool GuardInputDown;

    protected float jumpInputBufferTimer = 0f;
    protected float jumpInputBuffer = 0.15f;

    protected bool guardInputLock = false;
    protected bool canGuard = true;

    [Header ("Gravity")]
    public bool affectedByGravity = true;
    [SerializeField] protected bool ignoreGravity = false;
    /*[SerializeField] */protected bool ignoreMaxFall = false;
    public float gravity = 0.2f;
    public float maxFall = 0.5f;
    [SerializeField] protected bool bounceOffGround = false;
    [Range(0.2f, 1.2f)]
    [SerializeField] protected float bounceMass = 0.5f;
    [SerializeField] protected bool ignoreGroundCheck = false;

    [Header("Abilities")]
    [Header("Air Dash")]
    public bool canAirDash = true;
    public int maxAirDash = 1;
    public int airDashes;
    public float airDashSpeed;
    public float airDashTime;

    [Header ("Jumping")]
    public JumpAbility jumpAbility;
    public bool canJump = true; // Controls if jumping should be possible. GroundCheck() is used for determining if the player can jump in the current moment.
    public float jumpStartTime = 0.2f;
    public float jumpTime = 1.0f;
    public float jumpHSpeed = 0.42f;
    public float normalJumpHeight = 1.0f;
    public float shortHopHeight = 0.4f;
    public float doubleJumpTime = 0.35f;
    public float doubleJumpHeight = 0.18f;
    protected bool hasDoubleJump;

    [Header("Blink")]
    public float blinkDisappearSpeed = 0.03f;
    public float blinkDistance = 0.2f;
    public float blinkReappearDelay;
    public float blinkCooldown = 0.24f;

    [Header("Roll")]
    public float rollTime;
    public float rollSpeed;
    public float rollCooldown;

    protected Coroutine currentAction;

    protected void Awake()
    {
        battleActor = GetComponent<BattleActor>();
        midpoint = GameObject.Find("DistanceObject").GetComponent<BattleMidpoint>();
    }

    protected void Update()
    {
        if (jumpInputBufferTimer > 0f)
        {
            jumpInputBufferTimer -= Time.deltaTime;
        }
        prevY = pos.y;

        DirectionCheck();

        if(jumpAbility == JumpAbility.None)
        {
            hasDoubleJump = false;
        }

        Movement();

        transform.localPosition = new Vector3(Mathf.Clamp(pos.x - (currentXSpeed * dir) * Time.deltaTime, -GameManager.instance.battleManager.posClamp + midpoint.pos.x,
            GameManager.instance.battleManager.posClamp + midpoint.pos.x), pos.y + currentYSpeed, pos.z);
        GroundCheck();
        CeilCheck();
    }
    protected void FixedUpdate()
    {
        GroundCheck();
        CeilCheck();
    }

    protected void DirectionCheck()
    {
        if (!directionLocked)
        {
            if (HorInput > 0 && dir != -1)
            {
                dir = -1;
                currentXSpeed = -currentXSpeed;
            }
            else if (HorInput < 0 && dir != 1)
            {
                dir = 1;
                currentXSpeed = -currentXSpeed;
            }
        }
    }

    protected void Movement()
    {
        float friction = airFriction;
        float accel = airAccel;

        bool guarding = false;
        if (GuardInput && canGuard)
        {
            guarding = true;
        }
        else
        {
            guarding = false;
        }
        guardInputLock = guarding;

        CeilCheck();

        if (!GroundCheck())
        {
            // gravity happens if "affectedByGravity" is on and "ignoreGravity" is off.
            if (affectedByGravity && !ignoreGravity)
            {
                currentYSpeed -= gravity * Time.deltaTime;
            }
            if (JumpInputDown)
            {
                if (hasDoubleJump && CanJump())
                {
                    if (jumpInputBufferTimer <= 0)
                    {
                        EndCurrentAction();
                    }
                    switch (jumpAbility)
                    {
                        case JumpAbility.DoubleJump:
                            currentAction = StartCoroutine(Jump(doubleJumpTime * 0.75f, doubleJumpHeight, doubleJumpHeight, true));
                            break;
                        case JumpAbility.Float:
                            currentAction = StartCoroutine(Float());
                            break;
                        case JumpAbility.Blink:
                            currentAction = StartCoroutine(Blink());
                            break;
                        case JumpAbility.Roll:
                            currentAction = StartCoroutine(DodgeRoll());
                            break;
                    }
                }
            }
        }
        else
        {
            friction = groundFriction;
            accel = groundAccel;

            airDashes = 0;

            if(jumpAbility != JumpAbility.None)
            {
                hasDoubleJump = true;
            }
            if (JumpInputDown)
            {
                if (VerInput < 0 && currentAction == null)
                {
                    if (CanJump())
                    {
                        switch (jumpAbility)
                        {
                            default:
                                currentAction = StartCoroutine(Jump(jumpTime, normalJumpHeight, shortHopHeight, false));
                                break;
                            case JumpAbility.Blink:
                                currentAction = StartCoroutine(Blink());
                                break;
                            case JumpAbility.Roll:
                                currentAction = StartCoroutine(DodgeRoll());
                                break;
                        }
                    }
                }
                else
                {
                    if (CanJump())
                    {
                        currentAction = StartCoroutine(Jump(jumpTime, normalJumpHeight, shortHopHeight, false));
                    }
                }
            }
        }
        if (currentYSpeed < -maxFall && !ignoreMaxFall)
        {
            currentYSpeed = -maxFall;
        }

        float maxSpeed = groundMaxSpeed;
        if (!GroundCheck())
        {
            maxSpeed = airMaxSpeed;
        }
        if (ignoreMaxSpeed)
        {
            maxSpeed = 5;
        }
        float inputCheck = HorInput;
        if(inputCheck < 0)
        {
            inputCheck = -inputCheck;
        }
        maxSpeed = maxSpeed * inputCheck;

        if (guarding)
        {
            if (!GroundCheck())
            {
                if (canAirDash && airDashes < maxAirDash)
                {
                    if (TapHoriz)
                    {
                        EndCurrentAction();
                        currentAction = StartCoroutine(AirDash());
                    }
                }
            }
        }

        if (CanInfluenceMovement())
        {
            if (!directionLocked)
            {
                if (HorInput > 0 || HorInput < 0)
                {
                    if (currentXSpeed >= 0)
                    {
                        currentXSpeed = Mathf.Clamp(currentXSpeed + (accel - friction * (HorInput * dir) * Time.deltaTime), -maxSpeed, maxSpeed);
                    }
                    else
                    {
                        currentXSpeed = Mathf.Clamp(currentXSpeed + (accel + friction * (HorInput * dir) * Time.deltaTime), -maxSpeed, maxSpeed);
                    }
                }
                else
                {
                    if (currentXSpeed >= 0)
                    {
                        currentXSpeed = Mathf.Clamp(currentXSpeed - friction, 0, maxSpeed);
                    }
                    else
                    {
                        currentXSpeed = Mathf.Clamp(currentXSpeed + friction, -maxSpeed, 0);
                    }
                }
            }
            else
            {
                if (HorInput > 0)
                {
                    if (dir == 1)
                    {
                        currentXSpeed = Mathf.Clamp(currentXSpeed - (accel + friction * (HorInput * dir) * Time.deltaTime), -maxSpeed, maxSpeed);
                    }
                    else
                    {
                        currentXSpeed = Mathf.Clamp(currentXSpeed + (accel - friction * (HorInput * dir) * Time.deltaTime), -maxSpeed, maxSpeed);
                    }
                }
                else if (HorInput < 0)
                {
                    if (dir == 1)
                    {
                        currentXSpeed = Mathf.Clamp(currentXSpeed + (accel + friction * (HorInput * dir) * Time.deltaTime), -maxSpeed, maxSpeed);
                    }
                    else
                    {
                        currentXSpeed = Mathf.Clamp(currentXSpeed - (accel - friction * (HorInput * dir) * Time.deltaTime), -maxSpeed, maxSpeed);
                    }
                }
                else
                {
                    if (currentXSpeed >= 0)
                    {
                        currentXSpeed = Mathf.Clamp(currentXSpeed - friction, 0, maxSpeed);
                    }
                    else
                    {
                        currentXSpeed = Mathf.Clamp(currentXSpeed + friction, -maxSpeed, 0);
                    }
                }
            }
        }
        else
        {
            if (currentXSpeed >= 0)
            {
                currentXSpeed = Mathf.Clamp(currentXSpeed - friction, 0, maxSpeed);
            }
            else
            {
                currentXSpeed = Mathf.Clamp(currentXSpeed + friction, -maxSpeed, 0);
            }
        }
    }

    protected bool CanInfluenceMovement()
    {
        if (!influenceMomentum || guardInputLock)
        {
            return false;
        }
        return true;
    }
    protected bool CanJump()
    {
        if (!canJump || guardInputLock)
        {
            return false;
        }
        return true;
    }

    protected bool GroundCheck()
    {
        if (ignoreGroundCheck)
        {
            return false;
        }
        if (pos.y < GameManager.instance.battleManager.groundVal)
        {
            transform.localPosition = new Vector3(pos.x, GameManager.instance.battleManager.groundVal, pos.z);
        }
        if (currentYSpeed > 0)
        {
            return false;
        }
        if (pos.y <= GameManager.instance.battleManager.groundVal)
        {
            if (bounceOffGround)
            {
                if(currentYSpeed < 0)
                {
                    currentYSpeed = Mathf.Clamp(-currentYSpeed * bounceMass, -GameManager.instance.battleManager.maxKnockbackSpeed, GameManager.instance.battleManager.maxKnockbackSpeed);
                    if (currentYSpeed < 0.02f)
                    {
                        currentYSpeed = 0f;
                        bounceOffGround = false;
                    }
                }
                else
                {
                    bounceOffGround = false;
                }
            }
            else
            {
                if(currentAction == null)
                {
                    ignoreMaxSpeed = false;
                    canGuard = true;
                }
                currentYSpeed = 0f;
                if(currentAction == null)
                {
                    directionLocked = false;
                }
            }
            return true;
        }
        return false;
    }
    protected bool CeilCheck()
    {
        if (pos.y > GameManager.instance.battleManager.ceilVal)
        {
            transform.localPosition = new Vector3(pos.x, GameManager.instance.battleManager.ceilVal, pos.z);
        }
        if (currentYSpeed < 0)
        {
            return false;
        }
        if (pos.y >= GameManager.instance.battleManager.ceilVal)
        {
            if (GameManager.instance.battleManager.useCeil)
            {
                if(currentYSpeed > -0.02f)
                {
                    if (currentYSpeed > 0)
                    {
                        currentYSpeed = Mathf.Clamp(-currentYSpeed * bounceMass, -GameManager.instance.battleManager.maxKnockbackSpeed, GameManager.instance.battleManager.maxKnockbackSpeed);
                    }
                    else
                    {
                        bounceOffGround = false;
                    }
                }
            }
            return true;
        }
        return false;
    }

    protected IEnumerator Jump(float jumpTime, float jumpHeight, float shortHeight, bool ignoreBuffer)
    {
        if (jumpInputBufferTimer > 0f)
        {
            yield break;
        }
        jumpInputBufferTimer = jumpInputBuffer;
        if (!CanJump())
        {
            yield break;
        }

        hasDoubleJump = false;
        canGuard = true;

        influenceMomentum = false;
        directionLocked = true;
        float storedSpeed = currentXSpeed;
        float inputBuffer = 0f;
        if (!ignoreBuffer)
        {
            while (inputBuffer < jumpStartTime)
            {
                inputBuffer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        if (!JumpInput)
        {
            jumpHeight = shortHeight;
            jumpTime = jumpTime * 0.75f;
        }
        int holdDir = 0;
        if ((HorInput > 0 || HorInput < 0))
        {
            if (HorInput > 0 && dir == 1 || HorInput < 0 && dir == -1)
            {
                holdDir = -1;
            }
            else
            {
                holdDir = 1;
            }
        }
        currentXSpeed = Mathf.Clamp(((storedSpeed + jumpHSpeed) * holdDir) + ((airAccel * 6) * (dir * holdDir) * Time.deltaTime), -leaveGroundMax, leaveGroundMax);
        influenceMomentum = true;

        float jumpTimer = 0;
        ignoreGravity = true;
        float jumpStartVal = pos.y;
        
        while (jumpTimer < jumpTime)
        {
            jumpTimer += Time.deltaTime;
            float prevHeight = pos.y;
            float jumpSpeed = Mathf.Lerp(jumpStartVal, jumpStartVal + jumpHeight, jumpTimer / jumpTime); 
            prevY = pos.y;
            transform.localPosition = new Vector3(pos.x, jumpSpeed, pos.z);
            currentYSpeed = 0;
            if (CeilCheck())
            {
                jumpTimer = jumpTime;
            }
            yield return new WaitForEndOfFrame();
        }
        currentYSpeed = (pos.y-prevY);
        ignoreGravity = false;
        EndCurrentAction(); 
    }
    protected IEnumerator Float()
    {
        if (jumpInputBufferTimer > 0f)
        {
            yield break;
        }
        jumpInputBufferTimer = jumpInputBuffer;
        if (!CanJump())
        {
            yield break;
        }

        hasDoubleJump = false;
        canGuard = false;

        influenceMomentum = false;
        ignoreGravity = true;

        currentYSpeed = 0;

        yield return new WaitUntil(() => (JumpInputDown && jumpInputBufferTimer <= 0f));

        ignoreGravity = false;
        currentAction = StartCoroutine(Jump(jumpTime*0.5f, shortHopHeight/2, shortHopHeight/2, true));

        //canInfluenceMovement = false;
        yield break;
    }
    protected IEnumerator Blink()
    {
        if (jumpInputBufferTimer > 0f)
        {
            yield break;
        }
        jumpInputBufferTimer = jumpInputBuffer;
        if (!CanJump())
        {
            yield break;
        }

        hasDoubleJump = false;
        canJump = false;
        canGuard = false;

        influenceMomentum = false;
        ignoreGravity = true;
        ignoreGroundCheck = true;
        currentXSpeed = 0;
        currentYSpeed = 0;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        while (sr.color.a > 0f)
        {
            yield return new WaitForEndOfFrame();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - (blinkDisappearSpeed * Time.deltaTime));
        }

        ignoreGroundCheck = false;
        // Change character's position based on analog input
        transform.localPosition = new Vector3(pos.x + (blinkDistance * HorInput), pos.y + (blinkDistance * VerInput), pos.z);
        for (float wait = 0; wait < blinkReappearDelay; wait += Time.deltaTime)
        {
            yield return new WaitForEndOfFrame();
        }
        directionLocked = false;
        DirectionCheck();
        directionLocked = true;

        while (sr.color.a < 1f)
        {
            yield return new WaitForEndOfFrame();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a + (blinkDisappearSpeed * Time.deltaTime));
        }

        for (float recovery = 0; recovery < blinkCooldown; recovery += Time.deltaTime)
        {
            yield return new WaitForEndOfFrame();
        }

        influenceMomentum = true;
        ignoreGravity = false;
        canJump = true;
        canGuard = true;
        EndCurrentAction();
    }
    protected IEnumerator DodgeRoll()
    {
        if (jumpInputBufferTimer > 0f)
        {
            yield break;
        }
        jumpInputBufferTimer = jumpInputBuffer;
        if (!CanJump())
        {
            yield break;
        }

        ignoreGravity = false;
        canJump = false;
        canGuard = false;

        for (float rollTimer = 0; rollTimer < rollTime; rollTimer += Time.deltaTime)
        {
            if (GroundCheck())
            {
                ignoreMaxSpeed = true;
                influenceMomentum = false;
                currentXSpeed = rollSpeed;
            }
            else
            {
                ignoreMaxSpeed = false;
                influenceMomentum = true;
            }
            yield return new WaitForEndOfFrame();
        }
        ignoreMaxSpeed = false;
        for (float recovery = 0; recovery < rollCooldown; recovery += Time.deltaTime)
        {
            if (GroundCheck())
            {
                influenceMomentum = false;
            }
            else
            {
                influenceMomentum = true;
            }
            yield return new WaitForEndOfFrame();
        }

        canJump = true;
        canGuard = true;
        influenceMomentum = true;
        EndCurrentAction();
    }
    protected IEnumerator AirDash()
    {
        directionLocked = false;
        DirectionCheck();
        directionLocked = true;

        airDashes++;

        canJump = true;
        canGuard = false;
        ignoreMaxSpeed = true;
        ignoreGravity = true;
        influenceMomentum = false;
        currentXSpeed = airDashSpeed;
        for (float dashTimer = 0; dashTimer < airDashTime; dashTimer += Time.deltaTime)
        {
            currentYSpeed = 0f;
            if (HorInput < 0.5f && HorInput > -0.5f)
            {
                dashTimer = airDashTime;
            }
            yield return new WaitForEndOfFrame();
        }
        ignoreGravity = false;
        influenceMomentum = true;
        canGuard = true;
        //ignoreMaxSpeed = false;
        EndCurrentAction();
    }

    protected void EndCurrentAction()
    {
        if(currentAction == null)
        {
            return;
        }
        StopCoroutine(currentAction);
        currentAction = null;
    }
}
