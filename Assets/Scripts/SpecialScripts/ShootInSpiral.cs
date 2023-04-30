using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootInSpiral : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject projectile;
    public float projectileSpeed;
    public float distanceFromCenter;
    public float rotationSpeed;

    public float fireRate;
    public float timer;

    private LogicScript logic;
    private ProjectileScript projectileScript;
    void Start()
    {
        timer = fireRate;

        projectileScript = projectile.GetComponent<ProjectileScript>();
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= 1 * Time.fixedDeltaTime * fireRate;
        if (timer <= 0)
        {
            shoot();
            timer = 1;
        }

        transform.Rotate(new Vector3(0,0,rotationSpeed));
    }

    void shoot()
    {
        //projectileScript.create(projectile, transform.position + logic.getXAndYByAngle(distanceFromCenter, transform.eulerAngles.z * Mathf.Rad2Deg), transform.rotation, projectileSpeed);
    }
}
