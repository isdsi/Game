using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManagerTest : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardStack;

    // Start is called before the first frame update
    void Start()
    {
        /*for (int i = 0; i < 52; i++)
        {
            GameObject newCard = Instantiate(cardPrefab);
            newCard.transform.SetParent(cardStack, false);
            newCard.name = "Card_" + (i + 1);

            // 2. 생성된 카드에서 'CardTest' 스크립트 기능을 가져옵니다. (핵심!)
            CardTest cardScript = newCard.GetComponent<CardTest>();

            // 3. 만약 스크립트를 찾는 데 성공했다면, 번호를 부여합니다.
            if (cardScript != null)
            {
                // CardTest에 있는 numberText 변수에 접근해 숫자를 넣습니다.
                // i는 0부터 시작하니 i+1을 해서 1~52가 찍히게 합니다.
                //cardScript.numberText.text = (i + 1).ToString();
            }
            Debug.Log((i + 1) + "번째 카드가 배치되었습니다.");
        }*/

        foreach (Suit s in Enum.GetValues(typeof(Suit)))
        {
            for (int r = 1; r <= 13; r++) 
            {
                GameObject newCard = Instantiate(cardPrefab);
                newCard.transform.SetParent(cardStack, false);
                newCard.name = "Card_" + (r + 1) + "_" + ((int)s);

                // 2. 생성된 카드에서 'CardTest' 스크립트 기능을 가져옵니다. (핵심!)
                CardTest cardScript = newCard.GetComponent<CardTest>();

                // 3. 만약 스크립트를 찾는 데 성공했다면, 번호를 부여합니다.
                if (cardScript != null)
                {
                    cardScript.Construct(s, r);
                }
            }
        }
        cardStack.GetChild(0)
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
