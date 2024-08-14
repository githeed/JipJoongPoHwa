using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // 그렇지 않다면
        else
        {
            Destroy(gameObject);
        }
    }

    // audiosource
    public AudioSource briarAudio;
    public AudioSource zayahAudio;
    public AudioSource enemyAudio;
    public AudioSource bgmAudio;

    // effect audio clip 을 여러개 담아 놓을 변수
    // 0 : 총알발사
    // 1 : 터지는 소리

    public AudioClip[] briarAudios;
    public AudioClip[] zayahAudios;
    public AudioClip[] enemyAudios;
    public AudioClip bgmAuds;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlayBriarSound(int idx)
    {
        briarAudio.PlayOneShot(briarAudios[idx]);
    }

    public void PlayZayahSound(int idx)
    {
        zayahAudio.PlayOneShot(zayahAudios[idx]);
    }

    public void PlayEnemySound(int idx)
    {
        enemyAudio.PlayOneShot(enemyAudios[idx]);
    }


    // bgm Sound Play
    public void PlayBgmSound()
    {
        // 플레이 할 AudioClip 을 설정
        bgmAudio.clip = bgmAuds;
        // 플레이!
        bgmAudio.Play();
    }

    //public void AudioSourceEtc()
    //{
    //    // 일시정지
    //    bgmAudio.Pause();
    //    // 완전 멈춤
    //    bgmAudio.Stop();
    //    // 현재 실행되고있는 시간
    //    float currTime = bgmAudio.time;

    //    bgmAudio.time += 10;
    //}
}
