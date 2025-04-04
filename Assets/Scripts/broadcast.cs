using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class broadcast : MonoBehaviour
{
    public Image backgroundImage;
    public TextMeshProUGUI Text;
    public Player player;
    public bool isBroadcasting;
    
    public TextMeshProUGUI wintext;
    public GameObject winb;
    public Button goBack;

    void Start()
    {
        isBroadcasting = true;

        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>(); 
        }

        
        SetAlpha(0); 
        gameObject.SetActive(false);
        goBack.onClick.AddListener(goback);
        winb.SetActive(false);
    }
    private void goback(){
        SceneManager.LoadScene("StartScene");
    }
    public void win(string n){
        winb.SetActive(true);

        wintext.text=$"{n} is the winner!";


    }

    public void showBroad(Player player)
    {
       
        Text.text=player.name+"'s turn!";
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
        isBroadcasting = true;
    }

    public void closeBroad(Player player)
    {
        

        StartCoroutine(FadeOut());
        isBroadcasting = false;
    }

    private IEnumerator FadeIn()
    {
       
        float totalTime = 0.25f;
        float time = 0f;

        while (time < totalTime)
        {
            float alpha = Mathf.Lerp(0, 1, time / totalTime);
            SetAlpha(alpha);
            time += Time.deltaTime;
            yield return null;
        }
        SetAlpha(1);
    }

    private IEnumerator FadeOut()
    {
       
        float totalTime = 0.25f;
        float time = 0f;

        while (time < totalTime)
        {
            float alpha = Mathf.Lerp(1, 0, time / totalTime);
            SetAlpha(alpha);
            time += Time.deltaTime;
            yield return null;
        }
        SetAlpha(0);
        gameObject.SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        Color tcolor = Text.color;
        tcolor.a=alpha;
        Text.color=tcolor;
        Color color = backgroundImage.color;
        color.a = alpha;
        backgroundImage.color = color;
    }
}
