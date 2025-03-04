using UnityEngine;
using System.Collections;

public class CGcontrol : MonoBehaviour
{

    public GameObject behaviorCG;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        behaviorCG=GameObject.Find("BehaviorCG");
        foreach (Transform child in behaviorCG.transform)
            {
                child.gameObject.SetActive(false);
            }
    }
      public void CGDisplay(string name)
    {
    foreach (Transform child in behaviorCG.transform)
        {
            if (child.gameObject.name == name)
            {
                StartCoroutine(Display(child.gameObject));
                break;
            }
        }}
    

    private IEnumerator Display(GameObject cgObject)
    {
        cgObject.SetActive(true);
        float timer=0f;
        while (timer < 5f)
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Debug.Log("点击屏幕，立即关闭卡片 UI");
            break;
        }
        timer += Time.deltaTime;
        yield return null;   
    }
         cgObject.SetActive(false);
        
    }
       
    }
    


   
   

