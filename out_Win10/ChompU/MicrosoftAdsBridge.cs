﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UnityPlugins;
using UnityPlayer;

namespace ChompU
{
    internal class InterstitialAd : IInterstittialAd
    {

        private Microsoft.Advertising.WinRT.UI.InterstitialAd _ad;
        private Action<object> readyCallback;
        private Action<object> completedCallback;
        private Action<object> errorCallback;
        private Action<object> cancelledCallback;
        private bool showWhenReady = false;
        private bool hasPendingAdRequest = false;

        public InterstitialAd()  
        {
            _ad = new Microsoft.Advertising.WinRT.UI.InterstitialAd();
            _ad.AdReady += _ad_AdReady;
            _ad.Completed += _ad_Completed;
            _ad.Cancelled += _ad_Cancelled;
            _ad.ErrorOccurred += _ad_ErrorOccurred; 

        }

        

        public InterstitialAd(
            string appId, string adUnitId, AdType type, Action<object> readyCallback, Action<object> completedCallback,
            Action<object> cancelledCallback,
            Action<object> errorCallback)  : this( )
        {
            this.readyCallback = readyCallback;
            this.completedCallback = completedCallback;
            this.errorCallback = errorCallback;
            this.cancelledCallback = cancelledCallback; 

        }


        public InterstitialAdState State
        {
            get
            {
                return (InterstitialAdState) _ad.State;                
            }
        }

        public void AddCallback(AdCallback type, Action<object> cb)
        {
            if (_ad == null ) 
                throw new InvalidOperationException("No ad");
            switch (type)
            {
                case AdCallback.Cancelled:
                    cancelledCallback = cb; 
                    break; 
                case AdCallback.Completed:
                    completedCallback = cb; 
                    break; 
                case AdCallback.Error:
                    errorCallback = cb; 
                    break ; 
                case AdCallback.Ready:
                    readyCallback = cb; 
                   break ;                      
            }
        }

        private void _ad_AdReady(object sender, object e)
        {
            hasPendingAdRequest = false; 
            Marshal(this.readyCallback, e);
            if (showWhenReady)
                Show(); 
        }

        private void _ad_ErrorOccurred(object sender, Microsoft.Advertising.WinRT.UI.AdErrorEventArgs e)
        {
            Marshal(this.errorCallback, e);
        }

        private void _ad_Completed(object sender, object e)
        {
            Marshal(this.completedCallback, e); 
        }

        private void _ad_Cancelled(object sender, object e)
        {
            Marshal(this.cancelledCallback, e); 
        }

        void Marshal(Action<object> cb, object e)
        {
            if ( cb != null )
            {  
             AppCallbacks.Instance.InvokeOnAppThread( () => { cb(e);  } , true );
            }
        }
        public void ClearCallbacks(AdCallback type)
        {
            switch (type)
            {
                case AdCallback.Cancelled:
                    cancelledCallback = null;  
                    break;
                case AdCallback.Completed:
                    completedCallback = null; 
                    break;
                case AdCallback.Error:
                    errorCallback = null; 
                    break;
                case AdCallback.Ready:
                    readyCallback = null; 
                    break;
            }
        }
 

        public void Dispose()
        {
            ClearCallbacks(AdCallback.Cancelled);
            ClearCallbacks(AdCallback.Error); 
            ClearCallbacks(AdCallback.Completed);
            ClearCallbacks(AdCallback.Ready);
            this._ad = null; 
        }

        public void Request(string appId, string adUnitId, AdType type)
        {
            hasPendingAdRequest = true;
            AppCallbacks.Instance.InvokeOnUIThread(() =>
            {
                try
                {
                    _ad.RequestAd((Microsoft.Advertising.WinRT.UI.AdType) type, appId, adUnitId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    hasPendingAdRequest = false; 
                }
            }, true); 
        }

        public void RequestAndShow(string appId, string adUnitId)
        {
            showWhenReady = true; 
            Request( appId, adUnitId, AdType.Video);
        }

 
        public void Show()
        {
            if (_ad.State == Microsoft.Advertising.WinRT.UI.InterstitialAdState.Ready)
            {
                showWhenReady = false;
                AppCallbacks.Instance.InvokeOnUIThread(() =>
                {
                    _ad.Show();
                }, true );                  
            } 
            else if (_ad.State == Microsoft.Advertising.WinRT.UI.InterstitialAdState.NotReady &&
                     hasPendingAdRequest)
            {
                showWhenReady = true; 
            }  
        }
    }

    internal class MicrosoftAdsFactory : Microsoft.UnityPlugins.IInterstitialAdFactory
    {
        public IInterstittialAd CreateAd()
        {
            return new InterstitialAd(); 
        }

        public IInterstittialAd CreateAd(string appId, string adUnitId, AdType type, Action<object> readyCallback, Action<object> completedCallback, Action<object> cancelledCallback, Action<object> errorCallback)
        {
            InterstitialAd ad = new InterstitialAd (
                appId, adUnitId, type, readyCallback, completedCallback,  cancelledCallback , errorCallback );

            return ad;  
        }
    }
}
