using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    // Start is called before the first frame update
    private LogicScript logic;
    public Rigidbody2D rb;
    public float speed;
    public float damage;
    public float spread;

    public GameObject onImpact;

    [HideInInspector]
    public float z;

    public GameObject gameObj; // whoever shot the projectile

    public Vector2 velocity = new Vector2(0.0f, 0.0f);

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        velocity = logic.getXAndYByAngle(1, transform.eulerAngles.z + spread) * speed;
        transform.rotation = new Quaternion(0,0,0,0);
    }

    // Update is called once per frame

    void Update()
    {
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 newPositon = currentPosition + velocity * speed * Time.deltaTime;

        RaycastHit2D[] hits = Physics2D.LinecastAll(currentPosition, newPositon);

        foreach (RaycastHit2D hit in hits)
        {
            GameObject current = hit.collider.gameObject;
            if (current != gameObj)
            {
                //do stuff
                if (current.CompareTag("Enemy"))
                {
                    Destroy(gameObject);

                   
                }

                else if (current.CompareTag("Wall"))
                {
                    Destroy(gameObject);

                    GameObject oi = Instantiate(onImpact, transform.position, transform.rotation);
                    Destroy(oi, 0.5f);
                }

            }
        }

        transform.position = new Vector3(newPositon.x,newPositon.y,z);
    }
}
