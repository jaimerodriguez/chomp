using UnityEngine;
using System.Collections;

public class LogCallbackListener
{
    static LogCallbackListener ()
    {

    }
    static bool listening = false;
    private static object sync = new object(); 
    public static void Start ()
    {
        lock (sync)
        {  
            Application.logMessageReceivedThreaded += Application_logMessageReceivedThreaded; ;
            listening = true; 
        }
    }

    private static void Application_logMessageReceivedThreaded(string condition, string stackTrace, LogType type)
    {
        Callback(condition, stackTrace, type); 
    }

    public static void Stop ()
    {
        lock (sync)
        {
            Application.logMessageReceivedThreaded -= Application_logMessageReceivedThreaded;
            listening = false; 
        }
    }

    

#if DEBUG
    public static bool stopAlways = false; 
#endif 
    public static void Callback(string condition, string stackTrace, LogType type) 
    { 
        switch (type)
        {
            case LogType.Exception:
#if DEBUG && NETFX_CORE 
             if ( stopAlways && System.Diagnostics.Debugger.IsAttached )
                {
                    System.Diagnostics.Debugger.Break();  
                }
#endif

                break; 
        }

        if (type != LogType.Log)
        {
            System.Diagnostics.Debug.WriteLine(string.Format
            ("{0}: {1}\n\r{2}", type, condition, stackTrace));
        }
        else 
            System.Diagnostics.Debug.WriteLine(condition);
         
    } 
}
