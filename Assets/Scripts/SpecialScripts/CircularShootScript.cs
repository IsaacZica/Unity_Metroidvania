using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CircularShootScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject projectile;
    public float projectileSpeed;
    public int projectileAmount;
    public float distanceFromCenter;


    private float fireRate;
    private float timer;

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
        
    }

    public void OnNaoSei(InputAction.CallbackContext value)
    {
        float angle = 360 / projectileAmount;
        Debug.Log(transform.rotation);
        Debug.Log(transform.eulerAngles);
        for (int i = 0; i < projectileAmount; i++)
        {
            Vector3 v3 = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle * i);
            transform.eulerAngles = v3;

            

            Quaternion q = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z + 90, transform.rotation.w);

            Instantiate(projectile,transform.position, q);

            //projectileScript.create(projectile, transform.position + logic.getXAndYByAngle(distanceFromCenter, angle * i), transform.rotation, projectileSpeed);
        }
    }
}
