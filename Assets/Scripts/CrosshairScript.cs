using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairScript : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> triggerColliders = new List<GameObject>();
    public Transform lockPos;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        GameObject closest = null;
        float distance = 1000f;
        foreach (GameObject gm in triggerColliders)
        {
            if (Vector2.Distance(transform.position, gm.transform.position) < distance)
            {
                distance = Vector2.Distance(gameObject.transform.position, gm.transform.position);
                closest = gm;
            }
        }
        Vector2 position = Vector2.zero;
        position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = position;

        if (closest != null)
        {
            lockPos.position = closest.transform.position;
        }
        else
        {
            lockPos.position = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggerColliders.Contains(collision.gameObject))
        {
            if (collision.gameObject.tag == "Enemy")
            {
                triggerColliders.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (triggerColliders.Contains(collision.gameObject))
        {
            triggerColliders.Remove(collision.gameObject);
        }
    }
}
