using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    [Min(1), SerializeField] int frames;

    List<AIBrain> aIBrains = new List<AIBrain>();

    int tryUpdatePerFrame;

    int aisToUpdate = 0;

    int oldEntityCount = int.MinValue;

    void Update()
    {
        if (aisToUpdate >= aIBrains.Count)
            aisToUpdate = 0;

        if (oldEntityCount != aIBrains.Count)
        {
            tryUpdatePerFrame = aIBrains.Count < frames ? 1 : (int)Mathf.Round(aIBrains.Count / frames + 0.5f);
            oldEntityCount = aIBrains.Count;
        }

        string whoWasUpdated = "";
        int i = 0;
        for (; i < tryUpdatePerFrame; i++)
        {
            if (aisToUpdate >= aIBrains.Count) break;

            aIBrains[aisToUpdate].UpdateBrain();
            whoWasUpdated += aIBrains[aisToUpdate].name + " | ";

            aisToUpdate++;
        }

        Debug.Log("Up : " + i + " | " + whoWasUpdated);
    }

    public void Add(AIBrain brain)
    {
        aIBrains.Add(brain);
    }

    public void Remove(AIBrain brain)
    {
        aIBrains.Remove(brain);
    }
}