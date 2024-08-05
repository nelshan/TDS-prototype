using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TimerText;
    private float ElapsedTime;

    private void Update()
    {
        ElapsedTime += Time.deltaTime;
        int Minutes = Mathf.FloorToInt(ElapsedTime / 60);
        int Second = Mathf.FloorToInt(ElapsedTime % 60);
        TimerText.text = string.Format("{0:00}:{1:00}", Minutes, Second);
    }
}
