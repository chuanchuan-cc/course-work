using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class broadcast : MonoBehaviour
{
    public Image backgroundImage;
    public TextMeshProUGUI Text;
    public Player player;
    public bool isBroadcasting;

    void Start()
    {
        isBroadcasting = true;

        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>(); 
        }

        
        SetAlpha(0); 
        gameObject.SetActive(false);
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
