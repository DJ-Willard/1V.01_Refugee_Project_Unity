// based on last section of this article:
// https://gamedevbeginner.com/how-to-get-a-variable-from-another-script-in-unity-the-right-way/
// search on page: "How to create a global"

using UnityEngine;

[CreateAssetMenu(menuName = "Global Bool Variable")]
public class GlobalBoolVariable : ScriptableObject
{
    public bool value;
}
