using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager instance;

    public AudioSource BgmAudio;
    public AudioSource SoundAudio;
    public AudioSource WaetherAudio;

    public Dictionary<string, AudioClip> Sfxclip= new Dictionary<string, AudioClip>();


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        BgmAudio = transform.GetChild(0).GetComponent<AudioSource>();
        SoundAudio = transform.GetChild(1).GetComponent<AudioSource>();
        WaetherAudio = transform.GetChild(2).GetComponent<AudioSource>();

        Setting();
    }

    void Setting()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sfx/");

        foreach(AudioClip clip in clips)
        {
            Sfxclip.Add(clip.name, clip);
        }

        Debug.Log(clips.Length + "���� ���� Ŭ�� �ε��.");
    }

    public void BgmPlay(string name)
    {
        if (!Sfxclip.ContainsKey(name))
        {
            Debug.Log("�ش� ���� Ŭ���� �������� �ʽ��ϴ�.");
            return;
        }

        AudioClip clip = Sfxclip[name];

        if(clip == BgmAudio.clip)
        {
            Debug.Log("������ ���Դϴ�.");
            return;
        }

        BgmAudio.clip = clip;
        BgmAudio.loop = true;
        BgmAudio.Play();
    }

    public void SoundPlay(string name)
    {
        if (!Sfxclip.ContainsKey(name))
        {
            Debug.Log("�ش� ���� Ŭ���� �������� �ʽ��ϴ�.");
            return;
        }

        AudioClip clip = Sfxclip[name];

        if (clip == SoundAudio.clip)
        {
            Debug.Log("������ ���Դϴ�.");
            return;
        }

        SoundAudio.PlayOneShot(clip);
    }

    public void WeatherPlay(string name)
    {
        if (!Sfxclip.ContainsKey(name))
        {
            Debug.Log("�ش� ���� Ŭ���� �������� �ʽ��ϴ�.");
            return;
        }

        AudioClip clip = Sfxclip[name];

        if (clip == WaetherAudio.clip)
        {
            Debug.Log("������ ���Դϴ�.");
            return;
        }

        WaetherAudio.clip = clip;
        WaetherAudio.loop = true;
        WaetherAudio.Play();
    }
}
