﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public string playerName;
    public bool isUserPlayer;
    public PlayerCards cardsPanel;
    public GameObject CardPanelBG;

    [Header("Avatar")] public Image avatarImage;
    [SerializeField] Text avatarName, messageLbl;
    public ParticleSystem starParticleSystem;
    [Header("Timer")] public Image timerImage;
    public GameObject timerOjbect;

    [Header("ParticleForTimer")] [SerializeField]
    private Text _scoreText;

    [SerializeField] private int leftCards = 0;
    [SerializeField] private ParticleSystem Fx;
    [SerializeField] private RectTransform fxTransfrom;
    [SerializeField] private Color[] particleColors;
    private Vector3 xpos;
    private float totalTimer = 15f;
    [HideInInspector] public bool pickFromDeck, unoClicked, choosingColor;
    [HideInInspector] public bool isInRoom = true;


    void Start()
    {
        Timer = false;
        timerImage.fillAmount = 1f;
        leftCards = 0;
        xpos = fxTransfrom.transform.position;
    }

    public void SetAvatarProfile(AvatarProfile p)
    {
        playerName = p.avatarName;
        if (avatarName != null)
        {
            avatarName.text = p.avatarName;
            avatarName.GetComponent<TextUI>().UpdateText();
        }

        if (avatarImage != null)
            avatarImage.sprite = Resources.Load<Sprite>("Avatar/" + p.avatarIndex);
    }

    public bool Timer
    {
        get { return timerOjbect.activeInHierarchy; }
        set
        {
            CancelInvoke("UpdateTimer");
            timerOjbect.SetActive(value);
            if (value)
            {
                timerImage.fillAmount = 1f;
                fxTransfrom.transform.position = xpos;
                InvokeRepeating("UpdateTimer", 0f, .1f);
            }
            else
            {
                timerImage.fillAmount = 0f;
            }
        }
    }

    [Obsolete("Obsolete")]
    void UpdateTimer()
    {
        timerImage.fillAmount -= 0.07f / totalTimer;
        fxTransfrom.Translate(-0.0080f, 0f, 0f);
        if (timerImage.fillAmount <= 0)
        {
            if (choosingColor)
            {
                if (isUserPlayer)
                {
                    GamePlayManager.instance.colorChoose.HidePopup();
                }

                ChooseBestColor();
            }
            else if (GamePlayManager.instance.IsDeckArrow)
            {
                GamePlayManager.instance.OnDeckClick();
            }
            else if (cardsPanel.AllowedCard.Count > 0)
            {
                OnCardClick(FindBestPutCard());
            }
            else
            {
                OnTurnEnd();
            }
        }
        else if (timerImage.fillAmount <= 1f && timerImage.fillAmount > 0.675f)
        {
            Fx.startColor = particleColors[0];
            timerImage.sprite = Resources.Load<Sprite>("TimerBars/" + 0);
            timerOjbect.transform.GetChild(0).gameObject.SetActive(true);
            timerOjbect.transform.GetChild(1).gameObject.SetActive(false);
            timerOjbect.transform.GetChild(2).gameObject.SetActive(false);

            timerImage.color = Color.green;
        }
        else if (timerImage.fillAmount <= 0.675 && timerImage.fillAmount > 0.345f)

        {
            Fx.startColor = particleColors[1];
            timerImage.sprite = Resources.Load<Sprite>("TimerBars/" + 1);
            timerImage.color = Color.yellow;
            timerOjbect.transform.GetChild(0).gameObject.SetActive(false);
            timerOjbect.transform.GetChild(1).gameObject.SetActive(true);
            timerOjbect.transform.GetChild(2).gameObject.SetActive(false);
        }
        else if (timerImage.fillAmount <= 0.345f && timerImage.fillAmount > 0)
        {
            Fx.startColor = particleColors[2];
            timerImage.sprite = Resources.Load<Sprite>("TimerBars/" + 2);
            timerImage.color = Color.red;
            timerOjbect.transform.GetChild(0).gameObject.SetActive(false);
            timerOjbect.transform.GetChild(1).gameObject.SetActive(false);
            timerOjbect.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void OnTurn()
    {
        unoClicked = false;
        pickFromDeck = false;
        Timer = true;

        if (isUserPlayer)
        {
            UpdateCardColor();
            if (cardsPanel.AllowedCard.Count == 0)
            {
                GamePlayManager.instance.EnableDeckClick();
            }
        }

        else
        {
            StartCoroutine(DoComputerTurn());
        }
    }

    public void UpdateCardColor()
    {
        if (isUserPlayer)
        {
            foreach (var item in cardsPanel.AllowedCard)
            {
                item.SetGaryColor(false);
                item.IsClickable = true;
            }

            foreach (var item in cardsPanel.DisallowedCard)
            {
                item.SetGaryColor(true);

                item.IsClickable = false;
            }

            if (cardsPanel.AllowedCard.Count > 0 && cardsPanel.cards.Count == 2)
            {
                GamePlayManager.instance.EnableUnoBtn();
            }
            else
            {
                GamePlayManager.instance.DisableUnoBtn();
            }
        }
    }

    public void AddCard(Card c)
    {
        cardsPanel.cards.Add(c);
        c.transform.SetParent(cardsPanel.transform);
        if (isUserPlayer)
        {
            c.onClick = OnCardClick;
            c.IsClickable = false;
        }

        leftCards++;

        _scoreText.text = leftCards.ToString();
    }

    public void RemoveCard(Card c)
    {
        cardsPanel.cards.Remove(c);
        c.onClick = null;
        c.IsClickable = false;
        leftCards--;
        _scoreText.text = leftCards.ToString();
    }

    public void OnCardClick(Card c)
    {
        if (Timer)
        {
            GamePlayManager.instance.PutCardToWastePile(c, this);
            OnTurnEnd();
        }
    }

    public void OnTurnEnd()
    {
        if (!choosingColor) Timer = false;
        cardsPanel.UpdatePos();
        foreach (var item in cardsPanel.cards)
        {
            item.SetGaryColor(false);
        }
    }

    public void ShowMessage(string message, bool playStarParticle = false)
    {
        messageLbl.text = message;
        messageLbl.GetComponent<Animator>().SetTrigger("show");
        if (playStarParticle)
        {
            starParticleSystem.gameObject.SetActive(true);
            starParticleSystem.Emit(30);
        }
    }

    public IEnumerator DoComputerTurn()
    {
        if (cardsPanel.AllowedCard.Count > 0)
        {
            StartCoroutine(ComputerTurnHasCard(0.25f));
        }
        else
        {
            yield return new WaitForSeconds(Random.Range(1f, totalTimer * .3f));
            GamePlayManager.instance.EnableDeckClick();
            GamePlayManager.instance.OnDeckClick();

            if (cardsPanel.AllowedCard.Count > 0)
            {
                StartCoroutine(ComputerTurnHasCard(0.2f));
            }
        }
    }

    private IEnumerator ComputerTurnHasCard(float unoCoef)
    {
        bool unoClick = false;
        float unoPossibality = GamePlayManager.instance.UnoProbability / 100f;

        if (Random.value < unoPossibality && cardsPanel.cards.Count == 2)
        {
            yield return new WaitForSeconds(Random.Range(1f, totalTimer * unoCoef));
            GamePlayManager.instance.OnUnoClick();
            unoClick = true;
        }

        yield return new WaitForSeconds(Random.Range(1f, totalTimer * (unoClick ? unoCoef : unoCoef * 2)));
        OnCardClick(FindBestPutCard());
    }

    public Card FindBestPutCard()
    {
        List<Card> allow = cardsPanel.AllowedCard;
        allow.Sort((x, y) => y.Type.CompareTo(x.Type));
        return allow[0];
    }

    public void ChooseBestColor()
    {
        CardType temp = CardType.Other;
        if (cardsPanel.cards.Count == 1)
        {
            temp = cardsPanel.cards[0].Type;
        }
        else
        {
            int max = 1;
            for (int i = 0; i < 5; i++)
            {
                if (cardsPanel.GetCount((CardType)i) > max)
                {
                    max = cardsPanel.GetCount((CardType)i);
                    temp = (CardType)i;
                }
            }
        }

        if (temp == CardType.Other)
        {
            GamePlayManager.instance.SelectColor(Random.Range(1, 5));
        }
        else
        {
            if (Random.value < 0.7f)
                GamePlayManager.instance.SelectColor((int)temp);
            else
                GamePlayManager.instance.SelectColor(Random.Range(1, 5));
        }
    }

    public int GetTotalPoints()
    {
        int total = 0;
        foreach (var c in cardsPanel.cards)
        {
            total += c.point;
        }

        return total;
    }
}