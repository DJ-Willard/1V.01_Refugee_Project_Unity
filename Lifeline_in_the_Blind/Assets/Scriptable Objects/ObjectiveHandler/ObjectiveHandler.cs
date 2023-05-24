using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ObjectiveHandler", menuName = "Inventory System/ObjectiveHandler")]
public class ObjectiveHandler : ScriptableObject
{
    // MainObjectives is dependent list of objectives, must go in order
    // Side is independent (could do list of queues for more complex dependencies,
    // such as quests.
    // Implementing MainObjectives first
    public ObjectiveSlot CurrentMainObj;

    public List<ObjectiveSlot> MainObjList = new List<ObjectiveSlot>();
    public List<ObjectiveSlot> SideObjList = new List<ObjectiveSlot>(); // placeholder
    private Queue<ObjectiveSlot> MainObjQueue = new Queue<ObjectiveSlot>();

    public void Awake()
    {
        // convert public list to private queue
        for (int i = 0; i < MainObjList.Count; i++)
        {
            // does this work or need new?
            MainObjQueue.Enqueue(MainObjList[i]);
        }
        // is this proper syntax for use of 'out' keyword?
        if (MainObjQueue.TryDequeue(out CurrentMainObj)){
            Debug.Log("MainObj successfully set.");
        } else {
            Debug.Log("No remaining MainObj in queue.");
        }
    }
}

[System.Serializable]
public class ObjectiveSlot
{
    [TextArea(15, 20)]
    public string objectiveDescription;
}