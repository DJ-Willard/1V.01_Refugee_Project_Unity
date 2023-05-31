using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New ObjectiveHandler", menuName = "Inventory System/ObjectiveHandler")]
public class ObjectiveHandler : ScriptableObject
{
    // REFACTORED FROM QUEUE TO NORMAL LIST
    // Easier to handle with by just holding onto an index in the list.
    // Allows easier reset during something like SceneManager.LoadScene(0)

    // MainObjectives is dependent list of objectives, must go in order
    // Side is independent (could do list of queues for more complex dependencies,
    // such as quests.
    // Implementing MainObjectives first
    public ObjectiveItem CurrentMainObj;

    public List<ObjectiveItem> MainObjList = new List<ObjectiveItem>();
    // alternate get implemenation for these (would populate in Init)
    // public List<string> MainObjGOnames = new List<string>();
    // public List<string> MainObjLockTags = new List<string>();

    public List<ObjectiveItem> SideObjList = new List<ObjectiveItem>(); // placeholder
    public int CurrentMainObjIndex;
    private Queue<ObjectiveItem> MainObjQueue = new Queue<ObjectiveItem>();

    // called by ThirdPersonController in Start()
    public void Init(int objIndexOverride = 0)
    {
        /* QUEUE VERSION
        // convert public list to private queue
        for (int i = 0; i < MainObjList.Count; i++)
        {
            // does this work or need new?
            MainObjQueue.Enqueue(MainObjList[i]);
        }
        // get first objective from queue
        GetNextMainObjective();
        */

        // LIST VERSION
        CurrentMainObjIndex = objIndexOverride;
        CurrentMainObj = MainObjList[CurrentMainObjIndex];
    }

    public void GetNextMainObjective()
    {
        /* QUEUE VERSION
        // Set initial objective from queue
        if (MainObjQueue.TryDequeue(out CurrentMainObj)){
            //objectiveTextPanel.GetComponent<TextMeshProUGUI>().text = CurrentMainObj.ToString();
            Debug.Log("MainObj successfully set.");
        } else {
            Debug.Log("No remaining MainObj in queue.");
        }
        */

        // LIST VERSION
        if (CurrentMainObjIndex < MainObjList.Count)
        {
            CurrentMainObjIndex++;
            CurrentMainObj = MainObjList[CurrentMainObjIndex];
            Debug.Log("MainObj index successfully set.");
        } else {
            // reset index here or no? 
            Debug.Log("No remaining MainObj in list.");
        }
    }

    // Review: ref keyword allows passing of var
    public void DisplayCurrObjectiveByRef(ref TMP_Text newtext)
    {
        Debug.Log("ObjectiveHandler.cs: DCPBR() called.");
        
        // QUEUE VERSION
        // newtext.text = CurrentMainObj.objectiveText;

        // LIST VERSION
        newtext.text = MainObjList[CurrentMainObjIndex].objectiveText;
    }

    public string GetCurrObjText()
    {
        Debug.Log("ObjectiveHandler.cs: GetCurrObjText() called.");
        
        // QUEUE VERSION
        // Debug.Log(CurrentMainObj.objectiveText);
        // return CurrentMainObj.objectiveText;

        // LIST VERSION
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