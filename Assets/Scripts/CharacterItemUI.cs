using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using DG.Tweening;



/// <summary>
/// Visualize character attributes
/// </summary>
public class CharacterItemUI : MonoBehaviour
{

    [SerializeField] Color itemNotSelectedColor;
    [SerializeField] Color itemSelectedColor;

    [Space(20f)]
    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI characterPriceText;    
   // [SerializeField] TMP_Text characterNameText; // > Activate when add char Names   
    [SerializeField] Button characterPurchaseBtn;
    [Space(20f)]
    [SerializeField] Button itemBtn;
    [SerializeField] Image itemImage;
    [SerializeField] Outline itemOutline;


    //------------------------------------------------------//

    public void SetItemPositon(Vector2 pos)
    {
        GetComponent<RectTransform>().anchoredPosition = pos;
    }
    public void SetCharcterImage(Sprite sprite)
    {
        characterImage.sprite = sprite;

    }

    //TODO : CHARACTER NAME WILL BE ADDED
    /*
    public void SetCharcterName(String name)
    {
        characterNameText.text = name;

    }
    */
    public void SetCharcterPrice(int price)
    {
        characterPriceText.text = price.ToString();

    }
    public void  SetCharcterAsPurchased()
    {
        //After Purchase the char
        characterPurchaseBtn.interactable = false;
      //characterPurchaseBtn.gameObject.SetActive(false);
        characterPurchaseBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "OWNED";
        characterPurchaseBtn.transform.GetChild(1).gameObject.SetActive(false); 
        //
        itemBtn.interactable = true;
        itemImage.color = itemNotSelectedColor;

    }
    public void OnItemPurchased(int itemIndex, UnityAction<int> action)
    {
        characterPurchaseBtn.onClick.RemoveAllListeners();
        characterPurchaseBtn.onClick.AddListener(()=>action.Invoke (itemIndex));

    }

    public void OnItemSelect(int itemIndex, UnityAction<int> action)
    {
        itemBtn.interactable = true;
        itemBtn.onClick.RemoveAllListeners();
        itemBtn.onClick.AddListener(() => action.Invoke(itemIndex));

    }
    public void SelectItem()
    {
        itemOutline.enabled = true;
        itemImage.color = itemSelectedColor;
        itemBtn.interactable = false;
    }
    public void DeselectItem()
    {
        itemOutline.enabled = false;
        itemImage.color = itemNotSelectedColor;
        itemBtn.interactable = true;
    }
    public void AnimateShakeItem()
    {
        //End all animation first
        transform.DOComplete();

        transform.DOShakePosition(1f, new Vector3(4f, 0, 0),10,0).SetEase(Ease.Linear);
    }

}
