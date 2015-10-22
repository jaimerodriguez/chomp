using Microsoft.UnityPlugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class NavigationHelpers
 {
    public static void GoBack ()
    {      
        int previous = PlayerPrefs.GetInt("previousLevel");
        Application.LoadLevel(previous);      
    }

	public static IEnumerator GetSpriteFromUrl(string url, Action<Sprite> cb)
	{
		Debug.Log("Downloading: " + url);

		WWW www = new WWW(url);

		yield return www;

		if(www.texture != null)
		{
			Sprite sprite = new Sprite();
			sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
			cb(sprite);
		}

		yield return null;
	}
 }


 
public static class MessageBoxHelper
{
    public static void ShowModelessMessage(string message)
    {
        ShowModeless(message);
    }

    public static void ShowModelessError ( string message )
    {
        string errorPath = System.IO.Path.Combine(Application.streamingAssetsPath, Constants.Assets_ErrorIcon); 
        ShowModelessWithImage(message, errorPath); 
    }

    static void ShowModeless ( string message )
    {
#if UNITY_WINRT 
        try 
        { 
            Microsoft.UnityPlugins.Toasts.ShowToast(ToastTemplateType.ToastText01,
                       new string[] { message }, string.Empty);
        } catch  ( Exception /*ex*/ ) 
        { 
            //TODO: Log to server 
        } 
#endif 
    }


    static void ShowModelessWithImage(string message , string image )
    {
#if UNITY_WINRT 
        try 
        { 
            Microsoft.UnityPlugins.Toasts.ShowToast(ToastTemplateType.ToastImageAndText01 ,
                       new string[] { message }, image);
        } catch ( Exception /*ex*/ ) 
        { 
            //TODO: Log to server 
        } 
#endif 
    }

}
 
