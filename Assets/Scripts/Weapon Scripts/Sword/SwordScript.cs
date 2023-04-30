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
                g = Instantiate(Slash, transform.position, transform.rotation);
                g.transform.localScale = new Vector3(g.transform.localScale.x * scale, g.transform.localScale.y, g.transform.localScale.z);
                g.transform.eulerAngles = v;
                g.SetActive(true);
                gCol = g.GetComponent<BoxCollider2D>();
                Destroy(g, 0.3f);

                if (scale > 0) scale -= 2;
                else if (scale < 0) scale += 2;
            }
        }
    }
}
