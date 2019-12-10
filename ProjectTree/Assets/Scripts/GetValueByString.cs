using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GetValueByString : MonoBehaviour {
    public static float GetFloatValue (string s, Component comp) {
        return (float) comp.GetType ().GetField (s).GetValue (comp);
    }
    public static int GetIntValue (string s, Component comp) {
        return (int) comp.GetType ().GetField (s).GetValue (comp);
    }
    public static string GetStringValue (string s, Component comp) {
        return (string) comp.GetType ().GetField (s).GetValue (comp);
    }
}