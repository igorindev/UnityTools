using SpringSystem;
using System.Collections.Generic;
using UnityEngine;

public class SpringBoneAssistant : MonoBehaviour
{
    public SpringManager mSpringManager;

    public Vector3 mTargetBoneAxis = new Vector3(0f, 1f, 0f);

    public void MarkChildren()
    {
        //List<SpringBoneMarker> boneMarkers = new List<SpringBoneMarker>();

        // get spring bones set
        SpringBoneMarker[] boneMarkers = FindObjectsOfType<SpringBoneMarker>();

        // mark children
        for (int i = 0; i < boneMarkers.Length; i++)
        {
            boneMarkers[i].MarkChildren();
        }

        // get again, will include children
        boneMarkers = FindObjectsOfType<SpringBoneMarker>();
        List<SpringBone> springBones = new List<SpringBone> { };
        for (int i = 0; i < boneMarkers.Length; i++)
        {
            // add spring bone
            springBones.Add(boneMarkers[i].AddSpringBone());

            // set vector
            springBones[i].boneAxis = mTargetBoneAxis;

            // unmark object
            boneMarkers[i].UnmarkSelf();
        }

        mSpringManager.springBones = springBones.ToArray();
    }

    public void CleanUp()
    {
        SpringBoneMarker[] boneMarkers = FindObjectsOfType<SpringBoneMarker>();
        SpringBone[] springBones = FindObjectsOfType<SpringBone>();

        for (int i = 0; i < boneMarkers.Length; i++)
        {
            DestroyImmediate(boneMarkers[i]);

        }
        for (int i = 0; i < springBones.Length; i++)
        {
            DestroyImmediate(springBones[i]);
        }
    }
}
