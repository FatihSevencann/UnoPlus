using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerCards : MonoBehaviour
{
    public float maxSpace;
    public Vector2 cardSize;
    public List<Card> cards;
    private Player player;
    public float aciAraligi;

    public GameObject WhichGrid;
    // Kartların arasındaki açı


    public List<Card> AllowedCard
    {
        get
        {
            List<Card> e = cards.FindAll(
                (obj) => { return obj.IsAllowCard(); });
            return e;
        }
    }

    public List<Card> DisallowedCard
    {
        get
        {
            List<Card> e = cards.FindAll(
                (obj) => { return !obj.IsAllowCard(); });
            return e;
        }
    }

    void Awake()
    {
        cards = new List<Card>();
    }

    void Update()
    {
#if UNITY_EDITOR
        GetComponent<RectTransform>().anchorMax = Vector2.one * .5f;
        GetComponent<RectTransform>().anchorMin = Vector2.one * .5f;
        GetComponent<RectTransform>().pivot = Vector2.one * .5f;
#endif
    }

    public void UpdatePos(float delay = 0f)
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
            cards = new List<Card>(GetComponentsInChildren<Card>());
#endif
        if (cards.Count > 0 && cards[0].IsOpen)
            cards.Sort((x, y) => y.Type.CompareTo(x.Type));
        float space = 0;
        float start = 0;
        float weight = 0;
        float heightSpace = 2;


        float totalWidht = GetComponent<RectTransform>().sizeDelta.x;
        float totalHeight = GetComponent<RectTransform>().sizeDelta.y;
        if (cards.Count > 1)
        {
            space = (totalWidht - cardSize.x) / (cards.Count - 1);

            float maxCardHeight = (cards.Count - 1) * heightSpace + cardSize.y;
            heightSpace = (totalHeight - maxCardHeight) / (cards.Count - 1);

            if (space > maxSpace)
            {
                space = maxSpace;
                totalWidht = (space * (cards.Count - 1)) + cardSize.x;
            }

            start = (totalWidht / -2) + cardSize.x / 2;
        }


        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform item = cards[i].GetComponent<RectTransform>();
            item.SetSiblingIndex(i);
            item.anchorMax = Vector2.one * .5f;
            item.anchorMin = Vector2.one * .5f;
            item.pivot = Vector2.one * .5f;
            item.sizeDelta = cardSize;

            float Space = 55f;

            float yPos = (cards.Count - 1 - i) * heightSpace + cardSize.y / 2f;

            float yPosLast = -(cards.Count - 1 - i) * heightSpace + cardSize.y / 2f - Space;


#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                if (player.isUserPlayer)
                {
                    float aci = (i - (cards.Count - 1) / 2f) * aciAraligi;


                    cards[i].transform.position = new Vector3(start, yPos, 0);
                    cards[i].transform.rotation = Quaternion.Euler(0f, 0f, -aci);
                }
            }

            else
            {
                float aci = (i - (cards.Count - 1) / 2f) * aciAraligi;
                if (WhichGrid.CompareTag("Human")) 
                    cards[i].transform.localPosition = new Vector3(start,  i<=(cards.Count-1)/2 ? yPos: yPosLast,0f ); 
                
                else
                {
                    cards[i].transform.localPosition = new Vector3(start, 0, 0f);
                }


                cards[i].transform.rotation = Quaternion.Euler(0, 0f, -aci);
            }
#else
			cards [i].SetTargetPosAndRot(new Vector3 (start, 0f,0f),0f);
#endif
            start += space;
        }
    }

    public int GetCount(CardType t)
    {
        List<Card> temp = cards.FindAll((obj) => obj.Type == t);
        if (temp == null)
            return 0;
        return temp.Count;
    }
}