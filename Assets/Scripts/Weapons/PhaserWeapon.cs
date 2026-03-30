using UnityEngine;

public class PhaserWeapon : Weapon
{
    public static PhaserWeapon Instance;
    [SerializeField] private ObjectPooler bulletPool;

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

    public void Shoot(){
        AudioManager.Instance.PlayModifieldSound(AudioManager.Instance.shoot);

        for (int i = 0; i < stats[weaponLevel].amount; i++){
            GameObject bullet = bulletPool.GetPooledObject();
            float yPos = transform.position.y;

            if (stats[weaponLevel].amount > 1){
                float spacing = stats[weaponLevel].range / (stats[weaponLevel].amount - 1);
                yPos = transform.position.y - (stats[weaponLevel].range / 2) + i * spacing;
            }

            bullet.transform.position = new Vector2(transform.position.x, yPos);
            bullet.transform.localScale = new Vector2(stats[weaponLevel].size, stats[weaponLevel].size);
            bullet.SetActive(true);
        }
    }

    public void LevelUp()
    {
        if (weaponLevel < stats.Count - 1)
        {
            weaponLevel++;
        }
    }
}