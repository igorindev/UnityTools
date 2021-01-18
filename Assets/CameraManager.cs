using System.Collections;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [Header("Camera")]
    public GameObject cameraMain;
    public Camera cameraPlayer;
    public Camera cameraAmbient;
   // public FPSMouseLook fpsMouseLook;
   // public FreeCameraMovement freeCamera;
   // public PlayerInteract playerInteract;
   // public CameraMovement cameraMovement;
    public float lerpMoveSpeed = 0.4f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem fowardDash;
    [SerializeField] private ParticleSystem backwardDash;
    [SerializeField] private ParticleSystem leftDash;
    [SerializeField] private ParticleSystem rightDash;
    [SerializeField] private ParticleSystem hook;
    public ParticleSystem RightDash { get => rightDash; set => rightDash = value; }
    public ParticleSystem LeftDash { get => leftDash; set => leftDash = value; }
    public ParticleSystem BackwardDash { get => backwardDash; set => backwardDash = value; }
    public ParticleSystem FowardDash { get => fowardDash; set => fowardDash = value; }
    public ParticleSystem Hook { get => hook; set => hook = value; }

    [Header("Shake")]
    public float shakeDuration = 0f;
    public float shakeAmount = 0.7f;  // Amplitude of the shake. A larger value shakes the camera harder.
    public float decreaseFactor = 1.0f;
    public float defaultFov;

    bool mouseCond;
    public bool startCursorActive;

    [Header("Depth of Field")]
    Ray raycast;
    float hitDistance;
    public float maxFocusDistance = 5f;
    //[Range(1, 25)] float focusSpeed = 10f;
    RaycastHit hit;

   // public PostProcessVolume volume;
   // public PostProcessLayer layer;
   // DepthOfField depthOfField;
   // public LayerMask ignoreLayer;

    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Já há um CameraManager declarado");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        defaultFov = cameraPlayer.fieldOfView;
        CursorActive(startCursorActive);
        ang0 = transform.localEulerAngles;

        //VideoManager.instance.SetPostProcessing(volume, layer);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            UseFreeCamera();
            mouseCond = !mouseCond;
            CursorActive(mouseCond);
        }

        PerformRecoil();
    }

    Coroutine inExecutionMoveTo;

    public void MoveTo(Transform desiredPos, float moveSpeed = 0.8f, bool enablePlayerCamera = false)
    {
        if (inExecutionMoveTo != null)
        {
            StopCoroutine(inExecutionMoveTo);
        }
        inExecutionMoveTo = StartCoroutine(MoveToPerform(desiredPos, moveSpeed, enablePlayerCamera));
    }
    public void MoveTo(Vector3 desiredPos, Quaternion desiredRot, float moveSpeed = 0.8f, bool enablePlayerCamera = false)
    {
        if (inExecutionMoveTo != null)
        {
            StopCoroutine(inExecutionMoveTo);
        }
        inExecutionMoveTo = StartCoroutine(MoveToPerform(desiredPos, desiredRot, moveSpeed, enablePlayerCamera));
    }
    public void MoveToPlayer(bool enablePlayer)
    {
        //MoveTo(PlayerManager.instance.Head.position, fpsMouseLook.lastRotation, lerpMoveSpeed, enablePlayer);
    }
    IEnumerator MoveToPerform(Transform to, float moveSpeed, bool enablePlayerCamera)
    {
        if (enablePlayerCamera)
        {
            cameraPlayer.enabled = true;
        }
        float counter = 0;
        while (counter < moveSpeed - 0.1f)
        {
            counter += Time.deltaTime;
            transform.parent.position = Vector3.Slerp(transform.parent.position, to.position, counter / moveSpeed);
            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, to.rotation, counter / moveSpeed);

            yield return null;
        }
        transform.parent.position = to.position;
        transform.parent.rotation = to.rotation;

        if (enablePlayerCamera)
        {
            //fpsMouseLook.enabled = true;
        }
    }
    IEnumerator MoveToPerform(Vector3 to, Quaternion how, float moveSpeed, bool enablePlayerCamera)
    {
        if (enablePlayerCamera)
        {
            cameraPlayer.enabled = true;
        }
        float counter = 0;
        while (counter < moveSpeed - 0.1f)
        {
            counter += Time.deltaTime;
            transform.parent.position = Vector3.Slerp(transform.parent.position, to, counter / moveSpeed);
            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, how, counter / moveSpeed);

            yield return null;
        }
        transform.parent.position = to;
        transform.parent.rotation = how;

        if (enablePlayerCamera)
        {
            //fpsMouseLook.enabled = true;
        }
    }

    public void EnablePlayerCamera()
    {
        cameraPlayer.enabled = true;
        //fpsMouseLook.enabled = true;
    }
    public void DisablePlayerCamera()
    {
        cameraPlayer.enabled = false;
        //fpsMouseLook.enabled = false;
    }

    public void CursorActive(bool useCursor)
    {
       // if (CheckConnectedControll.instance.controlConnected)
       // {
       //     Cursor.lockState = CursorLockMode.Locked;
       //     Cursor.visible = false;
       //     return;
       // }
       //
       // if (useCursor)
       // {
       //     Cursor.lockState = CursorLockMode.None;
       // }
       // else
       // {
       //     Cursor.lockState = CursorLockMode.Locked;
       // }
       // Cursor.visible = useCursor;
    }

    public void DetachCameraFromParent()
    {
        cameraMain.transform.parent = null;
    }
    public void AtachCameraTo(Transform parent)
    {
        cameraMain.transform.SetParent(parent);
    }

    public void SetCameraPositionAndRotation(Transform pos, bool cursorActive = false)
    {
        cameraMain.transform.position = pos.position;
        cameraMain.transform.rotation = pos.rotation;
        CursorActive(cursorActive);
    }

    public void UseFreeCamera()
    {
       // PlayerManager.instance.PlayerMove.enabled = !PlayerManager.instance.PlayerMove.enabled;
       //fpsMouseLook.enabled = !fpsMouseLook.enabled;
       //freeCamera.enabled = !freeCamera.enabled;
       //if (fpsMouseLook.enabled && freeCamera.enabled)
       //{
       //    fpsMouseLook.enabled = false;
       //}
    }
    public void UseFreeCamera(bool condition)
    {
       // PlayerManager.instance.PlayerMove.enabled = !condition;
       //fpsMouseLook.enabled = !condition;
       //freeCamera.enabled = condition;
       //if (fpsMouseLook.enabled && freeCamera.enabled)
       //{
       //    fpsMouseLook.enabled = false;
       //}
    }

    #region Field of View
    public void ChangeFov(float multiplier)
    {
        StopCoroutine("RestoreFovPermorm");
        StartCoroutine("ChangeFovPerform", multiplier);
    }
    IEnumerator ChangeFovPerform(float multiplier)
    {
        while (cameraPlayer.fieldOfView < cameraPlayer.fieldOfView * multiplier)
        {
            cameraPlayer.fieldOfView = Mathf.Lerp(cameraPlayer.fieldOfView, cameraPlayer.fieldOfView * multiplier, 1 * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        cameraPlayer.fieldOfView *= multiplier;
    }
    public void RestoreFov()
    {
        StopCoroutine("ChangeFovPerform");
        StartCoroutine("RestoreFovPermorm");
    }
    IEnumerator RestoreFovPermorm()
    {
        while (cameraPlayer.fieldOfView > defaultFov)
        {
            cameraPlayer.fieldOfView = Mathf.Lerp(cameraPlayer.fieldOfView, defaultFov, 2 * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        cameraPlayer.fieldOfView = defaultFov;
    }
    #endregion

    #region Recoil
    //--------------------------------------------------------
    //RECOIL
    //

    float upSpeed; // controls smoothing speed 
    float dnSpeed; // how fast the weapon returns to original position

    Vector3 ang0; // initial angle private var 
    Vector3 ang = Vector3.zero; // smoothed angle
    Vector3 target;

    public void Recoil(float force = 2.5f, float weaponUpSpeed = 9, float weaponDnSpeed = 20)
    {
        dnSpeed = weaponDnSpeed;
        upSpeed = weaponUpSpeed;
        target.x += force; // add recoil force
        target.y += force / 2; // add recoil force
        target.z = 0; // add recoil force
    }
    void PerformRecoil()
    {
        ang = Vector3.Slerp(ang, target, upSpeed * Time.deltaTime);
        transform.localEulerAngles = ang0 - ang; // move the camera or weapon 
        target = Vector3.Slerp(target, Vector3.zero, dnSpeed * Time.deltaTime);
    }
    #endregion

    #region Shake
    Coroutine shake;
    public bool Shaking { get; set; }

    [ContextMenu("Shake Camera")]
    public void CameraShake()
    {
        CameraShake(shakeDuration, shakeAmount, shakeAmount);
    }
    public void CameraShake(float duration, float magnitudeX, float magnitudeY)
    {
        if (transform.parent != null)
        {
            if (shake != null)
            {
                StopCoroutine(shake);
            }
            Shaking = true;
            shake = StartCoroutine(Shake(duration, magnitudeX, magnitudeY));
        }
    }

    public void ForceStopShake()
    {
        StopCoroutine(shake);
        transform.localPosition = Vector3.zero;
    }
    IEnumerator Shake(float duration, float magnitudeX, float magnitudeY)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitudeX;
            float y = Random.Range(-1f, 1f) * magnitudeY;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = Vector3.zero;
    }
    #endregion

    #region DepthOfField
    public bool IsHit { get; set; }
    void GetDoF()
    {
        //volume.profile.TryGetSettings(out depthOfField);
    }
    void DepthOfFieldAutoAjust()
    {
       // raycast = new Ray(transform.position, transform.forward * maxFocusDistance);
       //
       // IsHit = false;
       //
       // if (Physics.Raycast(raycast, out hit, maxFocusDistance, ignoreLayer))
       // {
       //     IsHit = true;
       //     hitDistance = Vector3.Distance(transform.position, hit.point);
       // }
       // else
       // {
       //     if (hitDistance < maxFocusDistance)
       //     {
       //         hitDistance++;
       //     }
       // }
       // hitDistance = Mathf.Clamp(hitDistance, 1, maxFocusDistance);
       // SetFocus();
    }
    void SetFocus()
    {
       // depthOfField.focusDistance.value = Mathf.Lerp(depthOfField.focusDistance.value, hitDistance, focusSpeed * Time.deltaTime);
       //
       // if (hitDistance <= 2f)
       // {
       //     depthOfField.aperture.value = 10f;
       // }
       // else
       // {
       //     depthOfField.aperture.value = 3.4f;
       // }
    }
    #endregion
}
