using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


[System.Serializable]
public class TimeSetting
{
    public float temperature, gain;
}
public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    public float CurrentTime = 6f;     // float��
    public float timeSpeed = 0.2f;     // 1�ʿ� 0.2�ð�

    public Transform Sun;
    public PostProcessVolume volume;
    private ColorGrading colorGrading;

    public List<TimeSetting> settings = new List<TimeSetting>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (volume != null && volume.profile != null)
            volume.profile.TryGetSettings(out colorGrading);
    }

    private void Update()
    {
        // �ð� ����
        CurrentTime += timeSpeed * Time.deltaTime;

        if (CurrentTime >= 24f)
        {
            CurrentTime -= 24f;
            GameManager.Instance.Day++;
            WeatherManager.instance.SetWeather();
            GameManager.Instance.SetCropPrice();
        }

        UpdateWorldTime();
    }

    void UpdateWorldTime()
    {
        float dayPercent = CurrentTime / 24f;
        //Sun.rotation = Quaternion.Euler(Sun.rotation.eulerAngles.x, dayPercent * 360f, Sun.rotation.eulerAngles.z);


        float segmentLength = 24f / (settings.Count - 1);
        int cur = Mathf.FloorToInt(CurrentTime / segmentLength);
        int next = Mathf.Min(cur + 1, settings.Count - 1);
        float t = (CurrentTime - cur * segmentLength) / segmentLength;

        TimeSetting a = settings[cur], b = settings[next];
        colorGrading.temperature.value = Mathf.Lerp(a.temperature, b.temperature, t);
        colorGrading.postExposure.value = Mathf.Lerp(a.gain, b.gain, t);
    }
}
