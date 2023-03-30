using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScene : MonoBehaviour
{
    [Header("Mainscreen")]
    public Image playerAvatar1;
    public Image playerAvatar2;
    [SerializeField] private Image languageFlag;
    public Text playerName;
    [SerializeField] private Text languageName;
    public Toggle soundToggle;
    [Header("AvatarSetting")]
    public GameObject avatarSetting;

    public GameObject languangeSetting;
    public Transform chooseAvatarPanel;
    [SerializeField] private Transform chooseLanguagePanel;
    public Toggle avatarOptionPrefab;
    [SerializeField] private Toggle languageOptionPrefab;
    public InputField playerNameInput;

    private List<Toggle> toggleList;
    private List<Toggle> toggleLanguage;
    

    void Start()
    {
        Time.timeScale = 1f;
        SetupUI();
        SetupLanguageSettings();
        if (GameManager.IsFirstOpen)
        {
            ShowProfileChooser();
        }

        CUtils.ShowInterstitialAd();
        CUtils.ShowBannerAd();
        Timer.Schedule(this, 0.1f, AddEvents);
    }

    void SetupUI()
    {
        soundToggle.isOn = GameManager.IsSound;
        
        soundToggle.onValueChanged.RemoveAllListeners();
        soundToggle.onValueChanged.AddListener((arg0) =>
        {
            GameManager.PlayButton();
            GameManager.IsSound = arg0;
            soundToggle.gameObject.SetActive(arg0);
            
        });

        toggleList = new List<Toggle>();
        for (int i = 0; i < GameManager.TOTAL_AVATAR; i++)
        {
            Toggle temp = Instantiate<Toggle>(avatarOptionPrefab, chooseAvatarPanel);
            temp.group = chooseAvatarPanel.GetComponent<ToggleGroup>();
            temp.GetComponentsInChildren<Image>()[2].sprite = Resources.Load<Sprite>("Avatar/" + i);
            int index = i;
            temp.onValueChanged.AddListener((arg0) =>
            {
                if (arg0)
                {
                    GameManager.PlayerAvatarIndex = index;
                    UpdateUI();
                }
            });
            toggleList.Add(temp);
        }
        UpdateUI();
    }
    void SetupLanguageSettings()
    {
        soundToggle.isOn = GameManager.IsSound;

        soundToggle.onValueChanged.RemoveAllListeners();
        soundToggle.onValueChanged.AddListener((arg0) =>
        {
            GameManager.PlayButton();
            GameManager.IsSound = arg0;
            
        });

        // toggleLanguage = new List<Toggle>();
        // for (int i = 0; i < GameManager.TOTAL_LANGUAGE; i++)
        // {
        //     Toggle temp = Instantiate<Toggle>(languageOptionPrefab, chooseLanguagePanel);
        //     temp.group = chooseLanguagePanel.GetComponent<ToggleGroup>();
        //     temp.GetComponentsInChildren<Image>()[2].sprite = Resources.Load<Sprite>("Flags/" + i);
        //     int index = i;
        //     temp.onValueChanged.AddListener((arg0) =>
        //     {
        //         if (arg0)
        //         {
        //             GameManager.LanguageIndex = index;
        //             UpdateLanguage();
        //         }
        //     });
        //     toggleLanguage.Add(temp);
        // }
        //UpdateLanguage();
    }

    void UpdateUI()
    {
        playerAvatar1.sprite = Resources.Load<Sprite>("Avatar/" + GameManager.PlayerAvatarIndex);
        playerAvatar2.sprite = Resources.Load<Sprite>("Avatar/" + GameManager.PlayerAvatarIndex);
        playerName.text = GameManager.PlayerAvatarName;
        playerName.GetComponent<EllipsisText>().UpdateText();

    }

    // void UpdateLanguage()
    // {
    //     languageFlag.sprite = Resources.Load<Sprite>("Flags/" + GameManager.LanguageIndex);
    //     languageName.text = GameManager.LanguageName;
    // }
    public void ShowProfileChooser()
    {
        avatarSetting.SetActive(true);
        playerNameInput.text = GameManager.PlayerAvatarName;
        toggleList[GameManager.PlayerAvatarIndex].isOn = true;
    }
    public void ShowLanguagesChooser()
    {
        languangeSetting.SetActive(true);
        playerNameInput.text = GameManager.PlayerAvatarName;
        toggleList[GameManager.PlayerAvatarIndex].isOn = true;
    }

    public void OnContine()
    {
        var inputName = playerNameInput.text.Trim();
        if (inputName.Length == 0)
        {
            Toast.instance.ShowMessage("You need to enter your name");
            return;
        }

        GameManager.IsFirstOpen = false;
        GameManager.PlayerAvatarName = inputName;
        avatarSetting.SetActive(false);
        UpdateUI();
        GameManager.PlayButton();
    }

    public void OnSettingsContinue()
    {
        GameManager.IsFirstOpen = false;
        languangeSetting.SetActive(false);
        //UpdateLanguage();
        GameManager.PlayButton();
    }

    public void OnComputerPlay()
    {
        GameManager.currentGameMode = GameMode.Computer;
        GameManager.PlayButton();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void OnMultiPlayerPlay()
    {
        if (IsAdAvailable())
        {
#if UNITY_EDITOR
            HandleRewardBasedVideoRewarded(null, null);
#else
            AdmobController.instance.ShowRewardBasedVideo();
#endif
        }
        else
        {
            EnterMultiplayer();
        }
        GameManager.PlayButton();
    }

    private void EnterMultiplayer()
    {
        GameManager.currentGameMode = GameMode.MultiPlayer;
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    private void AddEvents()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (AdmobController.instance.rewardBasedVideo != null)
        {
            AdmobController.instance.rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        }
#endif
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        EnterMultiplayer();
    }

    private bool IsAdAvailable()
    {
#if UNITY_ANDROID || UNITY_IOS
        return AdmobController.instance.rewardBasedVideo.IsLoaded();
#else
        return false;
#endif
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (AdmobController.instance.rewardBasedVideo != null)
        {
            AdmobController.instance.rewardBasedVideo.OnAdRewarded -= HandleRewardBasedVideoRewarded;
        }
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
