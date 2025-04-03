using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance;

    public AudioClip bgm;

    public AudioClip jumpBgm;
    public AudioClip moneyBgm;
    public AudioClip jailBgm;
    public AudioClip movedirectlyBgm;
    public AudioClip cardBgm;
    private AudioSource audioSource;
    private AudioSource jumpAudioSource;

    private AudioSource moneyAudioSource;
    private AudioSource jailAudioSource;
    private AudioSource movedirectlyAudioSource;
    private AudioSource cardAudioSource;

    private AudioSource shipAudioSource;
    public AudioClip shipBgm;


    void Awake()
    {
        if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
        return;
    }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = bgm;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        setThemeVolume(0.5f);
        audioSource.Play();


        jumpAudioSource = gameObject.AddComponent<AudioSource>();
        jumpAudioSource.clip = jumpBgm;
        jumpAudioSource.volume *= 0.25f;
        jumpAudioSource.playOnAwake = false;

        moneyAudioSource = gameObject.AddComponent<AudioSource>();
        moneyAudioSource.clip = moneyBgm;
        moneyAudioSource.playOnAwake = false;

        jailAudioSource = gameObject.AddComponent<AudioSource>();
        jailAudioSource.clip = jailBgm;
        jailAudioSource.playOnAwake = false;
        
        
        movedirectlyAudioSource = gameObject.AddComponent<AudioSource>();
        movedirectlyAudioSource.clip = movedirectlyBgm;
        movedirectlyAudioSource.playOnAwake = false;


        cardAudioSource = gameObject.AddComponent<AudioSource>();
        cardAudioSource.clip = cardBgm;
        cardAudioSource.playOnAwake = false;

        shipAudioSource = gameObject.AddComponent<AudioSource>();
        shipAudioSource.clip = shipBgm;
        shipAudioSource.playOnAwake = false;
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
    public void PlayJumpSound()
{
    
    jumpAudioSource.PlayOneShot(jumpBgm);
}
public void PlayShipSound(){
    shipAudioSource.PlayOneShot(shipBgm);
}

public void PlayMoneySound()
{
    moneyAudioSource.PlayOneShot(moneyBgm);
}

public void PlayJailSound()
{
    jailAudioSource.PlayOneShot(jailBgm);
}

public void PlayMoveDirectlySound()
{
    movedirectlyAudioSource.PlayOneShot(movedirectlyBgm);
}
    public void PlayCardSound()
{
    jumpAudioSource.PlayOneShot(cardBgm);
}



}
