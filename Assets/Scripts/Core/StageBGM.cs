using UnityEngine;

public class StageBGM : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [Header("BGM")]
    [SerializeField] private AudioClip normalBGM;
    [SerializeField] private AudioClip bossBGM;


    private void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        PlayNormalBGM();
    }

    public void PlayNormalBGM()
    {
        PlayBGM(normalBGM);
    }
    public void PlayBossBGM()
    {
        PlayBGM(bossBGM);
    }
    private void PlayBGM(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        if (audioSource.clip == clip && audioSource.isPlaying) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
