using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    public GameObject firePoint;
    public GameObject ballPrefab;
    public float power = 40;

    Vector3 CalculateForce()
    {
        return CameraManager.instance.transform.forward * power;
    }
    Vector3 CalculateForce(Vector3 direction)
    {
        return direction * power;
    }

    public void Shoot()
    {
        GameObject ball = Instantiate(ballPrefab, firePoint.transform.position, transform.rotation);
        ball.GetComponent<Rigidbody>().AddForce(CalculateForce(), ForceMode.Impulse);
    }
    public void Shoot(GameObject grenadePrefab)
    {
        GameObject ball = Instantiate(grenadePrefab, firePoint.transform.position, transform.rotation);
        ball.GetComponent<Rigidbody>().AddForce(CalculateForce(), ForceMode.Impulse);
    }
    public void Shoot(GameObject grenadePrefab, Vector3 direction)
    {
        GameObject ball = Instantiate(grenadePrefab, firePoint.transform.position, transform.rotation);
        ball.GetComponent<Rigidbody>().AddForce(CalculateForce(direction), ForceMode.Impulse);
    }
    public void Shoot(Vector3 direction)
    {
        GameObject ball = Instantiate(ballPrefab, firePoint.transform.position, transform.rotation);
        ball.GetComponent<Rigidbody>().AddForce(CalculateForce(direction), ForceMode.Impulse);
    }
    public void Shoot(Vector3 direction, Transform origin)
    {
        GameObject ball = Instantiate(ballPrefab, origin.transform.position, origin.transform.rotation);
        ball.GetComponent<Rigidbody>().AddForce(CalculateForce(direction), ForceMode.Impulse);
    }
    public void Shoot(Vector3 direction, Transform projectile, Vector3 origin, Quaternion rotation)
    {
        projectile.position = origin;
        projectile.rotation = rotation;
        projectile.GetComponent<Rigidbody>().AddForce(CalculateForce(direction), ForceMode.Impulse);
    }
    public void Shoot(Transform projectile, Transform origin)
    {
        projectile.position = origin.position;
        projectile.rotation = origin.rotation;
        projectile.GetComponent<Rigidbody>().AddForce(CalculateForce(projectile.forward), ForceMode.Impulse);
    }
    public void Shoot(Transform projectile)
    {
        projectile.position = firePoint.transform.position;
        projectile.rotation = firePoint.transform.rotation;
        projectile.GetComponent<Rigidbody>().AddForce(CalculateForce(firePoint.transform.forward), ForceMode.Impulse);
    }
    public void ShootFromOrigin(Transform origin)
    {
        GameObject ball = Instantiate(ballPrefab, origin.transform.position, origin.transform.rotation);
        ball.GetComponent<Rigidbody>().AddForce(CalculateForce(origin.forward), ForceMode.Impulse);
    }
}