using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CheckpointItem", menuName = "Inventory System/CheckpointItem")]
public class CheckpointItem : ScriptableObject
{
    // might want to handle this elsewhere or with events
    // public GameObject prefabTrigger;
    [Tooltip("GameObject name for checkpoint empty trigger.")]
    public string trigger_name;
    [Tooltip("Empty whose transform player will be loaded to.")]
    public string transform_name;

    // could do onTrigger check in TPC for whether pickup is objective object
    // could also do this with tags, or do precheck with tags,
    // then if true look into item itself for appropriate text, prompt, sound, etc.

    public void OnEnable()
    {
        if (trigger_name == "") trigger_name = "not set";
        if (transform_name == "") transform_name = "not set";
    }
}
