using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScene : MonoBehaviour
{
    [Header("Mainscreen")] public Image playerAvatar1, playerAvatar2;
    public Text playerName;
    public Toggle soundToggle;


    [Header("AvatarSetting")] [SerializeField]
    GameObject avatarSetting, languangeSetting;

    [SerializeField] Transform chooseAvatarPanel;
    public Toggle avatarOptionPrefab;
    public InputField playerNameInput;
    private List<Toggle> toggleList, toggleLanguage;


    void Start()
    {
        Time.timeScale = 1f;
        SetupUI();
        SetupLanguageSettings();
        if (GameManager.IsFirstOpen)
        {
            ShowProfileChooser();
        }
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
    }

    void UpdateUI()
    {
        playerAvatar1.sprite = Resources.Load<Sprite>("Avatar/" + GameManager.PlayerAvatarIndex);
        playerAvatar2.sprite = Resources.Load<Sprite>("Avatar/" + GameManager.PlayerAvatarIndex);
        playerName.text = GameManager.PlayerAvatarName;
        playerName.GetComponent<TextUI>().UpdateText();
    }

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
        GameManager.PlayButton();
    }

    public void OnComputerPlay()
    {
        GameManager.PlayButton();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}