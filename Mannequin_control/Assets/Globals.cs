using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Globals 
{
    public static int Episode = 0;
    public static int Success = 0;
    public static int Fail = 0;
    private static Text txtDebug = null;

    public static void ScreenText()
    {
        if (txtDebug == null)
        {
            txtDebug = GameObject.Find("txtDebug").gameObject.GetComponent<Text>();
        }
        float SuccessPercent = (Success / (float)(Success + Fail)) * 100;
        if (txtDebug != null)
        {
            txtDebug.text = string.Format("Episode={0}, Success={1}, Fail={2} %{3}", Episode, Success, Fail, SuccessPercent.ToString("0"));
        }
    //    Debug.Log("Episode ="+Episode+","+"Success ="+Success+","+"Fail ="+Fail+","+"Success% ="+SuccessPercent);
    }
    
}
