using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance { get; private set; }

    private float elapsedTime;
    private bool isPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!isPaused)
        {
            elapsedTime += Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.T))
        {
            ResetGameTime();
        }
    }

    public float CurrentGameTime
    {
        get { return elapsedTime; }
    }

    public void PauseGameTime()
    {
        isPaused = true;
    }

    public void ResumeGameTime()
    {
        isPaused = false;
    }

    public void ResetGameTime()
    {
        elapsedTime = 0f;
    }
}
