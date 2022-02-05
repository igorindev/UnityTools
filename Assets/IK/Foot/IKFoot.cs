using System.Collections;
using UnityEngine;

namespace IKSolver
{
    [RequireComponent(typeof(Animator))]
    public class IKFoot : MonoBehaviour
    {
        [Header("Feet")]
        [SerializeField] bool enableFeetIk;
        [SerializeField] bool enableIkRotations;
        [SerializeField] bool movePelvis = false;
        [SerializeField] float disableSmoothSpeed = 1;
        [Space(7)]
        [SerializeField] [Range(0, 2)] float heightFromGroundRaycast = 1.14f;
        [SerializeField] [Range(0, 2)] float raycastDownDistance = 1.5f;
        [Space(7)]
        [SerializeField] float pelvisOffset = 0;
        [SerializeField] [Range(0, 1)] float pelvisUpAndDownSpeed = 0.25f;
        [SerializeField] [Range(0, 1)] float feetToIkPositionSpeed = 0.5f;
        [Space(7)]
        [SerializeField] LayerMask layer;

        [Header("Curve Names")]
        [SerializeField] string rightFootAnimVariableName = "RightFootCurve";
        [SerializeField] string leftFootAnimVariableName = "LeftFootCurve";

        [Header("Debugger")]
        [SerializeField] bool showSolverDebug = true;

        Animator anim;

        //Positions
        Vector3 rightFootPosition = Vector3.zero;
        Vector3 leftFootPosition = Vector3.zero;
        Vector3 rightFootIkPosition = Vector3.zero;
        Vector3 leftFootIkPosition = Vector3.zero;

        //Rotations
        Quaternion rightFootIkRotation = Quaternion.identity;
        Quaternion leftFootIkRotation = Quaternion.identity;

        //Last Y positions
        float lastPelvisPositionY;
        float lastRightFootPositionY;
        float lastLeftFootPositionY;

        Coroutine c;

        public float HeightFromGroundRaycast { get => active * heightFromGroundRaycast; set => heightFromGroundRaycast = value; }
        public float RaycastDownDistance { get => active * raycastDownDistance; set => raycastDownDistance = value; }
        public float PelvisUpAndDownSpeed { get => active * pelvisUpAndDownSpeed; set => pelvisUpAndDownSpeed = value; }
        public float FeetToIkPositionSpeed { get => active * feetToIkPositionSpeed; set => feetToIkPositionSpeed = value; }

        public void Disable(bool value)
        {
            if (c != null)
            {
                StopCoroutine(c);
            }

            c = StartCoroutine(SmoothDisable(value));
        }

        public float active = 1;

        void Start()
        {
            anim = GetComponent<Animator>();
        }

        /// <summary>
        /// Updating the AdjustFeetTarget method and also finding position of each foot inside the Solver Position
        /// </summary>
        void FixedUpdate()
        {
            if (!enableFeetIk) { return; }
            if (anim == null) { Debug.LogError("No Animator assigned"); return; }
            if (false)
            {
                //Disable(true);
            }
            else
            {
                Disable(false);
            }

            //movePelvis = !(playerController.PlayerState.IsOnSlope && (playerController.MoveDirection.x != 0 || playerController.MoveDirection.z != 0));

            AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
            AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

            //Find and cast to the ground to find positions
            FeetPositionSolver(rightFootPosition, ref rightFootIkPosition, ref rightFootIkRotation);
            FeetPositionSolver(leftFootPosition, ref leftFootIkPosition, ref leftFootIkRotation);
        }

        void OnAnimatorIK(int layerIndex)
        {
            if (!enableFeetIk) { return; }

            if (movePelvis)
            {
                MovePelvisHeight();
            }

            //Right Foot
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            if (enableIkRotations)
            {
                anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnimVariableName));
            }
            MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIkPosition, rightFootIkRotation, ref lastRightFootPositionY);

            //Left Foot
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            if (enableIkRotations)
            {
                anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(leftFootAnimVariableName));
            }
            MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIkPosition, leftFootIkRotation, ref lastLeftFootPositionY);
        }

        void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPositionY)
        {
            Vector3 targetIkPosition = anim.GetIKPosition(foot);

            if (positionIkHolder != Vector3.zero)
            {
                targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
                positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

                float yVariable = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, FeetToIkPositionSpeed);
                targetIkPosition.y += yVariable;

                lastFootPositionY = yVariable;
                targetIkPosition = transform.TransformPoint(targetIkPosition);

                anim.SetIKRotation(foot, rotationIkHolder);
            }

            anim.SetIKPosition(foot, targetIkPosition);
        }
        void MovePelvisHeight()
        {
            if (rightFootIkPosition == Vector3.zero || leftFootIkPosition == Vector3.zero || lastPelvisPositionY == 0)
            {
                lastPelvisPositionY = anim.bodyPosition.y;
                return;
            }

            float rightOffsetPos = rightFootIkPosition.y - transform.position.y;
            float leftOffsetPos = leftFootIkPosition.y - transform.position.y;

            float totalOffset = leftOffsetPos < rightOffsetPos ? leftOffsetPos : rightOffsetPos;

            Vector3 newPelvisPosition = anim.bodyPosition + Vector3.up * totalOffset;

            newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, PelvisUpAndDownSpeed);

            anim.bodyPosition = newPelvisPosition;

            lastPelvisPositionY = anim.bodyPosition.y;
        }
        void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIkPositions, ref Quaternion feetIkRotations)
        {
            if (showSolverDebug)
            {
                Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (RaycastDownDistance + HeightFromGroundRaycast), Color.yellow);
            }

            if (Physics.Raycast(fromSkyPosition, Vector3.down, out RaycastHit feetOutHit, RaycastDownDistance + HeightFromGroundRaycast, layer))
            {
                feetIkPositions = fromSkyPosition;
                feetIkPositions.y = feetOutHit.point.y + pelvisOffset;
                feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

                return;
            }

            //Get Nothing
            feetIkPositions = Vector3.zero;
        }
        void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
        {
            feetPositions = anim.GetBoneTransform(foot).position;
            feetPositions.y = transform.position.y + HeightFromGroundRaycast;
        }

        IEnumerator SmoothDisable(bool isDisabled)
        {
            if (isDisabled)
            {
                while (active > 0)
                {
                    active -= Time.deltaTime * disableSmoothSpeed;
                    yield return null;
                }
                active = 0;
            }
            else
            {
                while (active < 1)
                {
                    active += Time.deltaTime * disableSmoothSpeed;
                    yield return null;
                }
                active = 1;
            }
        }
    }
}