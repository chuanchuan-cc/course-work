using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class playerInteractionPanel : MonoBehaviour
{
    public GameObject panel;
    public CanvasGroup canvasGroup; 
    public TextMeshProUGUI title;
    public Button yesButton;
    public Button noButton;
    private System.Action<bool> callback; 
    public bool isResult = false;

    void Start()
    {
        if (canvasGroup == null)
        {
            canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = panel.AddComponent<CanvasGroup>(); 
            }
        }

        ClosePanel();
    }

    void ClosePanel()
    {
        panel.SetActive(false);
    }

    public void ShowPanel(string message, System.Action<bool> callback)
    {
        panel.SetActive(true);
        isResult = false;
        title.text = message;
        this.callback = callback;
        

        yesButton.onClick.AddListener(() => { SetResult(true); });
        noButton.onClick.AddListener(() => { SetResult(false); });

        StartCoroutine(PanelDisplay());
    }

    private IEnumerator PanelDisplay()
    {
        yield return StartCoroutine(FadeIn());
        yield return new WaitUntil(() => isResult);
        yield return StartCoroutine(FadeOut());
        panel.SetActive(false);
    }

    void SetResult(bool result)
    {
        isResult = true;
        callback?.Invoke(result);
    }

    private IEnumerator FadeIn()
    {
        float duration = 0.25f;
        float elapsedTime = 0f;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        float duration = 0.25f;
        float elapsedTime = 0f;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0;
    }
}
