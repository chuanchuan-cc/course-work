using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using SFB;
using System.IO;

public class initialPanelScript : MonoBehaviour
{

    public Button initialStart;
    public Button startGame;
    public Button loadMap;
    public Button exitpanel;
    public GameObject initialPanel;
    public AnimationCurve Curve;
    public float animationSpeed;
    public TextMeshProUGUI mapText;
    public Button loadCard;
    public TextMeshProUGUI cardText;
    public Slider playerNumberSlider;
    public TextMeshProUGUI playerNumber;
    public Toggle AIToggle;
    public Image toggleImage;
    public Sprite toggleTrueImage;
    public Sprite toggleFalseImage;
    public string defaultFolder;
    public Slider difficultySlider;
    public GameObject difficultyOutput;
    public TextMeshProUGUI difficultyText;
    public Button loadgame;

    public Button musicButton;
    public MusicController musicController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        initialPanel.transform.localScale = Vector3.zero;
        initialStart.onClick.AddListener(() => onClickStart(initialPanel));
        playerNumberSlider.onValueChanged.AddListener(UpdatePlayerNumberText);
        UpdatePlayerNumberText(playerNumberSlider.value);
        mapText.text="Default map";
        cardText.text="Default cardpool";
        playerNumberSlider.value=1;
        playerNumber.text = playerNumberSlider.value.ToString();
        difficultySlider.gameObject.SetActive(false);
        difficultyOutput.gameObject.SetActive(false);
        AIToggle.onValueChanged.AddListener(toggleClick);
        toggleClick(AIToggle.isOn);
        startGame.onClick.AddListener(startNewGame);
        loadMap.onClick.AddListener(LoadMap);
        loadCard.onClick.AddListener(LoadCard);
        exitpanel.onClick.AddListener(() => onClickExit(initialPanel));
        defaultFolder = Application.dataPath + "/Resources";
        loadgame.onClick.AddListener(loadGame);
        musicButton.onClick.AddListener(musicController.changeThemeMode);

    }
    // Update is called once per frame



   
   
    void startNewGame(){
    PlayerPrefs.SetInt("PlayerNumber", (int)playerNumberSlider.value);
    PlayerPrefs.SetInt("IsAI", AIToggle.isOn ? 1 : 0);
    PlayerPrefs.SetInt("difficulty", (int)difficultySlider.value);
        SceneManager.LoadScene("gameScene");
    }
    
    void toggleClick(bool i){
        toggleImage.sprite= i? toggleTrueImage:toggleFalseImage;
        if(i){
                       difficultySlider.gameObject.SetActive(true);
            difficultyOutput.gameObject.SetActive(true);
           difficultySlider.onValueChanged.AddListener(UpdateDifficultyText);
           UpdatePlayerNumberText(difficultySlider.value);
           difficultyText.text = "easy";
          

        }else{
            difficultySlider.gameObject.SetActive(false);
            difficultyOutput.gameObject.SetActive(false);

        
        }
        
    }
    void UpdatePlayerNumberText(float value)
{
    playerNumber.text = ((int)value).ToString();
}
  
      void UpdateDifficultyText(float value)
{
    switch(value){
    case 1:
        difficultyText.text = "easy";
        break;
    case 2:
        difficultyText.text = "normal";
        break;
    case 3:
        difficultyText.text = "difficult";
        break;

}
}
   
    public void onClickExit(GameObject gameObject){
       StopAllCoroutines();
        StartCoroutine(hidePanel(gameObject));
        
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
 public void LoadMap()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("choose your map(xlsx-formatted file)", defaultFolder+"/map", "xlsx", false);
        if (paths.Length > 0)
        {
            string mapPath=paths[0];
            PlayerPrefs.SetString("mapPath", mapPath);
            PlayerPrefs.Save();
            mapText.text=Path.GetFileName(mapPath);
        }
    }
     public void LoadCard()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("choose your card(xlsx-formatted file)", defaultFolder+"/card", "xlsx", false);
        if (paths.Length > 0)
        {
            string cardPath=paths[0];
            PlayerPrefs.SetString("cardPath", cardPath);
            PlayerPrefs.Save();
            cardText.text=Path.GetFileName(cardPath);
        }
    }
    public void loadGame(){
         var paths = StandaloneFileBrowser.OpenFilePanel("choose your save document", defaultFolder+"/save", "json", false);
        if (paths.Length > 0)
        {
            string savePath=paths[0];
            PlayerPrefs.SetInt("isLoadGame", 1);
            PlayerPrefs.SetString("savePath", savePath);
            PlayerPrefs.Save();
            startNewGame();
        }

    }




}
