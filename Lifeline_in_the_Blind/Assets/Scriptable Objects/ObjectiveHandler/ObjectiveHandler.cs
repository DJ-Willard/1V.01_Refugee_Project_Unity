using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New ObjectiveHandler", menuName = "Inventory System/ObjectiveHandler")]
public class ObjectiveHandler : ScriptableObject
{
    public ObjectiveItem CurrentMainObj;
    public CheckpointItem CurrentCheckpoint;
    public CheckpointItem NextCheckpoint;
    public List<ObjectiveItem> MainObjList = new List<ObjectiveItem>();
    public List<ObjectiveItem> SideObjList = new List<ObjectiveItem>(); // placeholder

    public List<CheckpointItem> CheckpointList = new List<CheckpointItem>();
    public int CurrentMainObjIndex;
    public int CurrentCheckpointIndex;

    // called by ThirdPersonController in Start()
    public void Init(int objIndexOverride = 0, int checkpointIndexOverride = 0)
    {
        CurrentMainObjIndex = objIndexOverride;
        CurrentCheckpointIndex = checkpointIndexOverride;
        CurrentMainObj = MainObjList[CurrentMainObjIndex];
        CurrentCheckpoint = CheckpointList[CurrentCheckpointIndex];
        SetNextCheckpoint();
    }

    public void IncrementMainObjective()
    {
        if (CurrentMainObjIndex < MainObjList.Count)
        {
            CurrentMainObjIndex++;
            CurrentMainObj = MainObjList[CurrentMainObjIndex];
            Debug.Log("MainObj index successfully set to " + CurrentMainObjIndex);
        } else {
            // reset index here or no? 
            Debug.Log("No remaining MainObj in list.");
        }
    }

    public void IncrementCheckpoint()
    {
        if (CurrentCheckpointIndex < CheckpointList.Count)
        {
            CurrentCheckpointIndex++;
            CurrentCheckpoint = CheckpointList[CurrentCheckpointIndex];
            SetNextCheckpoint();
            Debug.Log("Checkpoint index successfully set to " + CurrentCheckpointIndex);
        } else {
            // reset index here or no? 
            Debug.Log("No remaining checkpoints in list.");
        }
    }

    private void SetNextCheckpoint()
    {
        int nextCheckpointIndex = CurrentCheckpointIndex + 1;
        if (nextCheckpointIndex < CheckpointList.Count)
        {
            NextCheckpoint = CheckpointList[nextCheckpointIndex];
        }
        else Debug.Log("Log: Next checkpoint index out of range.");
    }

    // Review: ref keyword allows passing of var
    public void DisplayCurrObjectiveByRef(ref TMP_Text newtext)
    {
        Debug.Log("ObjectiveHandler.cs: DCPBR() called.");
        newtext.text = MainObjList[CurrentMainObjIndex].objectiveText;
    }

    public string GetCurrObjText()
    {
        Debug.Log("ObjectiveHandler.cs: GetCurrObjText() called.");
        return MainObjList[CurrentMainObjIndex].objectiveText;
    }

    public void Quit()
    {
        // May or may not want to do this depending on how level transfers work.
        // Doesn't hurt to have a placeholder quit function. 
        CurrentMainObj = null;
        CurrentMainObjIndex = 0;
    }
}