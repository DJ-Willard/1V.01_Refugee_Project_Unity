using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New ObjectiveHandler", menuName = "Inventory System/ObjectiveHandler")]
public class ObjectiveHandler : ScriptableObject
{
    // MainObjectives is dependent list of objectives, must go in order
    // Side is independent (could do list of queues for more complex dependencies,
    // such as quests.
    // Implementing MainObjectives first
    public ObjectiveItem CurrentMainObj;

    public List<ObjectiveItem> MainObjList = new List<ObjectiveItem>();
    public List<ObjectiveItem> SideObjList = new List<ObjectiveItem>(); // placeholder
    private Queue<ObjectiveItem> MainObjQueue = new Queue<ObjectiveItem>();

    // called by ThirdPersonController in Start()
    public void Init()
    {
        // convert public list to private queue
        for (int i = 0; i < MainObjList.Count; i++)
        {
            // does this work or need new?
            MainObjQueue.Enqueue(MainObjList[i]);
        }
        // get first objective from queue
        GetNextMainObjective();
    }

    public void GetNextMainObjective()
    {
        // Set initial objective from queue
        if (MainObjQueue.TryDequeue(out CurrentMainObj)){
            //objectiveTextPanel.GetComponent<TextMeshProUGUI>().text = CurrentMainObj.ToString();
            Debug.Log("MainObj successfully set.");
        } else {
            Debug.Log("No remaining MainObj in queue.");
        }
    }

    // Review: ref keyword allows passing of var
    public void DisplayCurrObjectiveByRef(ref TMP_Text newtext)
    {
        Debug.Log("ObjectiveHandler.cs: DCPBR() called.");
        newtext.text = CurrentMainObj.objectiveText;
    }

    public string GetCurrObjText()
    {
        Debug.Log("ObjectiveHandler.cs: GetCurrObjText() called.");
        Debug.Log(CurrentMainObj.objectiveText);
        return CurrentMainObj.objectiveText;
    }

    public void Quit()
    {
        // May or may not want to do this depending on how level transfers work.
        // Doesn't hurt to have a placeholder quit function. 
        CurrentMainObj = null;
    }
}