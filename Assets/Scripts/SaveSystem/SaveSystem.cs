using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    // Save File
    public static void Save(PlayerData data)
    {
        //檔案路徑
        Directory.CreateDirectory(Application.persistentDataPath + "/SaveFile");
        string path = Application.persistentDataPath + "/SaveFile/save.data";
        FileStream fs = new FileStream(path, FileMode.Create);

        //存成二進位
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(fs, data);
        Debug.Log("Save");
        //關閉檔案
        fs.Close();
    }

    // Load File
    public static PlayerData Load()
    {
        //確認檔案是否存在
        string path = Application.persistentDataPath + "/SaveFile/save.data";
        if (File.Exists(path))
        {
            //以二進位讀檔
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();

            //轉換資料型別
            PlayerData data = formatter.Deserialize(fs) as PlayerData;
            Debug.Log("Load");
            //關閉檔案
            fs.Close();
            return data;
        }
        else
        { Debug.Log("Can't Find File"); }
        return new PlayerData();
    }
}
