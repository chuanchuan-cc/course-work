using UnityEngine;
using System.Collections;
using System.Numerics;

public class CGcontrol : MonoBehaviour
{

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
    public void CGDisplay(string name)
    {
        foreach (Transform child in behaviorCG.transform)
        {
            if (child.gameObject.name == name)
            {
                StartCoroutine(Display(child.gameObject));
                break;
            }
        }
    }


    private IEnumerator Display(GameObject cgObject)
    {
        cgObject.SetActive(true);
        float timer = 0f;
        while (timer < 5f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("click, close the display");
                break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        cgObject.SetActive(false);

    }
    public IEnumerator PlayCGAnimation(string cgName, Player player)
    {
        Transform cgTransform = behaviorCG.transform.Find(cgName);
        if (cgTransform == null)
        {
            Debug.LogWarning("can't find the cgObject:" + cgName);
            yield break;
        }
        GameObject cgObject = cgTransform.gameObject;
        UnityEngine.Vector2 worldPos = player.transform.position;
        UnityEngine.Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        cgObject.transform.position = screenPos;
        cgObject.SetActive(true);
        Animator animator = cgObject.GetComponent<Animator>();
        yield return new WaitUntil(() =>
        animator.GetAnimatorTransitionInfo(0).normalizedTime >= 1f && !animator.IsInTransition(0));
        cgObject.SetActive(false);

    }

}






