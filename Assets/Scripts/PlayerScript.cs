using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    // Start is called before the first frame update

    private List<Collider2D> triggerCollisiion = new List<Collider2D>();
    private LogicScript logic;
    [Header("Canvas")]
    public Canvas canvas;

    [Header("Energy")]
    public float energy;
    public float maxEnergy;
    public int energyCollectionAmount = 1;
    [HideInInspector] public List<GameObject> hearths = new List<GameObject>();

    [Header("Health")]
    public float currentHealth;
    public float maxHealth = 3;
    public float InvincibilityTime = 1.5f;
    public int NumberOfFlashes = 3;
    private bool isInvincible = false;
    public GameObject HeartPrefab;
    public GameObject emptyHeartPrefab;
    public GameObject damageParticles;
    public Collider2D col;

    [Header("Jumping")]
    public float JumpStrength = 10;
    [HideInInspector]
    public bool isGrounded = false;
    public BoxCollider2D feet;
    public float gravityScale = 4;
    public float velocityClamp = -20;
    [SerializeField]
    public LayerMask jumpableGround;

    [Header("LightWhip")]
    public bool hasWhip;
    public float whipRange;
    public float whipSpeed;
    public GameObject whipIndicator;
    public GameObject whipVfxObject;
    private GameObject closestEn;
    private Vector2 endPos;
    private bool canWhip = false;
    private Collider2D encol;

    public float whipDashSpeed;
    private Vector2 whipDashMovement;
    private float whipDashAngle;
    private bool whipIsDashing = false;
    public float whipDashTime = 1;
    private float whipDashTimer = 0;
    private GameObject closestOnStart;
    public List<Collider2D> whipEnemies;

    [Header("Dashing")]
    public bool hasDash;
    public bool canDash = true;
    public bool isDashing = false;
    public float dashTime = 1;
    public float dashCooldown = 1;
    private float dashCooldownTimer = 0;
    private float dashTimer = 0;
    public float dashSpeed;
    private Vector2 dashMovement;
    private float dashAngle;
    public ParticleSystem dashParticles = null;
    public GameObject dashShockWave;

    [HideInInspector]
    public Vector2 mousePos;
    [HideInInspector]
    public float mouseAngle;

    public GameObject Crosshair;

    [Header("Weapons")]
    public Weapon weapon1Script;
    public Weapon weapon2Script;
    public bool isAttacking = false;

    [Header("Camera")]
    public Camera _camera;
    private CinemachineFramingTransposer cmvCamTransposer;
    public CinemachineVirtualCamera cmvCam;
    public float camDis = 0.5f;
    public Transform camPos;
    public float thresHold = 0;

    [Header("Moving")]
    public Vector2 movement;
    public float speed = 5;
    private int FacingDirection = 0;
    [HideInInspector]
    public string facing = "right";

    public Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;

        for (int i = 0; i < currentHealth; i++)
        {
            GameObject hearth = Instantiate(HeartPrefab,new Vector3(0,0,0), Quaternion.identity);
            hearth.transform.SetParent(canvas.transform);
            hearth.transform.localPosition = new Vector3(-94 + i*1.5f*hearth.transform.localScale.x, 54, -825);
            hearths.Add(hearth);
        }

        cmvCamTransposer = cmvCam.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (weapon1Script != null)
        {
            weapon1Script.transform.SetParent(transform);
            weapon1Script.transform.position = transform.position;
        }
        else
        {
            weapon1Script = new SwordScript();
        }

        if (weapon2Script != null)
        {
            weapon2Script.transform.SetParent(transform);
            weapon2Script.transform.position = transform.position;
        }
        else
        {
            weapon2Script = new SwordScript();
        }
       
        dashParticles.Stop(); // Cannot set duration whilst Particle System is playing
        var main = dashParticles.main;
        main.duration = 0.45f;

        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }
    void Update()
    {
        if (rb.velocity.y > 1)
        {
            if (cmvCamTransposer.m_YDamping < 2)
            {
                cmvCamTransposer.m_YDamping += Time.fixedDeltaTime * 2;
            }
        }
        else if (rb.velocity.y < -1)
        {
            if (cmvCamTransposer.m_YDamping > 0.25)
            {
                cmvCamTransposer.m_YDamping -= Time.fixedDeltaTime * 2;
            }
        }

        isGrounded = GroundCheck();

        if (hasDash) UpdateDash();
        if (hasWhip) UpdateWhip();
        UpdateCamera();
        UpdateAngle();

        weapon1Script.transform.eulerAngles = new Vector3(0, 0, mouseAngle);
        weapon2Script.transform.eulerAngles = new Vector3(0, 0, mouseAngle);
    }

    private bool GroundCheck()
    {
        Collider2D[] c = Physics2D.OverlapBoxAll(feet.bounds.center, feet.bounds.size, 0f);
        foreach (Collider2D col in c)
        {
            if (col.gameObject.layer == 10)
            {
                return true;
            }
        }
        return false;
    }
    private void FixedUpdate()
    {
        try
        {
            

            if (whipIsDashing)
            {
                rb.velocity = whipDashMovement * whipDashSpeed * whipDashTimer;
            }
            else if(isDashing) 
            {
                rb.velocity = dashMovement * dashSpeed * dashTimer;
                //transform.eulerAngles = new Vector3(0, 0, dashAngle);
            }
            else
            {
                rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
                if (movement.x < 0) FacingDirection = 180;
                if (movement.x > 0) FacingDirection = 0;
                transform.eulerAngles = new Vector3(0, FacingDirection, 0);
            }
        }
        catch (System.Exception)
        {

        }
    }

    public void Whip(InputAction.CallbackContext value)
    {
        if (value.started && !whipVfxObject.activeSelf && whipDashTimer <= 0 && hasWhip && canWhip && !isAttacking)
        {
            canWhip = false;
            closestOnStart = closestEn;

            if (closestOnStart != null)
            {
                whipVfxObject.SetActive(true);
            }
            else
            {
                whipVfxObject.SetActive(false);
            }
        }
    }
    public void Dash(InputAction.CallbackContext value)
    {
        if (value.started && hasDash && !whipVfxObject.activeSelf && !isAttacking)
        {
            if (canDash && dashCooldownTimer <= 0)
            {
                if (movement != Vector2.zero)
                {
                    dashMovement = movement;
                    isDashing = true;
                    canDash = false;

                    Vector2 difference = transform.position - transform.position + new Vector3(dashMovement.x, dashMovement.y);
                    dashAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90;
                    if (dashAngle < 0) dashAngle += 360;
                    dashTimer = 1;
                    dashCooldownTimer = dashCooldown;
                    dashParticles.Play();
                    GameObject dS = Instantiate(dashShockWave, transform.position,transform.rotation);
                    dS.SetActive(true);
                    Destroy(dS, 0.5f);

                }
            }
        }
    }
    public void Jump(InputAction.CallbackContext value)
    {
        if (isGrounded)
        {
            if (value.started)
            {
                rb.velocity = new Vector2(rb.velocity.x, JumpStrength);
            }

            

            isGrounded = false;
        }

        if (value.canceled && rb.velocity.y > 0.0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }
    public void SetMovement(InputAction.CallbackContext value)
    {
        movement = value.ReadValue<Vector2>();

        if (movement.y > 0) facing = "up";
        else if (movement.y < 0) facing = "down";
        else
        {
            if (movement.x > 0) facing = "right";
            if (movement.x < 0) facing = "left";
        }
       
        movement.y = 0;
        if (movement.x > 0)
        {
            movement.x = 1;
        }

        if (movement.x < 0)
        {
            movement.x = -1;
        }

        if (value.canceled)
        {
            if (FacingDirection == 0)
            {
                facing = "right";
            }
            else if (FacingDirection == 180)
            {
                facing = "left";
            }
        }
    }
    public void OnMousePos(InputAction.CallbackContext value)
    {
    }

    private void UpdateAngle()
    {
        Vector2 difference = transform.position - Crosshair.transform.position;
        mouseAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg + 90;
        if (mouseAngle < 0)
        {
            mouseAngle += 360;
        }
    }
    private void UpdateCamera()
    {
       
    }
    private void UpdateWhip()
    {
        if (isGrounded && !whipVfxObject.activeSelf) canWhip = true;

        closestEn = null;
        whipEnemies = new List<Collider2D>();
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, whipRange))
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                if (!whipEnemies.Contains(col) && col.isTrigger == false)
                {
                    whipEnemies.Add(col);
                }
            }
        }
        float distance = whipRange;
        foreach (Collider2D en in whipEnemies)
        {
            if (en.gameObject.CompareTag("Enemy"))
            {
                if (Vector2.Distance(transform.position, en.gameObject.transform.position) < distance)
                {
                    distance = Vector2.Distance(transform.position, en.gameObject.transform.position);
                    closestEn = en.gameObject;
                }
            }
        }

        if (whipDashTimer > 0)
        {
            whipDashTimer -= whipDashTime * Time.fixedDeltaTime;
        }
        if (whipDashTimer <= 0)
        {
            whipIsDashing = false;
            if (!whipVfxObject.activeSelf && encol != null)
            {
                Physics2D.IgnoreCollision(col, encol, false);
                isInvincible = false;
            }
        }

        if (closestEn != null && canWhip)
        {
            whipIndicator.SetActive(true);
            whipIndicator.transform.position = closestEn.transform.position;
        }
        else
        {
            whipIndicator.SetActive(false);
        }

        if (!whipVfxObject.activeSelf)
        {
            rb.gravityScale = gravityScale;
        }
        if (whipVfxObject.activeSelf && closestOnStart != null)
        {
            VisualEffect wVfx = whipVfxObject.GetComponent<VisualEffect>();
            if (wVfx.aliveParticleCount <= 0)
            {
                wVfx.Play();
            }
            wVfx.SetVector3("StartPos", transform.position);
            wVfx.SetVector3("EndPos", closestOnStart.transform.position);
            endPos = closestOnStart.transform.position;

            rb.gravityScale = 0;
            isDashing = false;

            Vector2 difference = (Vector2)transform.position - endPos;
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg + 90;
            if (angle < 0)
            {
                angle += 360;
            }

            Vector2 whipMovement = logic.getXAndYByAngle(1, angle);

            Vector2 currentPos = transform.position;
            Vector2 newPos = currentPos + whipMovement * whipSpeed * Time.fixedDeltaTime;
            Vector2 longNewPos = currentPos + whipMovement * (whipSpeed * 10) * Time.fixedDeltaTime;

            RaycastHit2D[] hits = Physics2D.LinecastAll(currentPos, longNewPos);

            bool hitWall = false;
            foreach (RaycastHit2D hit in hits)
            {
                if (!hit.collider.isTrigger)
                {
                    if (hit.collider.gameObject == closestOnStart.gameObject)
                    {
                        //Dash
                        encol = hit.collider;
                        Physics2D.IgnoreCollision(col,encol,true);
                        whipDashMovement = whipMovement;
                        whipIsDashing = true;
                        whipDashTimer = 1;
                        whipDashAngle = angle;

                        dashParticles.Play();
                        GameObject dS = Instantiate(dashShockWave, transform.position, transform.rotation);
                        dS.SetActive(true);
                        Destroy(dS, 0.5f);

                        canDash = true;
                        isInvincible = true;

                        whipVfxObject.SetActive(false);
                    }
                    else if (hit.collider.gameObject.CompareTag("Wall") || hit.collider.gameObject.CompareTag("Ground"))
                    {
                        whipVfxObject.SetActive(false);
                        hitWall = true;
                    }
                }
            }

            if (hitWall)
            {
                transform.position = currentPos;
            }
            else
            {
                transform.position = newPos;
            }

        }
        else if (whipVfxObject.activeSelf && closestOnStart == null)
        {
            whipVfxObject.SetActive(false); 
        }
    }
    private void UpdateDash()
    {
        if (dashTimer > 0)
        {
            dashTimer -= dashTime * Time.fixedDeltaTime;
        }
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= 1 * Time.fixedDeltaTime;
        }
        if (dashTimer <= 0)
        {
            if (isGrounded)
            {
                canDash = true;
            }
            isDashing = false;
        }
    }
    public float GetDashTimer()
    {
        return dashTimer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerCollisiion.Contains(collision))
        {
            return;
        }

        triggerCollisiion.Add(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (triggerCollisiion.Contains(collision))
        {
            triggerCollisiion.Remove(collision);
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Energy"))
        {
            energy++;
            if (energy > maxEnergy) energy = maxEnergy;
        }
    }

    [ContextMenu("TakeDamage")]
    public void TakeDamage(int damage)
    {
        if (!isInvincible && currentHealth > 0)
        {
            StartCoroutine(SetPlayerInvincibility(InvincibilityTime));

            currentHealth -= damage;
            for (int i = 0; i < damage; i++)
            {

                GameObject emphearth = Instantiate(emptyHeartPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                emphearth.transform.SetParent(canvas.transform);
                emphearth.transform.localPosition = hearths[hearths.Count - 1].transform.localPosition;

                GameObject d = Instantiate(damageParticles, new Vector3(0, 0, 0), Quaternion.identity);
                d.transform.localScale = new Vector3(0.5f / emphearth.transform.localScale.x, 0.5f / emphearth.transform.localScale.y, 0.5f / emphearth.transform.localScale.z);
                d.transform.SetParent(canvas.transform);

                d.transform.position = emphearth.transform.position;

                Destroy(hearths[hearths.Count - 1]);
                hearths.RemoveAt(hearths.Count - 1);

            }
        }
    }


    public void Weapon1Attack(InputAction.CallbackContext value)
    {
        if (value.started && !isDashing && !whipVfxObject.activeSelf && !isAttacking)
        {
            weapon1Script.Attack();
        }
    }
    public void Weapon2Attack(InputAction.CallbackContext value)
    {
        if (value.started && !isDashing && !whipVfxObject.activeSelf && !isAttacking)
        {
            weapon2Script.Attack();
        }
    }

    public IEnumerator setPlayerIsAttacking(float time)
    {
        isAttacking = true;
        yield return new WaitForSeconds(time);
        isAttacking = false;
    }
    

    public IEnumerator SetPlayerInvincibility(float time)
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        isInvincible = true;
        for (int i = 0; i < NumberOfFlashes; i++)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f, 0.5f);

            yield return new WaitForSeconds(time / (NumberOfFlashes * 2));
            GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(time / (NumberOfFlashes * 2));
        }
        isInvincible = false;
        Physics2D.IgnoreLayerCollision(7, 8, false);
    }

}

