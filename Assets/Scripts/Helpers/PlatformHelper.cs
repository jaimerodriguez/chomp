using UnityEngine;
using System.Collections;


#if UNITY_WSA_10_0 && NETFX_CORE 
using Windows.UI.ViewManagement;
#endif


public class PlatformHelper {

#if DEBUG
    static bool forceJoystick = false; 
#endif 

public static bool UseJoystick
{
    get
    {
        return ( 
#if DEBUG
                forceJoystick || 
#endif
            //for now, only phone is WSAPlayerARM 
            IsWindowsMobile  ||
            IsWindows10UserInteractiveModeTouch
            ) ;  
                 
    }
}

public static bool IsWindowsMobile
{
    get
    {
#if UNITY_WSA_10_0 && NETFX_CORE 
        return Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent 
                ("Windows.Phone.PhoneContract", 1); 
#else
            return false; 
#endif

    }
}

 
public static bool IsWindows10UserInteractiveModeTouch
{
    get
    {
        bool isInTouchMode = false;
#if UNITY_WSA_10_0 && NETFX_CORE && !UNITY_5_2_1
        UnityEngine.WSA.Application.InvokeOnUIThread(() =>
        {
            isInTouchMode = UIViewSettings.GetForCurrentView().UserInteractionMode == 
                UserInteractionMode.Touch;
        }, true);         
#endif
            return isInTouchMode; 
    }
}
 

}
