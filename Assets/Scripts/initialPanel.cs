using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class initialPanelScript : MonoBehaviour
{
    public static int playerNumberToGame;
    public static bool isAIToGame;
    public Button initialStart;
    public Button startGame;
    public Button loadMap;
    public Button exitpanel;
    public GameObject initialPanel;
    public AnimationCurve Curve;
    public float animationSpeed;
    public TextMeshProUGUI mapText;
    public Button loadLuckyCard;
    public Button loadOpportunityCard;
    public TextMeshProUGUI luckyCardText;
    public TextMeshProUGUI opportunityCardText;
    public Slider playerNumberSlider;
    public TextMeshProUGUI playerNumber;
    public Toggle AIToggle;
    public Image toggleImage;
    public Sprite toggleTrueImage;
    public Sprite toggleFalseImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
        initialPanel.transform.localScale = Vector3.zero;
        initialStart.onClick.AddListener(() => onClickStart(initialPanel));
        playerNumberSlider.onValueChanged.AddListener(UpdatePlayerNumberText);
        UpdatePlayerNumberText(playerNumberSlider.value);
        mapText.text="Default map";
        luckyCardText.text="Default lucky cardpool";
        opportunityCardText.text="Default opportunity cardpool";
        playerNumberSlider.value=1;
        playerNumber.text = playerNumberSlider.value.ToString();
        AIToggle.onValueChanged.AddListener(toggleBackground);
        toggleBackground(AIToggle.isOn);
        startGame.onClick.AddListener(startNewGame);
        loadMap.onClick.AddListener(LoadMap);
        loadLuckyCard.onClick.AddListener(LoadLuckyCard);
        loadOpportunityCard.onClick.AddListener(LoadOpportunityCard);
        exitpanel.onClick.AddListener(() => onClickExit(initialPanel));

    }
    // Update is called once per frame
   
   
    void startNewGame(){
    PlayerPrefs.SetInt("PlayerNumber", (int)playerNumberSlider.value);
    PlayerPrefs.SetInt("IsAI", AIToggle.isOn ? 1 : 0);
        SceneManager.LoadScene("gameScene");
    }
    void LoadMap(){
    // 调用陈良源的地图载入方法，并在文本框打印地图名
    mapText.text="";
    }
    void LoadLuckyCard(){
    // 调用陈子峰的lucky卡组载入方法，并在文本框打印卡组名
    luckyCardText.text="";
    }
    void LoadOpportunityCard(){
    // 调用陈子峰的opportunity卡组载入方法，并在文本框打印卡组名
    opportunityCardText.text="";
    }
    void toggleBackground(bool i){
        toggleImage.sprite= i? toggleTrueImage:toggleFalseImage;
    }
    void UpdatePlayerNumberText(float value)
{
    playerNumber.text = ((int)value).ToString();
}
  
   
    public void onClickExit(GameObject gameObject){
       StopAllCoroutines();
        StartCoroutine(hidePanel(gameObject));
        //请陈良源和陈子峰在此处添加清空已加载的地图和卡组的方法
    }
        IEnumerator hidePanel(GameObject gameObject){
          float timer = 0;
        while(timer<=1){
            gameObject.transform.localScale=Vector3.one*(1-Curve.Evaluate(timer));
            timer+=Time.deltaTime*animationSpeed;
           
            yield return null;
        }
        gameObject.transform.localScale = Vector3.zero;
       
    }
     public void onClickStart(GameObject gameObject)
    {
        StopAllCoroutines();
        StartCoroutine(showPanel(gameObject));
    }
   
    IEnumerator showPanel(GameObject gameObject){
        float timer = 0;
        while(timer<=1){

            gameObject.transform.localScale=Vector3.one*Curve.Evaluate(timer);
            timer+=Time.deltaTime*animationSpeed;
            
            yield return null;
        }
        gameObject.transform.localScale = Vector3.one;
        
}




}
