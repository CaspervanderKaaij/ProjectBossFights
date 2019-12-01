using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void Save(SaveManager data){

        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/willpower.savedata";
        FileStream stream = new FileStream(path,FileMode.Create);

        SaveStuff stuff = new SaveStuff(data);

        formatter.Serialize(stream,stuff);
        stream.Close();
    }

    public static SaveStuff LoadStuff(){
        string path = Application.persistentDataPath + "/willpower.savedata";
        if(File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path,FileMode.Open);
            SaveStuff stuff = formatter.Deserialize(stream) as SaveStuff;
            stream.Close();
            return stuff;
        } else {
            Debug.LogError("Save file not found in" + path);
            return null;
        }
    }
}
