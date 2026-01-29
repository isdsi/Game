using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public enum Suit { Spades, Hearts, Diamonds, Clubs }

public class CardTest : MonoBehaviour, 
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    private Image cardImage;
    
    // 원래 자리를 기억할 변수
    private Vector3 startPosition;

    public TextMeshProUGUI numberText;

    public TextMeshProUGUI numberText2;

    public Suit Suit;
    public int Rank;
    public bool IsFaceUp;

    // Start is called before the first frame update
    void Start()
    {
        cardImage = GetComponent<Image>();
        //startPosition = cardImage.transform.position;
        SuitRankToNumberText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickCard()
    {
        Debug.Log("카드가 클릭되었습니다! 유니티 정복 시작!");
        //Color randomColor = new Color(Random.value, Random.value, Random.value);
        //cardImage.color = randomColor;
        IsFaceUp = !IsFaceUp;
        SuitRankToNumberText();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = cardImage.transform.position;
        Debug.Log("카드를 옮기기 시작합니다.");
    }

    public void OnDrag(PointerEventData eventData)
    {
        cardImage.transform.position = eventData.position;  
        Debug.Log("카드를 옮기는 중입니다: " + eventData.position);
    }

    public void OnEndDrag(PointerEventData eventdata)
    {
        cardImage.transform.position = startPosition;
        Debug.Log("집으로 돌아왔습니다.");
    }

    // 생성자 대신 사용하는 초기화 함수
    public void Construct(Suit suit, int rank)
    {
        Suit = suit;
        Rank = rank;
        IsFaceUp = false;
    }

    public string GetColor() => (Suit == Suit.Hearts || Suit == Suit.Diamonds) ? "Red" : "Black";
    public Color GetColorValue() => (Suit == Suit.Hearts || Suit == Suit.Diamonds) ? Color.red : Color.black;

    public override string ToString()
    {
        if (!IsFaceUp) return "[??]";
        string r = Rank switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => Rank.ToString()
        };
        char s = Suit switch
        {
            Suit.Spades => '♠',
            Suit.Hearts => '♥',
            Suit.Diamonds => '♦',
            Suit.Clubs => '♣',
            _ => ' '
        };
        return $"[{r}{s}]";
    }

    public void SuitRankToNumberText()
    {
        numberText.text = ToString();
        numberText.color = GetColorValue();
        numberText2.text = ToString();
        numberText2.color = GetColorValue();
    }
}
