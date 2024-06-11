using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using DG.DemiLib;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    //Sound
    [SerializeField] AudioSource cardShuffleSound;
    [SerializeField] AudioSource loseRoundSound;
    [SerializeField] AudioSource winSound;
    [SerializeField] AudioSource winBJSound;
    public AudioSource chipSelectSound;
    [SerializeField] AudioSource buttonSound;
    [SerializeField] AudioSource sadSound;
    public AudioSource payoutSound;

    //Particle Coin Burst
    [SerializeField] ParticleSystem coinBurst;

    //Tutorial UI
    [SerializeField] GameObject tutorialUI;
    [SerializeField] GameObject hitDialogText;
    [SerializeField] GameObject fingerBid;
    [SerializeField] GameObject fingerBigger16;
    [SerializeField] GameObject fingerLess16;
   
    [SerializeField] GameObject fingerStand;
    [SerializeField] GameObject fingerCall;


    //Animators
    [Header("Animators")]
    public Animator c1Anim;
    public Animator c2Anim;
    public Animator c3Anim;
    public Animator c4Anim;


    [Space(20)]
    [Header("Dealer Animators")]
    public Animator d1Anim;

    [Space(20)]
    //BetButtons
    public Button[] betButtons; // Reference to the bet buttons


    //Total Money and Selected Bet
    public int totalMoney = 1500;

    public bool hitDealer = false;

    //GameObject
    [SerializeField] GameObject bJ;
    public GameObject winnerDialogBox;
    public GameObject adScreen;
    //Coin Bar Fx
    public GameObject[] coinLost;
    public GameObject[] coinGained;
    //Colors
    public Color whiteClr = Color.white;

    // Game Buttons
    public Button dealBtn;
    public Button hitBtn;
    public Button standBtn;
    public Button betBtn;
    public Button shuffleBtn;
    public Button cancelBtn;
    public GameObject callGlow;

    //Bet buttons
    public Button bet100Btn;
    public Button bet200Btn;
    public Button bet300Btn;

    // public Button betBtnTest;

    //Intergers
    private int standClicks = 0;
    public int betAmount;
    private int selectedBet = 0;
    public float animationDuration = 1f;

    [SerializeField] private float current, target;



    // Access the player and dealer's script
    public PlayerScript playerScript;
    public PlayerScript dealerScript;

    // Access the deck scritp
    public DeckScript deckScript;

    // public Text to access and update - hud
    public Text betBtnText;
    public Text hitBtnText;
    public Text dealBtnText;
    public Text PlayerHandText;
    public Text dealerHandText;
    //public Text cashText;
    public Text mainText;
    public Text standBtnText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI cashTextShop;


    // Card hiding dealer's 2nd card
    public GameObject hideCard;
    //CharacterSelection
    public int selectedCharacterIndex = 0;
    static CharacterShopData characterShopData = new CharacterShopData();
    static Character selectedCharacter;


    void Start()
    {
        //Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
        //Tutorial UI
        // Check if the tutorial has been shown before
        if (PlayerPrefs.GetInt("TutorialShown", 0) == 0)
        {
            // Show the tutorial UI
            tutorialUI.SetActive(true);
        }
        else
        {
            // Hide the tutorial UI
            tutorialUI.SetActive(false);
        }



        //Get Money
        LoadPlayer();
        SavePlayer();

        UpdateTotalMoneyDisplay(totalMoney);
    
        //Enable all bet buttons
        foreach (var button in betButtons)
        {
            button.interactable = true;
        }


        whiteClr.a = 0.1f;

        // Add on click listeners to the buttons
        shuffleBtn.onClick.AddListener(() => ShuffleClicked());
        dealBtn.onClick.AddListener(() => DealClicked());
        hitBtn.onClick.AddListener(() => HitClicked());
        standBtn.onClick.AddListener(() => StandClicked());

        //Add on click listernes to bet buttons
        bet100Btn.onClick.AddListener(() => Bet100BtnClicked());
        bet200Btn.onClick.AddListener(() => Bet200BtnClicked());
        bet300Btn.onClick.AddListener(() => Bet300BtnClicked());
        cancelBtn.onClick.AddListener(() => CancelButton());

        dealBtn.interactable = false;
        hitBtn.interactable = false;
        standBtn.interactable = false;
        betBtn.interactable = true;


    }
    public void CloseTutorial()
    {
        // Hide the tutorial UI
        tutorialUI.SetActive(false);

        // Set the flag to indicate that the tutorial has been shown
        PlayerPrefs.SetInt("TutorialShown", 1);
        PlayerPrefs.Save();
    }

    //Player Coin Save and Load Data
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
        Debug.Log("<color=green>Data Saved!</color>");
    }
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        totalMoney = data.totalMoney;
        Debug.Log("<color=magenta>Data Loaded!</color>");
    }
    public void SaveCharacter()
    {
        SaveSystem.SavePlayer(this);
        Debug.Log("<color=green>Data Saved!</color>");
    }
    public void LoadPCharacter()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        //totalMoney = data.totalMoney;
        Debug.Log("<color=magenta>Data Loaded!</color>");
    }


    public  void AddPurchasedCharacter(int characterIndex)
    {
        characterShopData.purchasedCharactersIndexes.Add(characterIndex);
        SavePlayer();
        
    }
    public  Character GetSelectedCharacter()
    {
        return selectedCharacter;
    }
    public void SetSelectedCharacter(Character character,int index)
    {
        selectedCharacter = character;
        selectedCharacterIndex = index;
        SavePlayer();
    }
    public List<int> GetAllPurchasedCharacter()
    {
        return characterShopData.purchasedCharactersIndexes;
    }
    public int GetPurchasedCharacter(int index)
    {
        return characterShopData.purchasedCharactersIndexes[index];
    }

    public  int GetSelectedCharacterIndex()
    {
        return selectedCharacterIndex;
    }

    public  bool CanSpendCoins(int amount)
    {
        return (totalMoney >= amount);
    }

    public  void SpendCoins(int amount)
    {
        totalMoney -= amount;
        SavePlayer();
    }

    public void Bet100BtnClicked()
    {
        
        DOTween.PlayForward("Bet100Glow");
    }
    public void Bet200BtnClicked()
    {
        DOTween.PlayForward("Bet200Glow");
    }
    public void Bet300BtnClicked()
    {
        DOTween.PlayForward("Bet300Glow");
    }

    public void OnBetButtonClicked(int betAmount)
    {
        StartCoroutine(BetAndCardSound());
        //cardShuffleSound.Play();
        fingerBid.SetActive(false);
        hitBtn.interactable = true;
        dealBtn.interactable = true;
        dealBtnText.GetComponent<Text>().color = Color.white;
   
       

        if (totalMoney >= betAmount)
        {
            totalMoney -= betAmount;
            selectedBet = betAmount;// Assign the betAmount to selectedBet
            dealBtn.interactable = true;
         
            //Starts game if money is enough
            DealClicked();
            DOTween.PlayForward("GameCards");
            DOTween.Restart("CoinLost");
            DOTween.Restart("CoinBarShake");
            // Perform other betting logic
            UpdateTotalMoneyDisplay(totalMoney);
        }
        else
        {
            // NOT ENOUGH MONEY!
            // Show "Not Enough Money" prompt and enable Watch Video button
          
            adScreen.SetActive(true);
            DOTween.Play("AdShineRotate");
            DOTween.PlayForward("AdFade");
      
        }
        // Disable all bet buttons
        
        foreach (var button in betButtons)
        {
            button.interactable = false;
        }
        DOTween.PlayForward("DealNeon");
    }
    public void UpdateTotalMoneyDisplay(int newTotalMoney)
    {
        totalMoney = newTotalMoney;
      
      //  cashText.text = $"{totalMoney}";
        AnimateNumber(totalMoney);
    }

    //Animate Numbers
    private void AnimateNumber(int newNumber)
    {
        // Tween the text value from its current value to the new number
        int currentNumber = int.Parse(cashText.text);
        DOTween.To(() => currentNumber, x => currentNumber = x, newNumber, animationDuration)
                .OnUpdate(() => cashText.text = currentNumber.ToString())
                .SetEase(Ease.Linear);

        // Tween the text value from its current value to the new number (Shop)
        int currentNumberShop = int.Parse(cashText.text);
        DOTween.To(() => currentNumberShop, x => currentNumberShop = x, newNumber, animationDuration)
                .OnUpdate(() => cashTextShop.text = currentNumberShop.ToString())
                .SetEase(Ease.Linear);
    }



    //Cancel Button
    public void CancelButton()
    {
        

        foreach (var button in betButtons)
        {
            button.interactable = true;
        }

        //Bet button neons
        DOTween.PlayBackwards("Bet100Glow");
        DOTween.PlayBackwards("Bet200Glow");
        DOTween.PlayBackwards("Bet300Glow");

        DOTween.Pause("AdShineRotate");
        adScreen.SetActive(false);
    }
    //Watch Ad Logic
    public void OnWatchVideoButtonClicked()
    {
        if (totalMoney > 0)
        {
          
        }
        else
        {
          
        }
        
        DOTween.PlayBackwards("AdFade");
        // Simulate watching an ad (add 300 money)
        totalMoney += 300;
        // Hide the prompt and disable the button
        UpdateTotalMoneyDisplay(totalMoney);
        adScreen.SetActive(false);
        foreach (var button in betButtons)
        {
            button.interactable = true;
        }
    }


    private void Update()
    {
     
    }

    public IEnumerator DealCoroutine()
    {
        yield return new WaitForSeconds(1);
        DOTween.PlayForward("DealNeon");
        dealBtnText.GetComponent<Text>().color = Color.white;
        //Invoke("LaunchProjectile", 2.0f);
        dealBtn.interactable = true;

        //yield on a new YieldInstruction that waits for 3 seconds.
       
    }
    public IEnumerator BetAndCardSound()
    {
        chipSelectSound.Play();
        yield return new WaitForSeconds(.5f);

        cardShuffleSound.Play();

    }

    public IEnumerator BetButtonsCoroutine()
    {
        yield return new WaitForSeconds(1);
        foreach (var button in betButtons)
        {
            button.interactable = true;
        }

    }
    public void ShuffleClicked()
    {
        cardShuffleSound.Play();
        StartCoroutine(BetButtonsCoroutine());
     

        //Bet button neons
        DOTween.PlayBackwards("Bet100Glow");
        DOTween.PlayBackwards("Bet200Glow");
        DOTween.PlayBackwards("Bet300Glow");


        DOTween.PlayBackwards("DealNeon");
        DOTween.PlayBackwards("CardsFade");
        DOTween.PlayBackwards("HandGlow");
       
        betBtn.interactable = true;
        dealerHandText.GetComponent<Text>().color = Color.white;
        PlayerHandText.GetComponent<Text>().color = Color.white;
        
        StartCoroutine(DealCoroutine());
        dealerHandText.text = "?";
        PlayerHandText.text = "?";
        DOTween.PlayBackwards("BlueNeon");
        DOTween.PlayBackwards("RedNeon");
    }
    public void DealClicked()
    {
        hitDialogText.SetActive(true);
        //Tutorial UI Fingers

        fingerBigger16.SetActive(true);
        fingerLess16.SetActive(true);

        DOTween.PlayForward("HandGlow");
        DOTween.PlayBackwards("DealNeon");
        DOTween.PlayForward("CardsFade");
        // Reset round, hide text, prep for new hand

        playerScript.ResetHand();
        dealerScript.ResetHand();

        // Hide deal hand score at start of deal
        //dealerScoreText.gameObject.SetActive(false);
        winnerDialogBox.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false);
        dealerHandText.gameObject.SetActive(false);
        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
        playerScript.StartHand();
        dealerScript.StartHand();

        // Update the scores displayed
        PlayerHandText.text = "" + playerScript.handValue.ToString();
        dealerHandText.text = "" + dealerScript.handValue.ToString();
        // Place card back on dealer card, hide card
        hideCard.GetComponent<Renderer>().enabled = true;
        // Adjust buttons visibility
        //dealBtn.gameObject.SetActive(false);
        betBtnText.GetComponent<Text>().color = Color.white;
        standBtnText.GetComponent<Text>().color = Color.white;
        dealBtnText.GetComponent<Text>().color = whiteClr;
        hitBtnText.GetComponent<Text>().color = Color.white;
        betBtn.interactable = false;
        dealBtn.interactable = false;
        hitBtn.interactable = true;
        standBtn.interactable = true;
        //hitBtn.gameObject.SetActive(true);
        //standBtn.gameObject.SetActive(true);
        standBtnText.text = "STAND";
        // Set standard pot size
 

    }
    public void ChangeTextColor()
    {
        standBtnText.GetComponent<Text>().color = Color.black;
    }
    private void HitClicked()
    {
        chipSelectSound.Play();
        fingerBigger16.SetActive(false);
        fingerLess16.SetActive(false);
        hitDialogText.SetActive(false);

        // Check that there is still room on the table
        if (playerScript.cardIndex <= 10)
        {
            playerScript.GetCard();
            PlayerHandText.text = "" + playerScript.handValue.ToString();
            if (playerScript.handValue > 20) RoundOver();
        }
    }

    private void StandClicked()
    {
        callGlow.SetActive(true);
        chipSelectSound.Play();
        fingerBigger16.SetActive(false);
        fingerLess16.SetActive(false);
        hitDialogText.SetActive(false);
        hitBtn.interactable = false;
        standClicks++;
        if (standClicks > 1) RoundOver();
        HitDealer();
        standBtnText.text = "CALL";
    }

    private void HitDealer()
    {
        
        while (dealerScript.handValue < 16 && dealerScript.cardIndex < 10)
        {
            dealerScript.GetCard();
            hitDealer = true;
            dealerHandText.text = "" + dealerScript.handValue.ToString();
            if (dealerScript.handValue > 20) RoundOver();
        }
    }
    public void ShuffleCardsButton()
    {
        //Character Animators
        c1Anim.Rebind();
        c2Anim.Rebind();
        c3Anim.Rebind();
        c4Anim.Rebind();
        
        //Dealer
        d1Anim.Rebind();


        //Animations
        DOTween.PlayBackwards("ChipNeon");
        DOTween.Pause("AdShineRotate");
        DOTween.Pause("ShineRotate");
        DOTween.PlayBackwards("ShineFade");
        DOTween.PlayBackwards("Green");
        DOTween.PlayBackwards("Red");
        DOTween.PlayBackwards("FadeIn");
        //Winner Dialog
        winnerDialogBox.gameObject.SetActive(false);
        //Text colors
        dealerHandText.GetComponent<Text>().color = Color.white;
        PlayerHandText.GetComponent<Text>().color = Color.white;
    }

    // Check for winnner and loser, hand is over
     public void RoundOver()
    {
        callGlow.SetActive(false);
        CloseTutorial();
        
        // Booleans (true/false) for bust and blackjack/21
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        bool player21 = playerScript.handValue == 21;
        bool dealer21 = dealerScript.handValue == 21;
        // If stand has been clicked less than twice, no 21s or busts, quit function
        if (standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21) return;
        bool roundOver = true;

        winnerDialogBox.gameObject.SetActive(true);
        // All bust, bets returned
        if (playerBust && dealerBust)
        {

            totalMoney += selectedBet;
            UpdateTotalMoneyDisplay(totalMoney);
            mainText.text = "RETURNED";
            
            //Character Animations
            c1Anim.SetTrigger("isSurprised");
            c2Anim.SetTrigger("isSurprised");
            c3Anim.SetTrigger("isSurprised");
            c4Anim.SetTrigger("isSurprised");
       
            //Dealer Animations
            d1Anim.SetTrigger("isSurprised");


        }
        // if player busts, dealer didnt, or if dealer has more points, dealer wins
        else if (playerBust || (!dealerBust && dealerScript.handValue > playerScript.handValue))
        {
            dealerHandText.GetComponent<Text>().color = Color.red;
            DOTween.Restart("CoinLost");
            DOTween.Restart("CoinBarShake");
            DOTween.PlayForward("RedNeon");
            DOTween.PlayForward("FadeIn");
            DOTween.PlayForward("Red");
            UpdateTotalMoneyDisplay(totalMoney);
            Debug.Log(totalMoney.ToString());
            mainText.text = "YOU LOST";
            loseRoundSound.Play();
            sadSound.Play();
            //Char Animators
            c1Anim.SetTrigger("isAngry");
            c2Anim.SetTrigger("isAngry");
            c3Anim.SetTrigger("isAngry");
            c4Anim.SetTrigger("isAngry");
            
            //Dealer Animations
            d1Anim.SetTrigger("isHappy");

            //Save Data
            SavePlayer();
        }
        // if dealer busts, player didnt, or player has more points, player wins
        else if (dealerBust || playerScript.handValue > dealerScript.handValue)
        {
            coinBurst.Play();
            if (playerScript.handValue == 21)
            {
                StartCoroutine(BJDelay());
                winBJSound.Play();
                totalMoney += selectedBet * 3;
            }
            
            PlayerHandText.GetComponent<Text>().color = Color.green;
            DOTween.Restart("CoinGained");
            DOTween.Play("ShineRotate");
            DOTween.PlayForward("ShineFade");
            DOTween.PlayForward("BlueNeon");
            DOTween.PlayForward("FadeIn");
            DOTween.PlayForward("Green");
            //Win money logic
            totalMoney += selectedBet * 2;
            UpdateTotalMoneyDisplay(totalMoney);
            Debug.Log(totalMoney.ToString());
            mainText.text = "YOU WIN!";
            winSound.Play();
            //Character Animators
            c1Anim.SetTrigger("isHappy");
            c2Anim.SetTrigger("isHappy");
            c3Anim.SetTrigger("isHappy");
            c4Anim.SetTrigger("isHappy");
            
            //Dealer Animations
            d1Anim.SetTrigger("isAngry");
            //Save Data
            SavePlayer();
    
        }
        //Check for tie, return bets
        else if (playerScript.handValue == dealerScript.handValue)
        {
            DOTween.PlayForward("FadeIn");
            DOTween.Restart("CoinGained");
            //Give bets back
            totalMoney += selectedBet;
            UpdateTotalMoneyDisplay(totalMoney);
            //Text colors
            dealerHandText.GetComponent<Text>().color = Color.yellow;
            PlayerHandText.GetComponent<Text>().color = Color.yellow;
            mainText.text = "RETURNED";
           
            //Character Animations
            c1Anim.SetTrigger("isSurprised");
            c2Anim.SetTrigger("isSurprised");
            c3Anim.SetTrigger("isSurprised");
            c4Anim.SetTrigger("isSurprised");

            //Dealer Animations
            d1Anim.SetTrigger("isSurprised");
            //Save Data
            SavePlayer();

        }
        else
        {
            roundOver = false;
        }

        

        // Set ui up for next move / hand / turn
        if (roundOver)
        {
            DOTween.PlayBackwards("ChipNeon");
            //DOTween.Play("CardBackMove");
            hitBtn.interactable = false;
            standBtn.interactable = false;
            standBtnText.GetComponent<Text>().color = whiteClr;
            hitBtnText.GetComponent<Text>().color = whiteClr;
            
            // hitBtn.gameObject.SetActive(false);
            // standBtn.gameObject.SetActive(false);
            //dealBtn.gameObject.SetActive(true);
            
            mainText.gameObject.SetActive(true);
            dealerHandText.gameObject.SetActive(true);
            hideCard.GetComponent<Renderer>().enabled = false;
            UpdateTotalMoneyDisplay(totalMoney);
            standClicks = 0;
        }
       
    }

    IEnumerator BJDelay()
    {
        //Print the time of when the function is first called.
        bJ.SetActive(true);
        DOTween.Restart("BjFadein");
        DOTween.Restart("BjScale");
        

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1.5f);
   
        //After we have waited 1 seconds print the time again.
        bJ.SetActive(false);
    }

}
