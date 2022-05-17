using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogSet
{
    public Dialog[] dialogs;

    //public void TriggerDialog()
    //{
    //    try
    //    {
    //        FindObjectOfType<DialogManager>().BeginDialogCoroutine(dialogs);
    //    }
    //    catch (System.Exception)
    //    {
    //        Debug.LogError("ERROR: DialogManager not found or dialogs is null!");
    //        throw;
    //    }
    //}
}
