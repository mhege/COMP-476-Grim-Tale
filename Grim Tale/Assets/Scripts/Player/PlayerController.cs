using System;
using AI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IHealable
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private LightProjectile lightProjectile;
    [SerializeField] private HeavyProjectile heavyProjectile;
    [SerializeField] private float lightProjectileSpeed;
    [SerializeField] private float heavyProjectileSpeed;
    [SerializeField] private float lightProjectileDamage;
    [SerializeField] private float heavyProjectileDamage;
    [SerializeField] private Transform rightCastSpawn;
    [SerializeField] private Transform leftCastSpawn;
    //If these are changed, change the text in the buy menu!
    [SerializeField] private float scaleAttackDMG;
    [SerializeField] private float scaleAttackSPD;
    ////////////////////////////////////////////////////////
    [SerializeField] private float heavyAttackTime = 0.5f;
    [SerializeField] private int heavyManaCost = 2;
    [SerializeField] private float lightSpellRechargeTime;
    private float attackSPDIncrementLight;
    private float attackSPDIncrementHeavy;
    private float attackDMGincrementLight;
    private float attackDMGincrementHeavy;
    private float lightSpellRechargeTimeIncrement;

    [HideInInspector] public PlayerInput input;
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    
    private Camera cam;
    private Vector3 camOffset;

    private bool heavyAttackIsUsable = true;
    private float timeRemaining;
    private float lightSpell;
    private float heavySpell;
    private float lightSpellRechargeRemainingTime;
    private bool lightSpellIsRecharging = false;
    private bool isLeftCast;
    private bool nextIsHeavy;

    [SerializeField] private float healHPScale;
    [SerializeField] private float healManaScale;
    private float HpIncrement;
    private float ManaIncrement;
    public HealthBar healthBar;
    public ManaBar manaBar;
    private float maxHealth = 5.0f;
    private float health;
    private float maxMana = 5.0f;
    private float mana;
    private goldText goldtext;
    private int gold;

    private bool isDead;
    
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Direction = Animator.StringToHash("Direction");
    private static readonly int LeftCast = Animator.StringToHash("LeftCast");
    private static readonly int RightCast = Animator.StringToHash("RightCast");

    //Static variables for transition
    private static int goldStatic;
    private static float healthStatic;
    private static float manaStatic;
    private static float lightProjectileSpeedStatic;
    private static float heavyProjectileSpeedStatic;
    private static float lightProjectileDamageStatic;
    private static float heavyProjectileDamageStatic;
    private static float lightSpellRechargeTimeStatic;

    private float re = .3f;

    private void Awake()
    {
        input = new PlayerInput();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cam = Camera.main;
    }

    private void Start()
    {
        camOffset = new Vector3(0f, 11.11f, -4.18f);
        timeRemaining = heavyAttackTime;

        //Fixed increments for scaling
        attackSPDIncrementLight = scaleAttackSPD * lightProjectileSpeed;
        attackSPDIncrementHeavy = scaleAttackSPD * heavyProjectileSpeed;
        attackDMGincrementLight = scaleAttackDMG * lightProjectileDamage;
        attackDMGincrementHeavy = scaleAttackDMG * heavyProjectileDamage;
        lightSpellRechargeTimeIncrement = scaleAttackSPD/2 * lightSpellRechargeTime;

        //UI elements to initialize
        HpIncrement = maxHealth * healHPScale;
        ManaIncrement = maxMana * healManaScale;
        goldtext = GameObject.FindGameObjectWithTag("goldtext").GetComponent<goldText>();

        if (SceneManager.GetActiveScene().buildIndex <= 1)
        {
            healthBar.SetMaxHealth(maxHealth);
            manaBar.SetMaxMana(maxMana);
            health = maxHealth;
            mana = maxMana;
            gold = 0;
        }
        else
        {
            healthBar.SetMaxHealth(maxHealth);
            manaBar.SetMaxMana(maxMana);
            health = healthStatic;
            mana = manaStatic;
            healthBar.SetHealth(health);
            manaBar.SetMana(mana);

            lightProjectileSpeed = lightProjectileSpeedStatic;
            heavyProjectileSpeed = heavyProjectileSpeedStatic;
            lightProjectileDamage = lightProjectileDamageStatic;
            heavyProjectileDamage = heavyProjectileDamageStatic;

            gold = goldStatic;
            goldtext.updateGoldText(gold);
        }

        lightSpellRechargeRemainingTime = lightSpellRechargeTime;
    }

    private void Update()
    {
        //Update static variable
        healthStatic = health;
        manaStatic = mana;
        lightProjectileSpeedStatic = lightProjectileSpeed;
        heavyProjectileSpeedStatic = heavyProjectileSpeed;
        lightProjectileDamageStatic = lightProjectileDamage;
        heavyProjectileDamageStatic = heavyProjectileDamage;
        goldStatic = gold;
        lightSpellRechargeTimeStatic = lightSpellRechargeTime;

        //Debug.Log(health);
        //Debug.Log(mana);

        IsTooClose();

        if (isDead) return;
        
        View();
        Move();

        if(Time.timeScale != 0)
        {
            Rotate();
            Animate();
        }

        LightSpellTimer();
    }

    private void IsTooClose()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, re);

        foreach (var hitCollider in hitColliders)
        {
            //Add other enemy types.
            if ((hitCollider.transform.CompareTag("skelly") || hitCollider.transform.CompareTag("warchief") || hitCollider.transform.CompareTag("shaman") || hitCollider.transform.CompareTag("charger")) && hitCollider.transform != gameObject.transform)
            {
                if ((hitCollider.transform.position - this.transform.position).magnitude < re)
                {
                    Vector3 direction = ((hitCollider.transform.position - this.transform.position).normalized) * (re + .4f);
                    hitCollider.transform.position = this.transform.position + direction;
                }
            }
        }
    }

    public void Damage(int amount)
    {
        if (isDead) return;

        FindObjectOfType<AudioManager>().PlayOneShot(Clip.PlayerHit);

        health = Mathf.Max(health - amount, 0);
        healthBar.SetHealth(health);
        if (isDead || !health.Equals(0)) return;
        
        isDead = true;
        animator.SetTrigger(Die);
        Invoke(nameof(LoadMainScreen), 1.5f);
    }

    public void Loot(int amount)
    {
        gold += amount;
    }

    public int getGold()
    {
        return gold;
    }

    public void setGold(int amount)
    {
        gold = amount;
        goldtext.updateGoldText(gold);
    }

    public bool IsDamaged()
    {
        return health < maxHealth;
    }

    public void incrementLightAttackDMG()
    {
        lightProjectileDamage += attackDMGincrementLight;
    }

    public void incrementHeavyAttackDMG()
    {
        heavyProjectileDamage += attackDMGincrementHeavy;
    }

    public void incrementLightAttackSPD()
    {
        lightProjectileSpeed += attackSPDIncrementLight;
        lightSpellRechargeTime -= lightSpellRechargeTimeIncrement;
    }

    public void incrementHeavyAttackSPD()
    {
        heavyProjectileSpeed += attackSPDIncrementHeavy;
    }

    public void Heal()
    {
        health = Mathf.Min(health + HpIncrement, maxHealth);
        healthBar.SetHealth(health);
    }
    
    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        healthBar.SetHealth(health);
    }

    public void RegenMana()
    {
        mana = Mathf.Min(mana + ManaIncrement, maxMana);
        manaBar.SetMana(mana);
    }

    private void heavySpellCasted(int amount)
    {
        if(amount <= mana)
            mana -= amount;

        manaBar.SetMana(mana);
    }

    private void LoadMainScreen()
    {
        SceneManager.LoadScene("StartMenu");
    }

    private void View()
    {
        cam.transform.position = transform.position + camOffset;
    }

    private void Move()
    {
        controller.Move(velocity * Time.deltaTime * speed);
    }

    private void Rotate()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        var screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        var relativePosition = mousePosition - screenCenter;
        var position = transform.position;
        var lookDirection = (position + new Vector3(relativePosition.x, 0f, relativePosition.y)).normalized;
        var lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        
        transform.rotation = lookRotation;
    }

    private void Animate()
    {
        var angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
        if (velocity == Vector3.zero)
        {
            animator.SetInteger(Direction, 0);
        }
        else if (angle > -15f && angle <= 15f)
        {
            animator.SetInteger(Direction, 1);
        }
        else if (angle > 15f && angle <= 75f)
        {
            animator.SetInteger(Direction, 2);
        }
        else if (angle > 75f && angle <= 105f)
        {
            animator.SetInteger(Direction, 3);
        }
        else if (angle > 105f && angle <= 165f)
        {
            animator.SetInteger(Direction, 4);
        }
        else if (angle > 165f || angle <= -165f)
        {
            animator.SetInteger(Direction, 5);
        }
        else if (angle > -165f && angle <= -105f)
        {
            animator.SetInteger(Direction, 6);
        }
        else if (angle > -105f && angle <= -75f)
        {
            animator.SetInteger(Direction, 7);
        }
        else if (angle > -75f && angle <= -15f)
        {
            animator.SetInteger(Direction, 8);
        }
    }

    private void LightSpell()
    {
        if (isDead) return;

        if (!lightSpellIsRecharging)
        {
            lightSpellIsRecharging = true;
            lightSpellRechargeRemainingTime = lightSpellRechargeTime;
            
            if (isLeftCast)
            {
                animator.SetTrigger(LeftCast);
                isLeftCast = false;
            }
            else
            {
                animator.SetTrigger(RightCast);
                isLeftCast = true;
            }
            
            // var start = transform.position;
            // start += new Vector3(0, 1, 0);
            // var projectile = Instantiate(lightProjectile, start, transform.rotation);
            // projectile.SetSpeed(lightProjectileSpeed);
            // lightSpellRechargeRemainingTime = lightSpellRechargeTime;
            // lightSpellIsRecharging = true;
        }
    }

    private void InstantiateLeftSpell()
    {
        if (isDead) return;

        
        if (nextIsHeavy)
        {
            FindObjectOfType<AudioManager>().PlayOneShot(Clip.HeavyCast);
            var hProjectile = Instantiate(heavyProjectile, leftCastSpawn.position, leftCastSpawn.rotation);
            hProjectile.SetSpeed(heavyProjectileSpeed);
            nextIsHeavy = false;

            return;
        }
        
        FindObjectOfType<AudioManager>().PlayOneShot(Clip.LightCast);
        var lProjectile = Instantiate(lightProjectile, leftCastSpawn.position, leftCastSpawn.rotation);
        lProjectile.SetSpeed(lightProjectileSpeed);
    }
    
    private void InstantiateRightSpell()
    {
        if (isDead) return;
        
        if (nextIsHeavy)
        {
            FindObjectOfType<AudioManager>().PlayOneShot(Clip.HeavyCast);
            var hProjectile = Instantiate(heavyProjectile, rightCastSpawn.position, rightCastSpawn.rotation);
            hProjectile.SetSpeed(heavyProjectileSpeed);
            nextIsHeavy = false;

            return;
        }
        
        FindObjectOfType<AudioManager>().PlayOneShot(Clip.LightCast);
        var projectile = Instantiate(lightProjectile, rightCastSpawn.position, rightCastSpawn.rotation);
        projectile.SetSpeed(lightProjectileSpeed);
    }

    private void LightSpellTimer()
    {
        if (lightSpellIsRecharging)
        {
            if (lightSpellRechargeRemainingTime > 0)
                lightSpellRechargeRemainingTime -= Time.deltaTime;
            else
            {
                lightSpellIsRecharging = false;
            }
        }
    }

    private void HeavySpell()
    {
        if (isDead) return;

        nextIsHeavy = true;
        
        if (isLeftCast)
        {
            animator.SetTrigger(LeftCast);
            isLeftCast = false;
        }
        else
        {
            animator.SetTrigger(RightCast);
            isLeftCast = true;
        }
        
        // var start = transform.position;
        // start += new Vector3(0, 1, 0);
        // var projectile = Instantiate(heavyProjectile, start, transform.rotation);
        // projectile.SetSpeed(heavyProjectileSpeed);
    }
    
    #region Input

    private void OnEnable()
    {
        input.Controls.Enable();
        input.Controls.Move.performed += ReadMoveInput;
        input.Controls.Move.canceled += ReadMoveInput;
        input.Controls.SmallSpell.performed += ReadLightSpellInput;
        input.Controls.SmallSpell.canceled += ReadLightSpellInput;
        input.Controls.BigSpell.performed += ReadHeavySpellInput;
        input.Controls.BigSpell.canceled += ReadHeavySpellInput;

        if (heavySpell == 0)
        {
            timeRemaining = heavyAttackTime;
            heavyAttackIsUsable = true;
        }
    }

    private void OnDisable()
    {
        input.Controls.Disable();
    }

    private void ReadMoveInput(InputAction.CallbackContext context)
    {
        var vector = context.action.ReadValue<Vector2>();
        velocity = new Vector3(vector.x, 0f, vector.y);
    }

    private void ReadLightSpellInput(InputAction.CallbackContext context)
    {   
        lightSpell = context.action.ReadValue<float>();

        if (Math.Abs(lightSpell - 1) < 0.01f)
        {
            LightSpell();
        }
    }

    private void ReadHeavySpellInput(InputAction.CallbackContext context)
    {
        heavySpell = context.action.ReadValue<float>();

        if (Math.Abs(heavySpell - 1) < 0.01f && mana > heavyManaCost)
        {
            HeavySpell();
            heavySpellCasted(heavyManaCost);
        }
    }

    #endregion
}
