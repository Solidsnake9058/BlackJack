using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Text playingDeckCounts;
    public Button increaseButton;
    public Button discreaseButton;
    public Button playButton;
    public Toggle fifToggle;
    public Toggle oneToggle;
    public Toggle halfToggle;

    public int minDecks = 1;
    public int maxDecks = 8;
    public int playingDecks = 3;
    public Enums.RestartMode defaultResetMode = Enums.RestartMode.fifCard;

    public void IncreaseDeck()
    {
        playingDecks++;
        playingDecks = playingDecks % (maxDecks - minDecks + 1);
        UpdatePlayingDeck();
        lockToggle();
    }

    public void DiscreaseDeck()
    {
        playingDecks--;
        playingDecks += (maxDecks - minDecks + 1);
        playingDecks = playingDecks % (maxDecks - minDecks + 1);
        UpdatePlayingDeck();
        lockToggle();
    }

    public void Play()
    {
        PlayerPrefs.SetInt("deckCounts", playingDecks + 1);
        PlayerPrefs.SetInt("restartMode", (int)defaultResetMode);

        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    private void UpdatePlayingDeck()
    {
        playingDeckCounts.text = (playingDecks + minDecks).ToString();
    }

    private void lockToggle()
    {
        if ((playingDecks + 1) > 1)
        {
            oneToggle.interactable = true;
            halfToggle.interactable = true;
        }
        else
        {
            fifToggle.isOn = true;
            oneToggle.interactable = false;
            halfToggle.interactable = false;
        }
    }

    // Use this for initialization
    void Start()
    {
        UpdatePlayingDeck();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

