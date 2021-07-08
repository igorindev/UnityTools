using System.Collections;
using UnityEngine;

public class SimulateProjectileProjection : MonoBehaviour
{
    private readonly float firingAngle = 35;
    private readonly float speed = 10;
    [SerializeField] Transform projectile = null;
    [SerializeField] Transform firePoint = null;
    [SerializeField] Transform target = null;

    Coroutine simulation;

    public void Simulate()
    {
        if (simulation != null)
        {
            StopCoroutine(simulation);
        }

        simulation = StartCoroutine(SimulateProjectile(projectile, firePoint, target.position));
    }
    public void Simulate(Transform projectile, Transform target)
    {
        if (simulation != null)
        {
            StopCoroutine(simulation);
        }

        simulation = StartCoroutine(SimulateProjectile(projectile, firePoint, target.position));
    }
    IEnumerator SimulateProjectile(Transform projectile, Transform origin, Vector3 target)
    {
        yield return new WaitForSeconds(0.5f);

        projectile.gameObject.SetActive(true);

        // Move projectile to the position of throwing object + add some offset if needed.
        projectile.position = origin.position + new Vector3(0, 0.0f, 0);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(projectile.position, target);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / speed);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;
        // Rotate projectile to face the target.
        projectile.rotation = Quaternion.LookRotation(target - projectile.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            projectile.Translate(0, (Vy - (speed * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }

        //return ball
        projectile.position = origin.position;
        projectile.gameObject.SetActive(false);
    }
}
