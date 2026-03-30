using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float worldSpeed;

    public int critterCounter;
    private ObjectPooler boss1Pool;

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

    void Start(){
        boss1Pool = GameObject.Find("Boss1Pool").GetComponent<ObjectPooler>();
        critterCounter = 0;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P) || Input.GetButtonDown("Fire3"))
        {
            Pause();
        }

        if (critterCounter >= 5){
            critterCounter = 0;
            GameObject boss1 = boss1Pool.GetPooledObject();
            boss1.transform.position = new Vector2(15f, 0);
            boss1.transform.rotation = Quaternion.Euler(0, 0, -90);
            boss1.SetActive(true);
        }
    }

    public void Pause()
    {
        if (UIController.Instance.pausePanel.activeSelf == false){
            UIController.Instance.pausePanel.SetActive(true);
            Time.timeScale = 0f;
            AudioManager.Instance.PlaySound(AudioManager.Instance.pause);
        }
        else
        {
            UIController.Instance.pausePanel.SetActive(false);
            Time.timeScale = 1f;
            PlayerController.Instance.ExitBoost();
            AudioManager.Instance.PlaySound(AudioManager.Instance.unpause);
        }
    }

    public void Quitgame()
    {
        Application.Quit();
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GameOver()
    {
        StartCoroutine(ShowGameOverScreen());
    }

    IEnumerator ShowGameOverScreen()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("GameOver");
    }

    public void SetWorldSpeed(float speed){
        worldSpeed = speed;
    }
}