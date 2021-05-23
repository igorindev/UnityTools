using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    [SerializeField] Ragdoll ragdoll;
    [SerializeField] LoadScene loadScene;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out Rigidbody r))
                {
                    ragdoll.RagdollSetActive(true, r, (hit.transform.position - ray.origin).normalized, 200);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            loadScene.LoadSceneName();
        }
    }
}
