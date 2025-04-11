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

    public Slider RuntimeSlider;
    public Toggle gameModeToggle;
    public TextMeshProUGUI RuntimeText;
    public Image gameModeToggleImg;
    public GameObject RuntimeOutput;
    public Slider AInumber;
    public TextMeshProUGUI AINumberText;
    private const int MaxTotalPlayers = 6;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        initialPanel.transform.localScale = Vector3.zero;
        initialStart.onClick.AddListener(() => onClickStart(initialPanel));
        playerNumberSlider.onValueChanged.AddListener(UpdatePlayerNumberText);
        UpdatePlayerNumberText(playerNumberSlider.value);
        mapText.text = "Default map";
        cardText.text = "Default cardpool";
        playerNumberSlider.value = 3;
        playerNumber.text = playerNumberSlider.value.ToString();
        AInumber.value=1;
        AInumber.onValueChanged.AddListener(UpdateAINumber);
        UpdateAINumber(AInumber.value);
        difficultySlider.onValueChanged.AddListener(UpdateDifficultyText);
        UpdateDifficultyText(difficultySlider.value);
        RuntimeSlider.gameObject.SetActive(false);
        RuntimeOutput.gameObject.SetActive(false);
        gameModeToggle.onValueChanged.AddListener(gameModeChange);
        gameModeChange(gameModeToggle.isOn);
        startGame.onClick.AddListener(startNewGame);
        loadMap.onClick.AddListener(LoadMap);
        loadCard.onClick.AddListener(LoadCard);
        exitpanel.onClick.AddListener(() => onClickExit(initialPanel));
        defaultFolder = Application.persistentDataPath;;
        loadgame.onClick.AddListener(loadGame);
        musicButton.onClick.AddListener(MusicController.Instance.changeThemeMode);

        string defaultMapPath = Path.Combine(Application.persistentDataPath, "map/testMap.xlsx");
        string defaultCardPath = Path.Combine(Application.persistentDataPath, "card/testCard.xlsx");
         if (!PlayerPrefs.HasKey("mapPath") && File.Exists(defaultMapPath))
        {
            PlayerPrefs.SetString("mapPath", defaultMapPath);
            mapText.text = "Default map";
        }

        if (!PlayerPrefs.HasKey("cardPath") && File.Exists(defaultCardPath))
        {
            PlayerPrefs.SetString("cardPath", defaultCardPath);
            cardText.text = "Default cardpool";
        }

        PlayerPrefs.Save();

    }
    // Update is called once per frame

    void UpdateAINumber(float value){
        AINumberText.text=$"{value}";
        if (playerNumberSlider.value+AInumber.value>MaxTotalPlayers)
    {
        playerNumberSlider.value=MaxTotalPlayers-AInumber.value;
        UpdatePlayerNumberText(playerNumberSlider.value);
    }
        
    }



    void startNewGame()
    {
        PlayerPrefs.SetInt("PlayerNumber", (int)playerNumberSlider.value);
        PlayerPrefs.SetInt("AInumber", (int)AInumber.value);
        PlayerPrefs.SetInt("difficulty", (int)difficultySlider.value);
        PlayerPrefs.SetInt("maxRuntime", gameModeToggle.isOn ? (int)RuntimeSlider.value : 0);
        SceneManager.LoadScene("gameScene");

    }


    void gameModeChange(bool i)
    {
        gameModeToggleImg.sprite = i ? toggleTrueImage : toggleFalseImage;
        if (i)
        {
            RuntimeSlider.gameObject.SetActive(true);
            RuntimeOutput.gameObject.SetActive(true);
            RuntimeSlider.onValueChanged.AddListener(UpdateRuntime);
            UpdateRuntime(RuntimeSlider.value);





        }
        else
        {
            RuntimeSlider.gameObject.SetActive(false);
            RuntimeOutput.gameObject.SetActive(false);


        }

    }

    void UpdateRuntime(float value)
    {
        RuntimeText.text = $"{value} minutes";
    }
    void UpdatePlayerNumberText(float value)
    {
        playerNumber.text = ((int)value).ToString();
         if (playerNumberSlider.value+AInumber.value>MaxTotalPlayers)
    {
        AInumber.value=MaxTotalPlayers-playerNumberSlider.value;
        UpdateAINumber(AInumber.value);
    }
    }

    void UpdateDifficultyText(float value)
    {
        switch (value)
        {
            case 0:
                difficultyText.text = "easy";
                break;
            case 1:
                difficultyText.text = "normal";
                break;
            case 2:
                difficultyText.text = "difficult";
                break;

        }
    }

    public void onClickExit(GameObject gameObject)
    {
        StopAllCoroutines();
        StartCoroutine(hidePanel(gameObject));

    }
    IEnumerator hidePanel(GameObject gameObject)
    {
        float timer = 0;
        while (timer <= 1)
        {
            gameObject.transform.localScale = Vector3.one * (1 - Curve.Evaluate(timer));
            timer += Time.deltaTime * animationSpeed;

            yield return null;
        }
        gameObject.transform.localScale = Vector3.zero;

    }
    public void onClickStart(GameObject gameObject)
    {
        StopAllCoroutines();
        StartCoroutine(showPanel(gameObject));
    }

    IEnumerator showPanel(GameObject gameObject)
    {
        float timer = 0;
        while (timer <= 1)
        {

            gameObject.transform.localScale = Vector3.one * Curve.Evaluate(timer);
            timer += Time.deltaTime * animationSpeed;

            yield return null;
        }
        gameObject.transform.localScale = Vector3.one;


    }
    public void LoadMap()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("choose your map(xlsx-formatted file)", defaultFolder + "/map", "xlsx", false);
        if (paths.Length > 0)
        {
            string mapPath = paths[0];
            PlayerPrefs.SetString("mapPath", mapPath);
            PlayerPrefs.Save();
            mapText.text = Path.GetFileName(mapPath);
        }
    }
    public void LoadCard()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("choose your card(xlsx-formatted file)", defaultFolder + "/card", "xlsx", false);
        if (paths.Length > 0)
        {
            string cardPath = paths[0];
            PlayerPrefs.SetString("cardPath", cardPath);
            PlayerPrefs.Save();
            cardText.text = Path.GetFileName(cardPath);
        }
    }
    public void loadGame()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("choose your save document", defaultFolder + "/save", "json", false);
        if (paths.Length > 0)
        {
            string savePath = paths[0];
            PlayerPrefs.SetInt("isLoadGame", 1);
            PlayerPrefs.SetString("savePath", savePath);
            PlayerPrefs.Save();
            startNewGame();
        }

    }




}
