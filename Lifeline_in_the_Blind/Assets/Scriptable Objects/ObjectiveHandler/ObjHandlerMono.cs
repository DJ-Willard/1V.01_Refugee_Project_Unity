using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// all this code could really go in third person controller. harder to access from here

public class ObjHandlerMono : MonoBehaviour
{
    public ObjectiveHandler objectiveHandler;
    public TMP_Text objPromptTMP;

    private void Start()
    {
        objectiveHandler.Init();
        DisplayCurrObjective();
    }

    public void DisplayCurrObjective()
    {
        objPromptTMP.text = objectiveHandler.GetCurrObjText();
    }

    private void OnApplicationQuit()
    {
        objectiveHandler.CurrentMainObj = null;
    }
}
