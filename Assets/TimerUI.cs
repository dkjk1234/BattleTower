// TimerUI.cs
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimerUI : MonoBehaviour
{
    [SerializeField]
    private Text timerText; // �ð��� ǥ���� UI Text ������Ʈ

    private float elapsedTime = 0f;
    private int displayedSeconds = 0;

    private bool timerIsRunning = false;

    void Start()
    {
        // Ÿ�̸� ����
        timerIsRunning = true;
        UpdateTimerUI(elapsedTime); // �ʱ� �ð� ǥ��
    }

    void Update()
    {
        if (timerIsRunning)
        {
            elapsedTime += Time.deltaTime;

            // 1�� ������ ������Ʈ
            int currentSeconds = Mathf.FloorToInt(elapsedTime);
            if (currentSeconds != displayedSeconds)
            {
                displayedSeconds = currentSeconds;
                UpdateTimerUI(elapsedTime);
            }
        }
    }

    void UpdateTimerUI(float currentTime)
    {
        // �ð��� ��:��:�� �������� ��ȯ
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
        string timeText = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        // UI Text ������Ʈ�� �ð� ������Ʈ
        if (timerText != null)
        {
            timerText.text = timeText;
        }
    }

    // Ÿ�̸� �Ͻ� ����
    public void PauseTimer()
    {
        timerIsRunning = false;
    }

    // Ÿ�̸� �簳
    public void ResumeTimer()
    {
        timerIsRunning = true;
    }

    // Ÿ�̸� �ʱ�ȭ
    public void ResetTimer()
    {
        elapsedTime = 0f;
        displayedSeconds = 0;
        UpdateTimerUI(elapsedTime);
    }
}
