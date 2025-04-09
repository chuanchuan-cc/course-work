using UnityEngine;
using System.Collections;


public class CGcontrol : MonoBehaviour
{
    public TileGenerator tileGenerator;
    public bool isCG=false;

    public GameObject behaviorCG;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileGenerator= GameObject.Find("Map").GetComponent<TileGenerator>();
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
    public void PlayCG(string cgName,int i){
     
        Vector3Int posInt = tileGenerator.BoardGetPosition(i);
        Vector3 pos = new Vector3(posInt.x, posInt.y, posInt.z);
        
        PlayCGAnimation(cgName,pos);
        

        
    }

    public void PlayCGAnimation(string cgName,Vector3 pos)
    {
        isCG=true;
        StartCoroutine(PlayAnimation(cgName,pos));
    }
    
    private IEnumerator PlayAnimation(string cgName,Vector3 pos)
    {


        Transform animTransform = behaviorCG.transform.Find(cgName);
        if (animTransform == null)
        {
            Debug.LogWarning("Can't find the animation object: " + cgName);
            yield break;
        }

        GameObject animObject = animTransform.gameObject;
        if(pos!=new Vector3(999999f,999999f,999999f)){
        animObject.transform.position=new Vector3(pos.x+0.5f, pos.y+1.5f, pos.z);
        animObject.transform.eulerAngles = new Vector3(-45f, 0f, 0f);

        }

        
        
     
        animObject.SetActive(true);

        Animator animator = animObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(cgName);
  
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length+0.2f);
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






