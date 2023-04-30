using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : Weapon
{
    public GameObject Slash;
    public float damage = 5;

    private PlayerScript player;
    private GameObject g;
    private BoxCollider2D gCol;
    private PolygonCollider2D pCol;

    private bool hasPogoed = false;

    private List<GameObject> hitEnemiesList = new List<GameObject>();

    private int scale = 1;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }
    private void Update()
    {
        if (g != null)
        {
            g.transform.position = player.transform.position;

            Collider2D[] hits = Physics2D.OverlapBoxAll(gCol.bounds.center,gCol.bounds.size,0f);

            
            foreach (Collider2D hit in hits)
            {
                if (pCol.IsTouching(hit))
                {
                    if (hit.gameObject.CompareTag("Wall") || hit.gameObject.CompareTag("Ground"))
                    {
                        break;
                    }
                    if (hit.gameObject.CompareTag("Enemy") && !hitEnemiesList.Contains(hit.gameObject))
                    {
                        EnemyScript en = hit.gameObject.GetComponent<EnemyScript>();
                        hitEnemiesList.Add(hit.gameObject);
                        en.TakeDamage(damage, g.transform);

                        if (g.transform.eulerAngles.z == 180 && en.givesUpwardVelocity && !hasPogoed)
                        {
                            player.rb.velocity = new Vector2(player.rb.velocity.x,player.JumpStrength/1.5f);
                            hasPogoed = true;
                        }
                    }
                }
            }
        }
        else
        {
            hitEnemiesList = new List<GameObject>();
        }
    }

    public override void Attack()
    {
        if (g == null)
        {
            Vector3 v = Vector3.zero;

            if (player.facing == "right") v = new Vector3(0, 0, -90);
            else if (player.facing == "left") v = new Vector3(0, 0, 90);
            else if (player.facing == "up") v = new Vector3(0, 0, 0);
            else if (player.facing == "down") v = new Vector3(0, 0, 180);

            if (player.facing == "down" && player.isGrounded) { }
            else
            {
                if (player.facing == "down" && player.GetDashTimer() <= 0)
                {
                    player.canDash = true;
                }

                hasPogoed = false;
                g = Instantiate(Slash, transform.position, transform.rotation);
                g.transform.localScale = new Vector3(g.transform.localScale.x * scale, g.transform.localScale.y, g.transform.localScale.z);
                g.transform.eulerAngles = v;
                g.SetActive(true);
                gCol = g.GetComponent<BoxCollider2D>();
                pCol = g.GetComponent<PolygonCollider2D>();
                Destroy(g, 0.3f);

                

                if (scale > 0) scale -= 2;
                else if (scale < 0) scale += 2;
            }
        }
    }
}
