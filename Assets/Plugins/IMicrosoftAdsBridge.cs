using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

namespace Microsoft.UnityPlugins
{
    public enum InterstitialAdState
    {
        NotReady = 0,
        Ready = 1,
        Showing = 2,
        Closed = 3
    }

    public enum AdType
    {
        Video = 0  
    }
     
    public enum  AdCallback 
    {
        Ready,
        Completed, 
        Error,
        Cancelled  
        
    }
    public interface IInterstittialAd : IDisposable 
    {

        void AddCallback(AdCallback type , Action<object> cb );
        void ClearCallbacks(AdCallback type); 

        void RequestAndShow(string appId, string adUnitId);

        void Request(string appId, string adUnitId, AdType type); 

        void Show();
        InterstitialAdState State { get; }
    }

    public interface IInterstitialAdFactory
    {
        IInterstittialAd CreateAd();

        IInterstittialAd CreateAd(string appId, string adUnitId, AdType type, 
            Action<object> readyCallback, Action<object> completedCallback, Action<object> cancelledCallback, Action<object> errorCallback     
            ); 
    }
    public class MicrosoftAdsBridge
    {
        public static IInterstitialAdFactory InterstitialAdFactory { get; set;  }
    }


#if UNITY_EDITOR
    public class EditorAdFactory : IInterstitialAdFactory
    {
        public IInterstittialAd CreateAd()
        {
            return new EditorInterstitialAd(); 
        }

        public IInterstittialAd CreateAd(string appId, string adUnitId, AdType type, Action<object> readyCallback, Action<object> completedCallback, Action<object> cancelledCallback, Action<object> errorCallback)
        {
            throw new NotImplementedException("Being Lazy"); 
        }
    }

    public class EditorInterstitialAd : IInterstittialAd
    {
        private Action<object> cbReady, cbCompleted, cbError, cbCancelled;
        private bool hasPendingRequests = false;
        private bool playNextReady = false; 
        private InterstitialAdState state = InterstitialAdState.NotReady;
        public MonoBehaviour syncMonoBehaviour;
        public UnityEngine.Canvas canvas;  
         
        public InterstitialAdState State
        {
            get { return state;  }
        }

        void StartCoroutine(IEnumerator e)
        {
            if (syncMonoBehaviour != null)
                syncMonoBehaviour.StartCoroutine(e); 
        }


        public void AddCallback(AdCallback type, Action<object> cb)
        {
            switch (type)
            {
                case AdCallback.Ready:
                     cbReady = cb;                       
                    break;
                case AdCallback.Completed:
                    cbCompleted = cb; 
                    break;
                case AdCallback.Cancelled:
                    cbCancelled = cb; 
                    break;
                case AdCallback.Error:
                    cbError = cb; 
                    break;
            }
        }

        public void ClearCallbacks(AdCallback type)
        {
            Action<object> cb = null ; 
            switch (type)
            {
                case AdCallback.Ready:
                    cbReady = cb;
                    break;
                case AdCallback.Completed:
                    cbCompleted = cb;
                    break;
                case AdCallback.Cancelled:
                    cbCancelled = cb;
                    break;
                case AdCallback.Error:
                    cbError = cb;
                    break;
            }
        }

        public void Request(string appId, string adUnitId, AdType type)
        {
            hasPendingRequests = true;
            StartCoroutine(PrepareRequest()); 
        }

        IEnumerator PrepareRequest()
        {
            yield return new WaitForSeconds(1f);
            hasPendingRequests = false; 
            state = InterstitialAdState.Ready;
            Fire( AdCallback.Ready );
            if (playNextReady)
            {
                playNextReady = false;
                Show(); 
            }
        }

        
        void Fire(AdCallback cb)
        {
            switch (cb)
            {
                case AdCallback.Ready:
                    state = InterstitialAdState.Ready;
                    if (cbReady != null)
                        cbReady(null);                     
                    break;
                case AdCallback.Completed:
                    state = InterstitialAdState.Closed;
                    if (cbCompleted != null)
                        cbCompleted(null);
                    
                    break;
                case AdCallback.Cancelled:
                    state = InterstitialAdState.Closed; 
                    if (cbCancelled != null)
                        cbCancelled(null);
                    break;
                case AdCallback.Error:
                    state = InterstitialAdState.Closed; 
                    if (cbError != null)
                        cbError(null);
                    break; 
            }
        }



        public void RequestAndShow(string appId, string adUnitId)
        {
            playNextReady = true; 
            Request( appId, adUnitId, AdType.Video); 
             
        }

#if UNITY_EDITOR
        private bool debugOverrideShow = true;
#endif 
        public void Show()
        {
            if (state == InterstitialAdState.Ready || debugOverrideShow )
            {
                state = InterstitialAdState.Showing;  
                StartCoroutine(DoShow(10f));
            }        
            else
                throw new InvalidOperationException("Not Ready");
        }

        private IEnumerator DoShow( float delay)
        {
            if (canvas != null)
            {
                GameObject go = new GameObject();
                go.AddComponent<RectTransform>();
                var image = go.AddComponent<Image>();
                image.color = Color.red;
                RectTransform rt = image.rectTransform;                
                image.transform.SetParent(canvas.transform); 
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 400);
                rt.localPosition = Vector3.zero;

                go = new GameObject();
                go.AddComponent<RectTransform>();
                var text = go.AddComponent<Text>();
                text.color = Color.white;
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 48; 
                text.transform.SetParent(canvas.transform);
                rt = text.rectTransform;                
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200);
                rt.localPosition = Vector3.zero;
                text.text = "This is one ugly ad"; 
                yield return new WaitForSeconds(delay);
                GameObject.Destroy(image);
                GameObject.Destroy(text);
            }
            else 
                yield return new WaitForSeconds(delay);


            Fire(AdCallback.Completed); 
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EditorInterstitialAd() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
#endif 

}
