using UnityEngine;

public class ScenePartition : MonoBehaviour
{
    [SerializeField] bool active;
    [SerializeField] float checkDelay = 0.15f;
    [SerializeField] ScenePart[] loadZones;

    Transform player;

    float lastCheck = 0;

    [ContextMenu("Get All Scene Parts")]
    void GetAllSceneParts()
    {
        loadZones = GetComponentsInChildren<ScenePart>();
    }

    void FixedUpdate()
    {
        if (active)
        {
            if (ReferenceEquals(player, null))
            {
                if (GameManager.instance.Player)
                {
                    player = GameManager.instance.Player.transform;
                }
            }

            if (Time.time >= lastCheck + checkDelay)
            {
                for (int i = 0; i < loadZones.Length; i++)
                {
                    loadZones[i].UpdateCheck(ReferenceEquals(player, null) ? Vector3.zero : player.position);
                }
                lastCheck = Time.time;
            }
        }
    }
}