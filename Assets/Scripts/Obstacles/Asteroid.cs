using System.Collections;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private FlashWhite flashWhite;
    private ObjectPooler destroyEffectsPool;

    private int lives;
    private int maxLives = 5;
    private int damage = 1;
    private int experienceToGive = 1;

    [SerializeField] private Sprite[] sprites;
    float pushX;
    float pushY;

    private void OnEnable(){
        lives = maxLives;
        transform.rotation = Quaternion.identity;
        pushX = Random.Range(-1f, 0);
        pushY = Random.Range(-1f, 1);
        if (rb) rb.linearVelocity = new Vector2(pushX, pushY);
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        pushX = Random.Range(-1f, 0);
        pushY = Random.Range(-1f, 1);
        flashWhite = GetComponent<FlashWhite>();
        destroyEffectsPool = GameObject.Find("Boom2Pool").GetComponent<ObjectPooler>();
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        float randomScale = Random.Range(0.6f, 1f);
        transform.localScale = new Vector2(randomScale, randomScale);
        lives = maxLives;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")){
            PlayerController player = collision.gameObject.
            GetComponent<PlayerController>();
            if (player) player.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage, bool giveExperience){
        AudioManager.Instance.PlayModifieldSound(AudioManager.Instance.hitRock);
        lives -= damage;
        if (lives > 0){
            flashWhite.Flash();
        } else {
            GameObject destroyEffect = destroyEffectsPool.GetPooledObject();
            destroyEffect.transform.position = transform.position;
            destroyEffect.transform.rotation = transform.rotation;
            destroyEffect.transform.localScale = transform.localScale;
            destroyEffect.SetActive(true);

            AudioManager.Instance.PlayModifieldSound(AudioManager.Instance.boom2);
            flashWhite.Reset();
            gameObject.SetActive(false);
            if(giveExperience) PlayerController.Instance.GetExperience(experienceToGive);
        }
    }
}