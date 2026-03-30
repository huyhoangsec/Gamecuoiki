using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private Rigidbody2D rb;
    private Animator animator;
    private FlashWhite flashWhite;

    private Vector2 playerDirection;
    [SerializeField] private float moveSpeed;
    public bool boosting = false;

    [SerializeField] private float energy;
    [SerializeField] private float maxEnergy;
    [SerializeField] private float energyRegen;

    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    private ObjectPooler destroyEffectPool;
    [SerializeField] private ParticleSystem engineEffect;

    [SerializeField] private int experience;
    [SerializeField] private int currentLevel;
    [SerializeField] private int maxLevel;
    [SerializeField] private List<int> playerLevels;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        flashWhite = GetComponent<FlashWhite>();
        destroyEffectPool = GameObject.Find("Boom1Pool").GetComponent<ObjectPooler>();

        for (int i = playerLevels.Count; i < maxLevel; i++){
            playerLevels.Add(Mathf.CeilToInt(playerLevels[playerLevels.Count - 1] * 1.1f + 15));
        }

        energy = maxEnergy;
        UIController.Instance.UpdateEnergySlider(energy, maxEnergy);
        health = maxHealth;
        UIController.Instance.UpdateHealthSlider(health, maxHealth);
        experience = 0;
        UIController.Instance.UpdateExperienceSlider(experience, playerLevels[currentLevel]);
    }

    void Update()
    {
        if (Time.timeScale > 0)
        {
            float directionX = Input.GetAxisRaw("Horizontal");
            float directionY = Input.GetAxisRaw("Vertical");

            animator.SetFloat("moveX", directionX);
            animator.SetFloat("moveY", directionY);

            playerDirection = new Vector2(directionX, directionY).normalized;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire2"))
            {
                EnterBoost();
            }
            else if (Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Fire2"))
            {
                ExitBoost();
            }


            if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetButtonDown("Fire1"))
            {
                PhaserWeapon.Instance.Shoot();
            }
        }
    }
    void FixedUpdate()
        {
            rb.linearVelocity = new Vector2(playerDirection.x * moveSpeed,
                                            playerDirection.y * moveSpeed);

        if (boosting){
            if (energy >= 0.5f) energy -= 0.5f;
            else{
                ExitBoost();
            }
        }else{
            if (energy < maxEnergy){
                energy += energyRegen;
            }
        }
        UIController.Instance.UpdateEnergySlider(energy, maxEnergy);
    }

    private void EnterBoost()
    {
        if (energy > 10)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.fire);
            animator.SetBool("boosting", true);
            GameManager.Instance.SetWorldSpeed(7f);
            boosting = true;
            engineEffect.Play();
        }
    }

    public void ExitBoost()
    {
        animator.SetBool("boosting", false);
        GameManager.Instance.SetWorldSpeed(1f);
        boosting = false;
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
            if (asteroid) asteroid.TakeDamage(1, false);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        UIController.Instance.UpdateHealthSlider(health, maxHealth);
        AudioManager.Instance.PlaySound(AudioManager.Instance.hit);
        flashWhite.Flash();
        if (health <= 0)
        {
            ExitBoost();
            GameManager.Instance.SetWorldSpeed(0f);
            gameObject.SetActive(false);
            GameObject destroyEffect = destroyEffectPool.GetPooledObject();
            destroyEffect.transform.position = transform.position;
            destroyEffect.transform.rotation = transform.rotation;
            destroyEffect.SetActive(true);
            GameManager.Instance.GameOver();
            AudioManager.Instance.PlaySound(AudioManager.Instance.ice);
        }
    }

    public void GetExperience(int exp){
        experience += exp;
        UIController.Instance.UpdateExperienceSlider(experience, playerLevels[currentLevel]);
        if (experience > playerLevels[currentLevel]){
            LevelUp();
        }
    }

    public void LevelUp(){
        experience -= playerLevels[currentLevel];
        if (currentLevel < maxLevel - 1)
            currentLevel++;
        UIController.Instance.UpdateExperienceSlider(experience, playerLevels[currentLevel]);
        PhaserWeapon.Instance.LevelUp();
        maxHealth++;
        health = maxHealth;
        UIController.Instance.UpdateHealthSlider(health, maxHealth);
    }
}