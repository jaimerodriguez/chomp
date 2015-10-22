using UnityEngine;
using System.Collections;

public interface IDebugBridge   {

    object DoWork(object[] callparams ); 
     
}

public static class Bridge 
{
    public static IDebugBridge Instance { set; get;  }
}
