// TimerUI.cs
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimerUI : MonoBehaviour
{
    [SerializeField]
    private Text timerText; // 시간을 표시할 UI Text 컴포넌트

    private float elapsedTime = 0f;
    private int displayedSeconds = 0;

    private bool timerIsRunning = false;

    void Start()
    {
        // 타이머 시작
        timerIsRunning = true;
        UpdateTimerUI(elapsedTime); // 초기 시간 표시
    }

    void Update()
    {
        if (timerIsRunning)
        {
            elapsedTime += Time.deltaTime;

            // 1초 단위로 업데이트
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
        // 시간을 시:분:초 형식으로 변환
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
        string timeText = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        // UI Text 컴포넌트에 시간 업데이트
        if (timerText != null)
        {
            timerText.text = timeText;
        }
    }

    // 타이머 일시 정지
    public void PauseTimer()
    {
        timerIsRunning = false;
    }

    // 타이머 재개
    public void ResumeTimer()
    {
        timerIsRunning = true;
    }

    // 타이머 초기화
    public void ResetTimer()
    {
        elapsedTime = 0f;
        displayedSeconds = 0;
        UpdateTimerUI(elapsedTime);
    }
}
