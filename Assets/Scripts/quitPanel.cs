using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class quitPanelScript : MonoBehaviour
{
    public initialPanelScript initialPanel;
    
    public Button quitButton;
    public Button yesButton;
    public Button noButton;
    public GameObject quitPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        quitPanel.transform.localScale = Vector3.zero;
        quitButton.onClick.AddListener(()=> initialPanel.onClickStart(quitPanel));
        noButton.onClick.AddListener(()=> initialPanel.onClickExit(quitPanel));
        yesButton.onClick.AddListener(quitGame);
    }
 void quitGame(){
     #if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying=false;
     #else
        Application.quit();
     #endif
    }
   

}
