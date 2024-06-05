using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//Character Save
[System.Serializable]
public class CharacterShopData
{
    public List<int>purchasedCharactersIndexes = new List<int>();


}

//Save Player Data
[System.Serializable]
public class PlayerData 
{
    public int totalMoney;
    public int selectedCharacterIndex = 0;
    static CharacterShopData characterShopData = new CharacterShopData();

    public  PlayerData(GameManager gameManager)
    {
        totalMoney = gameManager.totalMoney;
        selectedCharacterIndex = gameManager.selectedCharacterIndex;
    }

}
