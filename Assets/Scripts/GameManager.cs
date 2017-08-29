using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager inatance;

    public GameObject cardPrefab;
    public Transform deckPos;
    public Transform playerCardPos;
    public Transform computerCardPos;
    public Transform usedCardPos;
    public Button dealButton;
    public Button startButton;
    public Button standButton;
    public Button increaseButton;
    public Button discreaseButton;
    public Text betsText;
    public Text chipsText;

    public Image totalImage;
    public Text resultText;
    public Text comPointText;
    public Text player1PointText;
    public Button okayButton;
    public Button continueButton;
    public Button exitButton;


    public int deckCounts = 1;
    public int chips = 1000;
    public int bets = 0;
    public int betAdd = 100;
    public List<GameObject> gameDeck = new List<GameObject>();
    public List<GameObject> playerCards = new List<GameObject>();
    public List<GameObject> computerCards = new List<GameObject>();
    public List<GameObject> usedCards = new List<GameObject>();
    public bool isSendingCard = false;
    public bool isRecycle = false;
    public Enums.RestartMode restartMode = Enums.RestartMode.fifCard;

    private bool isPlayerTurn = true;
    private int playerPoint = 0;
    private int computerPoint = 0;
    private int totalRate = 0;
    private static int blackJack = 21;
    private static int deckCards = 52;


    public void CreateDeck()
    {
        gameDeck = new List<GameObject>();
        playerCards = new List<GameObject>();
        computerCards = new List<GameObject>();
        usedCards = new List<GameObject>();

        for (int i = 0; i < deckCounts; i++)
        {
            gameDeck.AddRange(CreateSingleDeck());
        }

        gameDeck = Shuffle(gameDeck);
    }

    private List<GameObject> CreateSingleDeck()
    {
        List<GameObject> deck = new List<GameObject>();
        for (int i = 1; i < 53; i++)
        {
            GameObject card = (GameObject)Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3()));
            Card cardScript = card.GetComponent<Card>();
            cardScript.cardIndex = i;
            cardScript.SetCardFace();
            cardScript.TurnCard();
            deck.Add(card);
        }
        return deck;
    }

    private static List<T> Shuffle<T>(IEnumerable<T> values)
    {
        List<T> list = new List<T>(values);
        T tmp;
        int iS;
        for (int N1 = 0; N1 < list.Count; N1++)
        {
            iS = Random.Range(0, list.Count);
            tmp = list[N1];
            list[N1] = list[iS];
            list[iS] = tmp;
        }
        return list;
    }

    public void DealCard(bool isUpdate = true)
    {
        StartCoroutine(SendIngCard(isUpdate));
    }

    public void Pass()
    {
        DisableDeal();
        NextTurn();
        computerCards[0].GetComponent<Card>().TurnCard();
        CheckPoint();
    }

    private void NextTurn()
    {
        isPlayerTurn = !isPlayerTurn;
    }

    public void StartGame()
    {
        StartCoroutine(StartSendCard());
    }

    public void ActiveDeal()
    {
        isSendingCard = false;
        dealButton.interactable = true;
        standButton.interactable = true;
    }

    public void DisableDeal()
    {
        dealButton.interactable = false;
        standButton.interactable = false;
    }

    public void ActiveStart()
    {
        startButton.interactable = true;
    }

    public void DisableStart()
    {
        startButton.interactable = false;
    }

    public void ActiveBets()
    {
        increaseButton.interactable = true;
        discreaseButton.interactable = true;
        if (bets >0)
        {
            ActiveStart();
        }
        else
        {
            DisableStart();
        }
    }

    public void DisableBets()
    {
        increaseButton.interactable = false;
        discreaseButton.interactable = false;
        DisableStart();
    }

    public void IncreaseBets()
    {
        if (chips -betAdd >=0)
        {
            chips -= betAdd;
            bets += betAdd;
        }
        ActiveStart();
        UpdateChips();
    }

    public void DiscreaseBets()
    {
        if (bets - betAdd >= 0)
        {
            bets -= betAdd;
            chips += betAdd;
        }
        if (bets == 0)
        {
            DisableStart();
        }
        UpdateChips();
    }

    public void Recycle()
    {
        CollectDeck(playerCards, usedCards);
        CollectDeck(computerCards, usedCards);

        TotalChips(totalRate);
        ResetBets();

        okayButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        continueButton.interactable = chips >= betAdd ? true : false;
    }

    private void CollectDeck(List<GameObject> deck, List<GameObject> targetDeck)
    {
        int existCardCount = targetDeck.Count;
        for (int i = 0; i < deck.Count; i++)
        {
            deck[i].transform.localPosition = new Vector3(usedCardPos.position.x, usedCardPos.position.y, (existCardCount + 1) * -0.01f);
            if (!deck[i].GetComponent<Card>().GetIsFaceOn)
            {
                deck[i].GetComponent<Card>().TurnCard();
            }
            targetDeck.Add(deck[i]);
        }
        deck.Clear();
    }

    IEnumerator SendIngCard(bool isUpdate = true)
    {
        if (!isSendingCard)
        {
            isSendingCard = true;
            dealButton.interactable = !isSendingCard;

            GameObject dealCard = gameDeck[0];
            Card card = gameDeck[0].GetComponent<Card>();
            if (!(!isPlayerTurn && computerCards.Count == 0))
            {
                card.TurnCard();
            }
            Vector3 targetPos = isPlayerTurn ? playerCardPos.position : computerCardPos.position;
            int targetCardCounts = isPlayerTurn ? playerCards.Count : computerCards.Count;
            card.settingPos = new Vector3(targetPos.x + (targetCardCounts * 1.45f * (isPlayerTurn ? 1 : -1)), targetPos.y, 0);
            card.setToPos = true;
            if (isPlayerTurn)
            {
                playerCards.Add(dealCard);
            }
            else
            {
                computerCards.Add(dealCard);
            }
            gameDeck.RemoveAt(0);

            yield return new WaitForSeconds(1);
            isSendingCard = false;

            if (isUpdate)
            {
                if (isPlayerTurn)
                {
                    ActiveDeal();
                }
                UpdatePoint();
            }
        }
    }

    IEnumerator StartSendCard()
    {
        DisableBets();
        for (int i = 0; i < 4; i++)
        {
            DealCard(false);
            NextTurn();
            yield return new WaitForSeconds(1);
        }
        ActiveDeal();
        UpdatePoint();
    }

    private void CheckPoint()
    {
        bool isEnd = false;
        if (isPlayerTurn)
        {
            if (playerPoint == blackJack)
            {
                Pass();
            }
            else if (playerPoint > blackJack)
            {
                //loss
                DisableDeal();
                computerCards[0].GetComponent<Card>().TurnCard();
                totalRate = 0;
                isEnd = true;
            }
        }
        else
        {
            if (computerPoint == blackJack)
            {
                if (playerPoint != blackJack)
                {
                    //loss
                    totalRate = 0;
                }
                else
                {
                    //draw
                    totalRate = 1;
                }
                isEnd = true;
            }
            else if (computerPoint > blackJack)
            {
                //win
                totalRate = 2;
                isEnd = true;
            }
            else
            {
                if (computerPoint < 17)
                {
                    DealCard();
                }
                else
                {
                    bool isHit = false;
                    bool isHasAce = false;
                    int tempPoint = 0;
                    List<List<int>> tempCards = computerCards.Select(x => x.GetComponent<Card>().point).ToList();
                    for (int i = 0; i < tempCards.Count; i++)
                    {
                        tempPoint += tempCards[i][0];
                        if (tempCards[i].Count >1)
                        {
                            isHasAce = true;
                        }
                    }
                    if (isHasAce)
                    {
                        isHit = (tempPoint != computerPoint); 
                    }
                    if (!isHit)
                    {
                        if (computerPoint < playerPoint)
                        {
                            int runningCount = GetRunningCount();
                            float trueCount = runningCount / (gameDeck.Count / (float)deckCounts);
                            float randRate = Random.Range(0, Mathf.Abs(trueCount));
                            Debug.Log(runningCount + "," + trueCount + "," + randRate);
                            if (trueCount < 0)
                            {
                                if (randRate < Mathf.Abs(trueCount) * 0.5f)
                                {
                                    isHit = true;
                                }
                            }
                            else
                            {
                                if (randRate < trueCount * 0.2f)
                                {
                                    isHit = true;
                                }
                            }
                            //win
                            totalRate = 2;
                        }
                        else if (computerPoint > playerPoint)
                        {
                            //loss
                            totalRate = 0;
                        }
                        else
                        {
                            //draw
                            totalRate = 1;
                        }
                    }

                    if (isHit)
                    {
                        DealCard();
                    }
                    isEnd = !isHit;
                }
            }
        }

        if (isEnd)
        {
            switch (totalRate)
            {
                case 0:
                    resultText.text = "Loss!";
                    resultText.color = Color.red;
                    break;
                case 1:
                    resultText.text = "Draw!";
                    resultText.color = Color.blue;
                    break;
                default:
                    resultText.text = "Win!";
                    resultText.color = Color.yellow;
                    break;
            }

            SetResultPoint(comPointText, computerPoint);
            SetResultPoint(player1PointText, playerPoint);
            totalImage.gameObject.SetActive(true);
            okayButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(false);
        }
    }

    private void SetResultPoint(Text uiText,int Point)
    {
        if (Point==blackJack)
        {
            uiText.text = "Black Jack";
            uiText.color = Color.yellow;
        }
        else if (Point > blackJack)
        {
            uiText.text = "Bust";
            uiText.color = Color.red;
        }
        else
        {
            uiText.text = Point.ToString();
            uiText.color = Color.white;
        }
    }

    public void ResetGame()
    {
        bool resetDeck = false;
        totalImage.gameObject.SetActive(false);
        isPlayerTurn = true;
        switch (restartMode)
        {
            case Enums.RestartMode.oneDeck:
                if (gameDeck.Count < deckCards)
                {
                    resetDeck = true;
                }
                break;
            case Enums.RestartMode.halfCountDeck:
                if (gameDeck.Count < (deckCards * deckCounts) / 2)
                {
                    resetDeck = true;
                }
                break;
            case Enums.RestartMode.fifCard:
                if (gameDeck.Count < 15)
                {
                    resetDeck = true;
                }
                break;
            default:
                break;
        }

        if (resetDeck)
        {
            CollectDeck(playerCards, gameDeck);
            CollectDeck(computerCards, gameDeck);
            CollectDeck(usedCards, gameDeck);

            gameDeck = Shuffle(gameDeck);
            ResetDeckPos();
        }

        ActiveBets();
    }

    public void ResetDeckPos()
    {
        for (int i = 0; i < gameDeck.Count; i++)
        {
            if (gameDeck[i].GetComponent<Card>().GetIsFaceOn)
            {
                gameDeck[i].GetComponent<Card>().TurnCard();
            }
            gameDeck[i].transform.localPosition = new Vector3(deckPos.position.x, deckPos.position.y, (i * 0.01f) - (gameDeck.Count * 0.01f));
        }
    }

    public void Exit()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    private void ResetBets()
    {
        bets = 0;
        UpdateChips();
    }

    private void TotalChips(int rate)
    {
        chips += bets * rate;
    }

    private void UpdatePoint()
    {
        playerPoint = CalPoint(playerCards);
        computerPoint = CalPoint(computerCards);

        CheckPoint();
    }

    private int CalPoint(List<GameObject> cards)
    {
        if (cards.Count > 0)
        {
            int tempPoint = 0;
            List<List<int>> tempCards = cards.Select(x => x.GetComponent<Card>().point).ToList();
            tempCards.Sort((x, y) => x.Count.CompareTo(y.Count));
            for (int i = 0; i < tempCards.Count; i++)
            {
                List<int> points = tempCards[i];

                if (points.Count == 1)
                {
                    tempPoint += points[0];
                }
                else
                {
                    for (int j = 0; j < points.Count; j++)
                    {
                        if (j == 0)
                        {
                            tempPoint += points[j];
                        }
                        else
                        {
                            if ((tempPoint + points[j] - points[j - 1]) <= 21)
                            {
                                tempPoint += (points[j] - points[j - 1]);
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (tempPoint >= blackJack)
                        {
                            break;
                        }
                    }
                }
            }
            return tempPoint;
        }
        else
        {
            return 0;
        }
         
    }

    private void UpdateChips()
    {
        chipsText.text = chips.ToString();
        betsText.text = bets.ToString();
    }

    private int  GetRunningCount()
    {
        return GetHLCount(usedCards) + GetHLCount(computerCards) + GetHLCount(playerCards);
    }

    private int GetHLCount(List<GameObject> cards, bool skipFirst = false)
    {
        int hlCount = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            if (skipFirst && i == 1)
            {
                continue;
            }
            Card card = cards[i].GetComponent<Card>();
            hlCount += card.hlPoint;
        }
        return hlCount;
    }


    // Use this for initialization
    void Start()
    {
        deckCounts = PlayerPrefs.GetInt("deckCounts");
        restartMode = (Enums.RestartMode)PlayerPrefs.GetInt("restartMode");

        Debug.Log(deckCounts + "," + restartMode);

        totalImage.gameObject.SetActive(false);

        CreateDeck();
        UpdateChips();
        ResetDeckPos();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
