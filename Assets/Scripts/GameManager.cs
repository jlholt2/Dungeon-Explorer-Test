using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [HideInInspector] public OWPlayerController owPlayer;
    public BattleActor battlePlayer;

    [SerializeField] public Camera dungeonCamera;
    [SerializeField] public Camera battleCamera;

    [SerializeField] public BattleManager battleManager;

    [SerializeField] private int dungeonFrameRate = 30;
    [SerializeField] private int battleFrameRate = 30;

    [Header("Game Data")]
    public int area = 1;
    public int floor = 1;
    public int keyRing = 0;
    public Dictionary<string, bool> eventSwitches;
    public Dictionary<string, int> eventVariables;

    public Vector2 spawnPos;

    public int FloorNum { get { return ((area - 1) * 10) + floor; } }

    [SerializeField] private int queuedEvents = 0;
    [SerializeField] private float deltaTime;

    #region Event Switches and Variables
    public bool EventSwitchExists(string eventID)
    {
        SetupEventSwitchDictionary();
        if (eventSwitches.ContainsKey(eventID))
        {
            return true;
        }
        return false;
    }
    public bool EventVariableExists(string eventID)
    {
        SetupEventVariableDictionary();
        if (eventVariables.ContainsKey(eventID))
        {
            return true;
        }
        return false;
    }
    public void SetupEventSwitchDictionary()
    {
        if (eventSwitches == null)
        {
            eventSwitches = new Dictionary<string, bool>();
        }
    }
    public void SetupEventVariableDictionary()
    {
        if (eventVariables == null)
        {
            eventVariables = new Dictionary<string, int>();
        }
    }
    #endregion

    public void SetFrameRate(string type)
    {
        switch (type)
        {
            case "battle":
                Application.targetFrameRate = battleFrameRate;
                break;
            case "dungeon":
                Application.targetFrameRate = dungeonFrameRate;
                break;
            default:
                break;
        }
    }

    void Awake()
    {
        SetFrameRate("dungeon");

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeCameras();

        battleManager = GameObject.Find("BattleBox").GetComponent<BattleManager>();

        //Debug.Log("Frame Rate: " + Application.targetFrameRate + ", timeScale: " + Time.timeScale);
    }

    void Start()
    {
        dungeonCamera.gameObject.SetActive(false);
        battleCamera.gameObject.SetActive(false);

        dungeonCamera.gameObject.SetActive(true);
    }

    void InitializeCameras()
    {
        dungeonCamera = GameObject.Find("Dungeon Camera").GetComponent<Camera>();
        battleCamera = GameObject.Find("Battle Camera").GetComponent<Camera>();
    }

    void Update()
    {
        deltaTime = Time.deltaTime;

        DebugMode();

        queuedEvents = eventQueue.Count;

        if (queuedEvents > 0)
        {
            instance.owPlayer.GetComponent<OWPlayerInput>().acceptInputs = false;
            instance.owPlayer.eventInteraction = false;
            if (eventQueue.Peek().Execute())
                eventQueue.Dequeue();

        }
        else
        {
            if (!GameObject.Find("DialogBox").GetComponent<DialogManager>().dialogOn)
            {
                instance.owPlayer.GetComponent<OWPlayerInput>().acceptInputs = true;
                instance.owPlayer.eventInteraction = true;
            }
            else
            {
                instance.owPlayer.GetComponent<OWPlayerInput>().acceptInputs = false;
                instance.owPlayer.eventInteraction = false;
            }
        }

        if (InventoryManager.instance)
        {
            keyRing = InventoryManager.instance.GetQuantity("Key");
        }
    }

    private void DebugMode()
    {
        var keyboard = Keyboard.current;
        //var gamepad = Gamepad.current;

        /// Rework these to support queueing individual events
        //if (keyboard.iKey.wasPressedThisFrame)
        //{
        //    FloorEvent newEvent = new FloorEvent();
        //    newEvent.AddEventToQueue();
        //}
        //if (keyboard.uKey.wasPressedThisFrame)
        //{
        //    eventQueue.Enqueue(new DelayIEvent(0.5f));
        //}
        //if (keyboard.oKey.wasPressedThisFrame)
        //{
        //    FloorEvent debugBattleEvent = new FloorEvent();
        //    debugBattleEvent.typeOfEvent = EventType.Battle;
        //    eventQueue.Enqueue(new FloorIEvent(debugBattleEvent));
        //}
        //if (keyboard.tKey.wasPressedThisFrame)
        //{
        //    //InventoryManager.instance.ModifyInventory("Key", 5);
        //    FloorEvent newEvent = new FloorEvent();
        //    newEvent.typeOfEvent = EventType.Item;
        //    newEvent.itemName = "Key";
        //    newEvent.itemQuantity = 5;
        //    newEvent.SetEventInteractable(true,true,true,true,true);
        //    newEvent.AddEventToQueue();
        //    //Debug.Log(InventoryManager.instance.GetQuantity("Key"));
        //}
    }

    public void NewIEvent(FloorEvent newEvent)
    {
        eventQueue.Enqueue(new FloorIEvent(newEvent));
    }
}

public partial class GameManager : MonoBehaviour
{
    private Queue<IEvent> eventQueue = new Queue<IEvent>();

    [HideInInspector] public bool advanceEvent = false;

    //commands to queue
    public interface IEvent
    {
        //return true when finish
        bool Execute();
    }

    private class FloorIEvent : IEvent
    {
        protected FloorEvent floorEvent;

        bool eventTriggered = false;

        public FloorIEvent(FloorEvent floorEvent)
        {
            instance.advanceEvent = false;
            this.floorEvent = floorEvent;
        }

        public virtual bool Execute()
        {
            if (!eventTriggered)
            {
                instance.advanceEvent = false;
                floorEvent.TriggerEvent();
                eventTriggered = true;
            }

            if (floorEvent.typeOfEvent == EventType.Battle)
            {
                instance.battleManager.currentBattle = floorEvent.battleParams.battleData; // change this to use the BattleData instead of the event
            }

            if (!instance.advanceEvent)
            {
                return false;
            }
            return true;
        }
    }

    private class DelayIEvent : IEvent
    {
        private float _delayTime;
        private float _timer = 0.0f;

        //private bool lockPlayerInput = true;

        private DelayIEvent() { }

        public DelayIEvent(float delayTime/*, bool lockInput*/)
        {
            _delayTime = delayTime;
            //lockPlayerInput = lockInput;
        }

        public bool Execute()
        {
            //if (lockPlayerInput)
            //{
            //    instance.owPlayer.GetComponent<OWPlayerInput>().acceptInputs = false;
            //}

            _timer += Time.deltaTime;

            if (_timer < _delayTime)
                return false;
            else
            {
                instance.advanceEvent = true;
            }

            return true;
        }
    }
}
