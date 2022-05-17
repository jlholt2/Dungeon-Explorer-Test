using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public MessageBoxHandler messageBoxHandler;

    public TextMeshProUGUI floorNumText;
    public TextMeshProUGUI keyCountText;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        UpdateKeyCountText();
    }

    public void UpdateFloorNumText()
    {
        floorNumText.text = "Floor " + GameManager.instance.FloorNum.ToString("00");
    }

    public void UpdateKeyCountText()
    {
        if (GameManager.instance)
        {
            if (int.Parse(keyCountText.text) != GameManager.instance.keyRing)
            {
                keyCountText.text = GameManager.instance.keyRing.ToString();
            }
        }
    }
}
