Chomp
===== 
A simple game to showcase Windows 10 platform integration from within a Unity
game.  Demonstrates:

+ Windows Store integration, via Microsoft’s [Windows Store plugin
](<http://microsoft.github.io/unityplugins/store/>)for Unity
+ Live tiles, and roaming settings, via the Windows 10 [Core
plugin](<http://microsoft.github.io/unityplugins/core/>)
+ Interstitial videos from Microsoft ads
+ Windows 10 APIs for platform detection  

 
## Building Chomp     

### What you will need 
Since Chomp targets Windows 10, you need:     

+ [Unity 5.2.1p3 or later](http://unity3d.com/unity/qa/patch-releases) 
+ [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads) (now installed by Unity)
+ [Optional] [Microsoft's Windows 10 and 8.1 Advertisment SDK](https://msdn.microsoft.com/en-us/library/mt313199(v=msads.30).aspx), if you want to try video interstitials     
   
      
      
  
### Building with no ads.    
Chomp ships as Unity source. The 3rd party plugins are already included, so just go into Unity's build settings dialog, and target Windows 10 Store.   

  
### Building with ads.          
To build chomp and use ads, just target out _ Win10 as the build folder from Unity
s
tldr; explanation on out _ Win10  Since  the Microsoft Ads SDK is platform specific and Unity does not like platform specific winmds, the ads implementation is a bridge that decouples the interface(in Assets\Plugins\IMicrosoftAdsBridge.cs) from actual implementation (in out_Win10\ChompU\MicrosoftAdsBridge.cs) so for this, I included a pre-generated project file (.csproj) that already includes this bridge file, one line of initialization in App.xaml.cs and the references to the ad SDK.    
   

### Feeedback & Status.   
Chomp was coded super rushed as a quick demonstrator. Use it as a guidance for implementing Windows 10 features in your game, but don't assume we tested it thoroughly.  



