using UnityEditor;
using UnityEngine;

public class IKControll : MonoBehaviour
{
    //Chain lenght of bones
    public int chainLenght = 2;

    //Target the chain should bent to
    public Transform target;
    public Transform Pole;

    //Solver iterations per update
    [Header("Solver Parameters")]
    public int iterations = 10;

    //Distance when the solver stops
    public float Delta = 0.001f;

    //Strenght of going back to the start position
    [Range(0, 1)] public float SnapBackStrength = 1f;

    public Transform[] Bones { get; private set; }
    public Vector3[] Positions { get; private set; }
    public float[] BonesLength { get; private set; }
    public float CompleteLength { get; private set; }

    private Vector3[] StartDirectionSucc;
    private Quaternion[] StartRotationBone;
    private Quaternion StartRotationTarget;
    //private Quaternion StartRotationRoot;

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        //initial array
        Bones = new Transform[chainLenght + 1];
        Positions = new Vector3[chainLenght + 1];
        BonesLength = new float[chainLenght];

        StartDirectionSucc = new Vector3[chainLenght + 1];
        StartRotationBone = new Quaternion[chainLenght + 1];

        //init fields
        if (target == null)
        {
            target = new GameObject(gameObject.name + "Target").transform;
            target.position = transform.position;
        }
        StartRotationTarget = target.rotation;

        CompleteLength = 0;

        //init data
        Transform current = transform;
        for (int i = Bones.Length - 1; i >= 0; i--)
        {
            Bones[i] = current;
            StartRotationBone[i] = current.rotation;

            if (i == Bones.Length - 1)
            {
                //leaf
                StartDirectionSucc[i] = target.position - current.position;
            }
            else
            {
                //mid bone
                StartDirectionSucc[i] = Bones[i + 1].position - current.position;
                BonesLength[i] = StartDirectionSucc[i].magnitude;
                CompleteLength += BonesLength[i];
            }

            current = current.parent;
        }
    }

    void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (target == null)
        {
            return;
        }

        if (BonesLength.Length != chainLenght)
        {
            Init();
        }

        //get position
        for (int i = 0; i < Bones.Length; i++)
        {
            Positions[i] = Bones[i].position;
        }

       //Quaternion RootRot = (Bones[0].parent != null) ? Bones[0].parent.rotation : Quaternion.identity;
       //Quaternion RootRotDiff = RootRot * Quaternion.Inverse(StartRotationRoot);

        //1st is possible to reach?
        if ((target.position - Bones[0].position).sqrMagnitude >= CompleteLength * CompleteLength)
        {
            //stretch it
            Vector3 direction = (target.position - Positions[0]).normalized;

            //set everything after root
            for (int i = 1; i < Positions.Length; i++)
            {
                Positions[i] = Positions[i - 1] + direction * BonesLength[i - 1];
            }
        }
        else
        {
            for (int i = 0; i < Positions.Length - 1; i++)
            {
                Positions[i + 1] = Vector3.Lerp(Positions[i + 1], Positions[i] + StartDirectionSucc[i], SnapBackStrength); //Positions[i] + RootRotDiff * StartDirectionSucc[i]
            }

            for (int it = 0; it < iterations; it++)
            {
                //back
                for (int i = Positions.Length - 1; i > 0; i--)
                {
                    if (i == Positions.Length - 1)
                    {
                        Positions[i] = target.position; //set it to target
                    }
                    else
                    {
                        Positions[i] = Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * BonesLength[i];
                    }
                }

                //foward
                for (int i = 1; i < Positions.Length; i++)
                {
                    Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * BonesLength[i - 1];
                }

                //CLose enough?
                if ((Positions[Positions.Length - 1] - target.position).sqrMagnitude < Delta * Delta)
                {
                    break;
                }
            }
        }

        //move towards pole
        if (Pole != null)
        {
            for (int i = 1; i < Positions.Length - 1; i++)
            {
                Plane plane = new Plane(Positions[i + 1] - Positions[i - 1], Positions[i - 1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(Pole.position);
                Vector3 projectedBone = plane.ClosestPointOnPlane(Positions[i]);
                float angle = Vector3.SignedAngle(projectedBone - Positions[i - 1], projectedPole - Positions[i - 1], plane.normal);
                Positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (Positions[i] - Positions[i - 1]) + Positions[i - 1];
            }
        }

        //set position & rotation
        for (int i = 0; i < Positions.Length; i++)
        {
            if (i == Positions.Length - 1)
            {
                Bones[i].rotation = target.rotation * Quaternion.Inverse(StartRotationTarget) * StartRotationBone[i];
            }
            else
            {
                Bones[i].rotation = Quaternion.FromToRotation(StartDirectionSucc[i], Positions[i + 1] - Positions[i]) * StartRotationBone[i];
            }
            Bones[i].position = Positions[i];
        }
    }
    
    #if (UNITY_EDITOR) 

    private void OnDrawGizmos()
    {
        Transform current = transform;

        for (int i = 0; i < chainLenght && current != null && current.parent != null; i++)
        {
            float scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }
    #endif
}
