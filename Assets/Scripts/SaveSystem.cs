using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//Static class can't be instantiate
public static class SaveSystem
{
    //State function
    //Call anywhere without instance
    //Save Player Money
    public static void SavePlayer(GameManager gameManager)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.txt";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(gameManager);

        formatter.Serialize(stream, data);
        stream.Close();
    }   

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.txt";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream (path, FileMode.Open);
            

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.Log("Save file not found"+ path);
            return null;
        }
    }

}
