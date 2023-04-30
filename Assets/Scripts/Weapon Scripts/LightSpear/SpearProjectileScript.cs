using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearProjectileScript : MonoBehaviour
{
    public bool shouldMove = false;
    public float timeToStartMoving;

    public float speed;
    private Vector2 velocity;
    private LogicScript logic;

    public GameObject onImpactParticles;
    public GameObject onImpactShockWave;

    public GameObject spearParent;

    private GameObject player;
    private PlayerScript playerScript;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();

    }

    // Update is called once per frame
    void Update()
    {
        if (timeToStartMoving > 0)
        {
            timeToStartMoving -= Time.fixedDeltaTime;
            transform.eulerAngles = new Vector3(0,0,playerScript.mouseAngle);
            transform.position = spearParent.transform.position;
            
        }
        else
        {
            velocity = logic.getXAndYByAngle(1, transform.eulerAngles.z) * speed;
            shouldMove = true;
        }

        if (shouldMove == true)
        {
            Vector2 currentPos = transform.position;
            Vector2 newPos = currentPos + velocity * Time.fixedDeltaTime * speed;


            RaycastHit2D[] hits = Physics2D.LinecastAll(currentPos, newPos);

            try
            {
                foreach (RaycastHit2D hit in hits)
                {
                    GameObject current = hit.collider.gameObject;
                    if (current != player.gameObject && !hit.collider.isTrigger)
                    {
                        GameObject oi = Instantiate(onImpactParticles, currentPos - velocity * Time.fixedDeltaTime, transform.rotation);
                        GameObject ois = Instantiate(onImpactShockWave, newPos, transform.rotation);
                        Destroy(oi, 3f);
                        Destroy(ois, 0.5f);

                        Destroy(gameObject);
                        break;
                    }
                }
            }
            catch (System.Exception)
            {

                
            }           
          

            transform.position = new Vector3(newPos.x, newPos.y);
        }

    }
}
