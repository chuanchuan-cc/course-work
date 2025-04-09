using UnityEngine;
using System.Collections;


public class CGcontrol : MonoBehaviour
{
    public bool isCG=false;

    public GameObject behaviorCG;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        behaviorCG = GameObject.Find("BehaviorCG");
        foreach (Transform child in behaviorCG.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    // public void CGDisplay(string name)
    // {
    //     foreach (Transform child in behaviorCG.transform)
    //     {
    //         if (child.gameObject.name == name)
    //         {
    //             StartCoroutine(Display(child.gameObject));
    //             break;
    //         }
    //     }
    // }


    // private IEnumerator Display(GameObject cgObject)
    // {
    //     cgObject.SetActive(true);
    //     float timer = 0f;
    //     while (timer < 5f)
    //     {
    //         if (Input.GetMouseButtonDown(0))
    //         {
    //             Debug.Log("click, close the display");
    //             break;
    //         }
    //         timer += Time.deltaTime;
    //         yield return null;
    //     }
    //     cgObject.SetActive(false);

    // }
 

    public void PlayCGAnimation(string cgName)
    {
        isCG=true;
        StartCoroutine(PlayAnimation(cgName));
    }
    
    private IEnumerator PlayAnimation(string cgName)
    {

        Transform animTransform = behaviorCG.transform.Find(cgName);
        if (animTransform == null)
        {
            Debug.LogWarning("Can't find the animation object: " + cgName);
            yield break;
        }

        GameObject animObject = animTransform.gameObject;
        
        
        animObject.transform.position = new UnityEngine.Vector3(0, 0, 0);
        animObject.transform.rotation = Quaternion.Euler(-45f, 0f, 0f);
        animObject.SetActive(true);

        Animator animator = animObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(cgName);
  
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            Debug.LogWarning("No Animator component found on: " + cgName);
            yield return new WaitForSeconds(5f);
        }
        isCG=false;
        
        animObject.SetActive(false);
    }

    }






