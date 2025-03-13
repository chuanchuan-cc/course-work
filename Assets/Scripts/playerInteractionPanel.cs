using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class playerInteractionPanel : MonoBehaviour
{
    public GameObject panel;
    public Text title;
    public Button yesButton;
    public Button noButton;
    public bool result;
    public bool isResult=false;
    public Image panelImage;


void start(){
    panelImage = panel.GetComponent<Image>(); 
    closePanel();

}
void closePanel(){
    //SetAlpha(0);
    panel.gameObject.SetActive(false);
}
public bool showPanel(string i){
    title.text=i;
    yesButton.onClick.AddListener(setYes);
    noButton.onClick.AddListener(setNo);
    StartCoroutine(panelDisplay());
    return result;
}
public IEnumerator panelDisplay(){
    //StartCoroutine(FadeIn());
    panel.gameObject.SetActive(true);
    yield return new WaitUntil(()=>isResult);
    //StartCoroutine(FadeOut());
    panel.gameObject.SetActive(false);


    



}
void setYes(){
    result=true;
    isResult=true;
}
void setNo(){
    result=false;
    isResult=true;

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
        panel.gameObject.SetActive(true);
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
        panel.gameObject.SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        Color tcolor = title.color;
        tcolor.a=alpha;
        title.color=tcolor;
        Color color = panelImage.color;
        color.a = alpha;
        panelImage.color = color;
    }
}
