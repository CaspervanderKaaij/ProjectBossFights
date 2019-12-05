using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem {
    public static void Save (SaveStuff data) {

        BinaryFormatter formatter = new BinaryFormatter ();

        string path = Application.persistentDataPath + "/willpower.savedata";
        FileStream stream = new FileStream (path, FileMode.Create);

        //SaveStuff stuff = new SaveStuff();//if it goes wrong from now, change SaveStuff data back to SaveManager data, and remove the */'s in SaveStuff

        formatter.Serialize (stream, data);
        stream.Close ();
        Debug.Log ("save complete");
    }

    public static SaveStuff LoadStuff () {
        string path = Application.persistentDataPath + "/willpower.savedata";
        if (File.Exists (path)) {
            BinaryFormatter formatter = new BinaryFormatter ();
            FileStream stream = new FileStream (path, FileMode.Open);
            SaveStuff stuff = formatter.Deserialize (stream) as SaveStuff;
            stream.Close ();
            return stuff;
        } else {
            Debug.Log ("Save file not found in" + path + " . Create new save file");
            SaveStuff stuff = new SaveStuff ();
            Save(stuff);
            return stuff;
        }
    }
}