using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CheckTerrainTexture terrainTexture;
    private float startStepTimer = 0;
    private float controlStepTime = 0.6f;

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startStepTimer > controlStepTime)
        {
            startStepTimer = Time.time;
            terrainTexture.PlayFootstep();

            // if (inWater)
            // {
            //     source.PlayOneShot(waterStep[Random.Range(0, waterStep.Length)], 1);
            // }
            // else
            // {
            //     if (collidingWithTerrain)
            //     {
            //         checkTerrain.PlayFootstep();
            //     }
            //     else
            //     {
            //         source.PlayOneShot(defaultStep[Random.Range(0, defaultStep.Length)], 1);
            //     }
            // }


        }
    }
}
