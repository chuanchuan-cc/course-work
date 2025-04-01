using UnityEngine;

public class MusicController : MonoBehaviour
{

    public AudioClip bgm;

    public AudioClip jumpBgm;
    public AudioClip moneyBgm;
    public AudioClip jailBgm;
    private AudioSource audioSource;
    private AudioSource jumpAudioSource;

    private AudioSource moneyAudioSource;
    private AudioSource jailAudioSource;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = bgm;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.Play();

        jumpAudioSource = gameObject.AddComponent<AudioSource>();
        jumpAudioSource.clip = jumpBgm;
        jumpAudioSource.playOnAwake = false;

        moneyAudioSource = gameObject.AddComponent<AudioSource>();
        moneyAudioSource.clip = moneyBgm;
        moneyAudioSource.playOnAwake = false;

        jailAudioSource = gameObject.AddComponent<AudioSource>();
        jailAudioSource.clip = jailBgm;
        jailAudioSource.playOnAwake = false;
    }
    public void changeThemeMode(){
        if(audioSource.isPlaying)
            audioSource.Pause();

        else
        audioSource.UnPause();

    }
    public void setThemeVolume(float f){
        if(f==0f)
        audioSource.UnPause();
        else{
            if(!audioSource.isPlaying)
            audioSource.UnPause();
            audioSource.volume=f;
            

        }

        
    }

}
