using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getXAndYByAngle(float distance, float angle)
    {
        angle += 90;
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;
        return new Vector3(x, y, 0);
    }
}
