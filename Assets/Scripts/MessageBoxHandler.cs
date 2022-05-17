using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageBoxHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageBox;

    [SerializeField] Image backdrop;

    [SerializeField] float activeTime;
    float activeTimer;

    private void Update()
    {
        activeTimer -= Time.deltaTime;
        backdrop.enabled = (activeTimer > 0);
        messageBox.enabled = backdrop.enabled;
    }

    public void PlayMessage(string message)
    {
        messageBox.text = message;
        activeTimer = activeTime;
    }
}
