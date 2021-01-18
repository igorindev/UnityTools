using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    public Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void OnAnimatorIK(int layerIndex)
    {
        if (anim)
        {
            if (CameraManager.instance.cameraMain != null)
            {
                anim.SetLookAtWeight(1);
                anim.SetLookAtPosition(CameraManager.instance.cameraMain.transform.position);
            }
        }
    }
}
