using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpear : Weapon
{
    public GameObject spearProjectile;
    public GameObject MaterializeVfx;
    public float Speed = 50;

    public float timeToMoveSpear = 8;

    private void Start()
    {

    }
   

    public override void Attack()
    {
        
        GameObject current = Instantiate(spearProjectile, transform.position, transform.rotation);
        GameObject mat = Instantiate(MaterializeVfx, transform.position,transform.rotation);
        mat.transform.SetParent(current.transform);

        current.transform.position = transform.position;
        SpearProjectileScript spc = current.GetComponent<SpearProjectileScript>();
        spc.spearParent = gameObject;
        spc.timeToStartMoving = timeToMoveSpear;
        spc.speed = Speed;

        Destroy(current, 3f);
        
    }
}
