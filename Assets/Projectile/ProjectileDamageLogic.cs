using UnityEngine;

public class ProjectileDamageLogic : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] GameObject vfx;
    [Header("Configurations")]
    [SerializeField] float explosionTimer = 0;
    [SerializeField] float explosionRadius = 0;

    [SerializeField] LayerMask layer;

    [SerializeField] bool onlyDisable;

    public bool OnlyDisable { get => OnlyDisable; set => OnlyDisable = value; }

    private void Start()
    {
        if (explosionTimer > 0)
        {
            Invoke("Explode", explosionTimer);
        }
        else
        {
            Invoke("Explode", 20);
        }
    }

    protected virtual void Explode()
    {
        //Spawn Vfx
        if (vfx != null)
        {
            Instantiate(vfx, transform.position, Quaternion.identity);
        }

        //Apply damage on range
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, explosionRadius, layer);
        for (int i = 0; i < hitCollider.Length; i++)
        {
            //Apply damage
        }

        if (OnlyDisable)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
