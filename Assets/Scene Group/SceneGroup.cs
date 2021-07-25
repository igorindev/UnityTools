using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneGroup", menuName = "ScriptableObjects/Scene Group")]
public class SceneGroup : ScriptableObject
{
    [Scene] public string[] scenes;
}
