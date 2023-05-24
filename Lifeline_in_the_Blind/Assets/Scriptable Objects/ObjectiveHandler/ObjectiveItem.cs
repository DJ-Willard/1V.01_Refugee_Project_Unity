using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ObjectiveItem", menuName = "Inventory System/ObjectiveItem")]
public class ObjectiveItem : ScriptableObject
{
    // might want to handle this elsewhere or with events
    public GameObject prefabTrigger;
    [TextArea(3, 20)]
    public string objectiveText;

    // could do onTrigger check in TPC for whether pickup is objective object
    // could also do this with tags, or do precheck with tags,
    // then if true look into item itself for appropriate text, prompt, sound, etc.
}
