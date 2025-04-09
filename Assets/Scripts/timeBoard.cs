using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class timeBoard : MonoBehaviour
{
    public float maxRuntime;
    public float runtime;
    public TextMeshProUGUI timetext;
    
    public void setMaxtime(float mxt){
        maxRuntime=mxt;
    }
    public void updateTimeBoard(float runtime){
        int minutes;
        int seconds;
        if(maxRuntime!=0){
        float remain=Mathf.Max(0, maxRuntime-runtime);
        minutes=Mathf.FloorToInt(remain/60);
        seconds=Mathf.FloorToInt(remain%60);
        if(remain==0){
            timetext.text="Time is Over";

        }
        else
        timetext.text=$"Time Left: {minutes:D2}:{seconds:D2}";
        }
        if(maxRuntime==0){
        minutes=Mathf.FloorToInt(runtime/60);
        seconds=Mathf.FloorToInt(runtime%60);
        timetext.text=$"Time Past: {minutes:D2}:{seconds:D2}";
        }

    }
   

    
}
