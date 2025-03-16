using UnityEngine;

public class LightFakeGI : MonoBehaviour
{
    [SerializeField] Transform originTransform;
    [SerializeField] Transform pointLight;

    [SerializeField] Transform camera;

    [SerializeField, Min(0)] float minDistance = 1.5f;
    [SerializeField] Vector3[] directions = new Vector3[] { Vector3.up, Vector3.up, };

    RaycastHit[] raycastHits;
    int collisionsCount;

    void Start()
    {
        camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        pointLight.position = GetCollisionsCenterPosition();
    }

    private bool RaycastDirection(Vector3 direction, out RaycastHit hitInfo)
    {
        Ray ray = new Ray(camera.position, direction);

        return Physics.Raycast(ray, out hitInfo, 100);
    }

    private Vector3 GetCollisionsCenterPosition()
    {
        //var forward = RaycastDirection(camera.forward, out RaycastHit hitInfo);
        //var up = RaycastDirection(camera.up, out RaycastHit hitInfo2);
        //
        //RaycastDirection(Vector3.down);

        bool forward = RaycastDirection(camera.forward, out RaycastHit hitInfo);

        if (forward)
        {
            Vector3 diff = hitInfo.point - camera.position;
            float distanceSqr = diff.sqrMagnitude;
            return Vector3.Lerp(hitInfo.point, camera.position, 0.5f);
        }

        return camera.forward * 100;
    }
}
