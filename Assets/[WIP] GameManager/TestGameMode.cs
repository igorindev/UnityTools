using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameMode", menuName ="Managers/GameMode")]
public class TestGameMode : GameMode
{
    protected override void Initialize()
    {
        //Debug.Log("teste");
    }

    protected override void OnUpdate()
    {
        throw new System.NotImplementedException();
    }
}
