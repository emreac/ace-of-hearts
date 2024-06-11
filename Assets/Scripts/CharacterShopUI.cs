using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterShopUI : MonoBehaviour
{
    public GameManager gameManager;
    

    [Header("Layout Settings")]
   // [SerializeField] float itemSpacing = .5f;
    float itemHeight;

    [Header("UI Elements")]
    [SerializeField] Image selectedCharIcon;
    [SerializeField] Transform ShopMenu;
    [SerializeField] Transform ShopItemContainer;
    [SerializeField] GameObject itemPrefab;
    [Space(20)]

    [SerializeField] CharacterShopDatabase characterDB;


    [Header ("Shop Events")]
    [SerializeField] GameObject shopUI;
    [SerializeField] Button openShopBtn;
    [SerializeField] Button closeShopBtn;
    [Space(20)]

    [Header("GameScene")]
   // [SerializeField] Image gameSceneCharacterImage;
    [SerializeField] GameObject[] charSkins;
    [Space(20)]

    [Header("Error Message")]
    [SerializeField] TextMeshProUGUI notEnoughCoinText;



    int newSelectedItemIndex = 0;
    int previousSelectedItemIndex = 0;

    private void Start()
    {
        AddShopEvents();
        GenerateShopItemsUI();
        SetSelectedCharacter();

        //Select UI Item
        SelectItemUI(gameManager.GetSelectedCharacterIndex());

        //update player skin
        ChangePlayerSkin();
    }



    void SetSelectedCharacter()
    {
        //Get saved index
        int index = gameManager.GetSelectedCharacterIndex();

        //set selected char
        gameManager.SetSelectedCharacter(characterDB.GetCharacter(index),index);
    }


    void GenerateShopItemsUI()
    {
        //Save purchades items in as purchased in database array
        for(int i = 0; i < gameManager.GetAllPurchasedCharacter().Count; i++)
        {
            int purchasedCharacterIndex = gameManager.GetPurchasedCharacter(i);
            characterDB.PurchaseCharacter(purchasedCharacterIndex);

        }



        //Delete Item Template after height calculation
        itemHeight = ShopItemContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        Destroy(ShopItemContainer.GetChild (0).gameObject);
        ShopItemContainer.DetachChildren();

        //Generate Items
        for (int i = 0;i<characterDB.CharactersCount;i++)
        {
            Character character = characterDB.GetCharacter(i);
            CharacterItemUI uiItem = Instantiate(itemPrefab, ShopItemContainer).GetComponent<CharacterItemUI>();

            //Move item to its position
          // uiItem.SetItemPositon(Vector2.down*i*(itemHeight + itemSpacing));

            //Add info to UI
            uiItem.SetCharcterImage(character.image);
            uiItem.SetCharcterPrice(character.price);

            if (character.isPurchased)
            {
                //Char Purchased
                uiItem.SetCharcterAsPurchased();
                uiItem.OnItemSelect(i,OnItemSelected);
            }
            else
            {
                //No Purchase
                uiItem.SetCharcterPrice(character.price);
                uiItem.OnItemPurchased(i,OnItemPurchased);
            }
        }
    }

    void ChangePlayerSkin()
    {
        
        Character character = gameManager.GetSelectedCharacter();
        if (character.image != null)
        {
            // gameSceneCharacterImage.sprite = character.image;
            //Get selected char index
            int selectedSkin = gameManager.GetSelectedCharacterIndex();
            //Show selected skin's gameobject
            charSkins[selectedSkin].SetActive(true);
            //Hide others (except selected One)
            for (int i = 0; i < charSkins.Length; i++)
            {
                if(i != selectedSkin)
                    charSkins[i].SetActive(false);

            }

            //Selected Character Small Icon on top
            selectedCharIcon.sprite = gameManager.GetSelectedCharacter().image;
        }
    }
    void OnItemSelected(int index)
    {
        gameManager.chipSelectSound.Play();
        //Select item in the UI
        SelectItemUI(index);
      

        //save data
        gameManager.SetSelectedCharacter(characterDB.GetCharacter(index), index);
        //Change Player skin
        ChangePlayerSkin();
    }
    void SelectItemUI(int itemIndex)
    {
        previousSelectedItemIndex = newSelectedItemIndex;
        newSelectedItemIndex = itemIndex;

        CharacterItemUI prevUiItem = GetItemUI(previousSelectedItemIndex);
        CharacterItemUI newUiItem = GetItemUI(newSelectedItemIndex);

        prevUiItem.DeselectItem();
        newUiItem.SelectItem();

    }

    CharacterItemUI GetItemUI(int index)
    {
        return ShopItemContainer.GetChild(index).GetComponent<CharacterItemUI>();   
    }

    void OnItemPurchased(int index)
    {
        Character character = characterDB.GetCharacter(index);
        CharacterItemUI uiItem = GetItemUI(index);

        if (gameManager.CanSpendCoins(character.price))
        {
            //Proceed with the purchase operation
            gameManager.SpendCoins(character.price);
            //TODO: Purchase FX
            //Update UI coin text
            gameManager.UpdateTotalMoneyDisplay(gameManager.totalMoney);
            characterDB.PurchaseCharacter(index);
            uiItem.SetCharcterAsPurchased();
            uiItem.OnItemSelect(index,OnItemSelected);


            //Add purchased item to shop data
            gameManager.AddPurchasedCharacter(index);
        }
        else
        {
            AnimateNoMoreCoinText();
            uiItem.AnimateShakeItem();
            //Debug.Log("Not Enough Money!");
        }
    }

    void AnimateNoMoreCoinText()
    {
        //Complete Animations
        notEnoughCoinText.transform.DOComplete();
        notEnoughCoinText.DOComplete();
       

        notEnoughCoinText.transform.DOShakePosition(3f, new Vector3(5f, 0f, 0f), 10, 0);
        notEnoughCoinText.DOFade(1f, 3f).From(0f).OnComplete(() =>
        {
            notEnoughCoinText.DOFade(0f, 1f);

        });
        
    }

    void AddShopEvents()
    {
        openShopBtn.onClick.RemoveAllListeners();
        openShopBtn.onClick.AddListener(OpenShop);


        closeShopBtn.onClick.RemoveAllListeners();
        closeShopBtn.onClick.AddListener(CloseShop);
    }
    void OpenShop()
    {
        gameManager.chipSelectSound.Play();
        shopUI.SetActive (true);
    }
    void CloseShop()
    {
        gameManager.chipSelectSound.Play();
        shopUI.SetActive(false);
    }
}
