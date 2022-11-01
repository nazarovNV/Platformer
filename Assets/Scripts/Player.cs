using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    public new Rigidbody2D rigidbody2D;
    public float force;
    public float minimalheight;
    public bool isCheatMode;
    public GroundDetection groundDetection;
    private Vector3 direction;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public bool isJumping;
    [SerializeField] private Arrow arrow;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float shootForce;
    [SerializeField] private float cooldawnTimer;
    [SerializeField] private float isJumpingCooldownTimer;
    [SerializeField] private bool isCooldown;
    [SerializeField] private float damageForce;
    [SerializeField] private int arrowsCount = 3;
    [SerializeField] private Health health;
    [SerializeField] private Item item;
    [SerializeField] private BuffReciever buffReciever;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Component player;
    [SerializeField] private HideFlags playerHideFlags;

    private float bonusForce;
    private float bonusDamage;
    private float bonusHealth;
    private bool isBlockMovement;
    private UICharacterController controller;

    public Health Health
        { get { return health; } }
    private Arrow currentArrow;
    private List<Arrow> arrowPool;
    public static Player Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        HideFlagsBugFix.SetHideFlagsRecursively(player, playerHideFlags);
    }
    private void Start()
    {
       
        GameManager.Instance.playerContainer.Add(gameObject, this);

        arrowPool = new List<Arrow>();
        for(int i=0; i<arrowsCount; i++)
        {
            var arrowTemp= Instantiate(arrow, arrowSpawnPoint);
            arrowPool.Add(arrowTemp);
            arrowTemp.gameObject.SetActive(false);
        }
        health.OnTakeHit += TakeHit;
        buffReciever.OnBuffsChanged += ApplyBuffs;
    }

    public void InitUIController(UICharacterController uiController)
    {
        controller = uiController;
        controller.Jump.onClick.AddListener(Jump);
        controller.Fire.onClick.AddListener(CheckShoot);
    }

    private void ApplyBuffs()
    {
        var forceBuff = buffReciever.Buffs.Find(t => t.type == BuffType.Force);
        var damageBuff = buffReciever.Buffs.Find(t => t.type == BuffType.Damage);
        var armorBuff = buffReciever.Buffs.Find(t => t.type == BuffType.Armor);
        bonusForce = forceBuff == null ? 0 : forceBuff.additiveBonus;
        bonusHealth = armorBuff == null ? 0 : armorBuff.additiveBonus;
        health.SetHealth((int)bonusHealth);
        bonusDamage = damageBuff == null ? 0 : damageBuff.additiveBonus;

    }

    private void TakeHit(int damage,GameObject attacker)
    {
        animator.SetBool("GetDamage",true);
        animator.SetTrigger("TakeHit");
        isBlockMovement = true;
        rigidbody2D.AddForce(transform.position.x < attacker.transform.position.x?
            new Vector2(-damageForce,0):new Vector2(damageForce,0),ForceMode2D.Impulse);
    }
    public void UnblockMovement()
    {
        isBlockMovement = false;
        animator.SetBool("GetDamage", true);
    }

    void FixedUpdate()
    {
        Move();
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
#endif
        animator.SetFloat("Speed",Mathf.Abs(direction.x));
        CheckFall();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
#endif
    }

    private void Move()
    {
        if (isJumping == true && groundDetection.isGrounded == true)
            isJumping = false;
        animator.SetBool("isGrounded", groundDetection.isGrounded);
        if (!groundDetection.isGrounded&&!isJumping)
        {
            animator.SetBool("StartFall",true);
        }
        if(groundDetection.isGrounded)
            animator.SetBool("StartFall", false);
        //isJumping = isJumping && !groundDetection.isGrounded;
        direction = Vector3.zero;//(0;0)
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.A))
            direction = Vector3.left;
        if (Input.GetKey(KeyCode.D))
            direction = Vector3.right;
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.OnClickPause();
        }
#endif
        if (controller.Left.IsPressed)
        {
            direction = Vector3.left;//(-1;0)
        }
        if (controller.Right.IsPressed)
        {
            direction = Vector3.right;//(1;0)
        }
        direction *= speed;
        direction.y = rigidbody2D.velocity.y;
        if (!isBlockMovement)
        rigidbody2D.velocity = direction;


        if (direction.x > 0)
            spriteRenderer.flipX = false;
        if (direction.x < 0)
            spriteRenderer.flipX = true;
    }

    private void Jump()
    {
        if (groundDetection.isGrounded)
        {
            animator.SetTrigger("StartJump");
            Invoke("IsJumpingSettingTrue", 0.1F); 
            rigidbody2D.AddForce(Vector2.up * (force + bonusForce), ForceMode2D.Impulse);
            
        }
    }
    private void IsJumpingSettingTrue()
    {
        isJumping = true;
    }

    void CheckShoot()

    {
        if(!isCooldown)
        {
            animator.SetTrigger("StartShoot");
        }
    }

    public void InitArrow()
    {
        currentArrow = GetArrowFromPool();
        currentArrow.SetImpulse(Vector2.right, 0,0, this);
    }
    void CheckFall()
    {
        if (transform.position.y < minimalheight && isCheatMode)
        {
            rigidbody2D.velocity = new Vector2(0, 0);
            transform.position = new Vector3(0, 0, 0);
        }
        else if (transform.position.y < minimalheight && !isCheatMode)
        {
            Destroy(gameObject);
        }
    }

    void Shoot()
    {

        currentArrow.SetImpulse(Vector2.right, spriteRenderer.flipX ?
            -force * shootForce : force * shootForce,(int) bonusDamage, this);

        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.1f);
    }
    
    private Arrow GetArrowFromPool()
    {
        if(arrowPool.Count>0)
        {
            var arrowTemp = arrowPool[0];
            arrowPool.Remove(arrowTemp);
            arrowTemp.gameObject.SetActive(true);
            arrowTemp.transform.parent = null;
            arrowTemp.transform.position = arrowSpawnPoint.transform.position;
            return arrowTemp;
        }
        return Instantiate(arrow, arrowSpawnPoint.position, Quaternion.identity);
    }

    public void ReturnArrowToPool(Arrow arrowTemp)
    {
        if(!arrowPool.Contains(arrowTemp))
        {
            arrowPool.Add(arrowTemp);
        }

        arrowTemp.transform.parent = arrowSpawnPoint;
        arrowTemp.transform.position = arrowSpawnPoint.transform.position;
        arrowTemp.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        //HideFlagsBugFix.SetHideFlagsRecursively(playerCamera,flagsCamera);
        playerCamera.transform.parent = null;
        playerCamera.enabled = true;
    }
}

public static class HideFlagsBugFix
{
    public static void SetHideFlagsRecursively(this Component obj, HideFlags flags)
    {
        foreach (var child in obj.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.hideFlags = flags;
        }
    }
}



