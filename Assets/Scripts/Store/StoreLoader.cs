using UnityEngine;
using System.Collections;

using Microsoft.UnityPlugins;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class StoreLoader : MonoBehaviour {

    
    static bool useSimulator = true;
    static bool isSimulatorLicenseLoaded = false;
    static ListingInformation listingInformation;
    static LicenseInformation licenseInformation;  
    // Use this for initialization
     
    private static Dictionary<string, string> mappings = new Dictionary<string, string>()
    {
        { "Levels1030" , "com.sasogames.chomp.levels1030"  } ,
        { "Levels3050" , "com.sasogames.chomp.levels3060" } ,
        { "Levels60100", "com.sasogames.chomp.levels60100" } ,
        { "LevelsAll"  , "com.sasogames.chomp.levelsall" } ,
        { "Booster01"  , "com.sasogames.chomp.speedboost" },
        { "Booster02"  , "com.sasogames.chomp.10sfreeze" } ,
        { "Booster03"  , "com.sasogames.chomp.10xpoints" },
        { "Booster04"  , "com.sasogames.chomp.extralives" }
    }; 

    
    void OnSimulatorLicenseLoaded ( CallbackResponse cr )
    {
        if (cr.Status == CallbackStatus.Success)
        {
            Store.LoadListingInformation(OnListingInformationCompleted);
            isSimulatorLicenseLoaded = true;
        }
#if DEBUG
        else
            MessageBoxHelper.ShowModelessError("Simulator load failed"); 
#endif 
    }

void Start()
{

        if ( useSimulator && !isSimulatorLicenseLoaded)
        {                            
                string path = System.IO.Path.Combine(Application.streamingAssetsPath, "windowsstoreproxy.xml");
                // Use URI to clean up the path as streamingAssets uses // instead of \\ 
                Uri uri = new Uri(path); 
                Store.LoadLicenseXMLFile(OnSimulatorLicenseLoaded, uri.LocalPath);             
        } 
        else
        { 
            Store.LoadListingInformation(OnListingInformationCompleted);            
        }

        foreach (string s in mappings.Keys)
        {
            Button btn = GameObject.Find(s).GetComponent<Button>();             
            btn.interactable = false; 
            Text txt = btn.transform.FindChild("Text").GetComponent<Text>();
            txt.text = "";
        }
    }

    // Update is called once per frame
    void Update () {
	     
	}


    void OnListingInformationCompleted (CallbackResponse<ListingInformation> response )
    {
        if (  response.Status == CallbackStatus.Success)
        {
            listingInformation = response.Result;  
            PopulatePrices(listingInformation.ProductListings); 
        }
    }


    void PopulatePurchasedDurables(Dictionary<string, ProductLicense> licenses)
    {
        foreach ( ProductLicense lic in licenses.Values )
        {
            if (lic.IsActive )
            {
                GameObject go = GameObject.Find(GetUIObjectName(lic.ProductId));
                if (go != null)
                {
                    Button btn = go.GetComponent<Button>();
                    btn.interactable = false;
                    btn.GetComponentInChildren<Text>().text = Constants.Store_Owned; 
                }
            } 
        }
    }

    string GetUIObjectName ( string key )
    {
        foreach (var pair in mappings)
        {
            if (pair.Value == key)
                return pair.Key; 
        }
        Debug.Assert(false);
        return ""; 
    }

    string GetProductId ( string controlName )
    {
        return mappings[controlName]; 
    }

    void PopulatePrices ( Dictionary<string, ProductListing> listings )
    {           
        foreach ( var listing in listings )
        {
            Debug.Log(string.Format("{0}: {1}, {2}", listing.Key, listing.Value.Name, listing.Value.ProductId));

            string productId = listing.Key;
            licenseInformation = Store.GetLicenseInformation(); 
            if ( mappings.ContainsValue ( productId ))
            {
                GameObject go = GameObject.Find(GetUIObjectName(productId));
                if (go != null)
                {
                    Button btn = go.GetComponent<Button>();
                    if (btn != null)
                    {
                        Transform textObject = go.transform.FindChild("Text"); 
                        if ( textObject )
                        {
                            Text textElement = textObject.GetComponent<Text>(); 
                            textElement.text = listing.Value.FormattedPrice;

                            if (licenseInformation.ProductLicenses.ContainsKey(productId))
                            {
                                if (!licenseInformation.ProductLicenses[productId].IsActive)
                                {                                    
                                    btn.interactable = true;
                                }
                                else
                                {                                   
                                    btn.interactable = false;
                                    textElement.text = Constants.Display_Owned; 
                                }
                            }
                            else
                            {                               
                                btn.interactable = true ; 
                            }
                        }                         
                    }
                }                 
            }            
        }
    }

   

    public void OnDurablePurchase ( GameObject buttonClicked )
    {
            Consume(buttonClicked, false); 
    }


    void Consume ( GameObject go , bool isConsumable )
    {
        string productId = GetProductId(go.name);
        if (!string.IsNullOrEmpty(productId))
        {
            Store.RequestProductPurchase(productId, (response) =>
            {
                if (response.Status == CallbackStatus.Success)
                {
                    // response.Status just tells us if callback was successful. 
                    // The CallbackResponse tells us the actual Status as returned from store
                    // Check both 
                    if (response.Result.Status == ProductPurchaseStatus.Succeeded)
                    {
                        if (!isConsumable)
                            EnableLevels(go, productId);
                        else
                        {
                            FulfillConsumable(productId, response.Result.TransactionId.ToString());
                        }                       
                    }
                    else  
                    {
                        MessageBoxHelper.ShowModelessError( Constants.Display_PurchaseFailed ); 
                    }
                }
                else   
                {
                    MessageBoxHelper.ShowModelessError(Constants.Display_PurchaseFailed); 
                }
            });
        }
    }

    void EnableLevels ( GameObject go, string productId )
    {
        go.GetComponent<Button>().interactable = false;
        var textObject = go.GetComponentInChildren<Text>(); 
        if (textObject != null)
            textObject.text = Constants.Display_Owned; 

        MessageBoxHelper.ShowModelessMessage(Constants.Display_LevelPurchaseCompleted); 
    }

    void FulfillConsumable ( string productId , string transactionId )
    {
        Store.ReportConsumableFulfillment(productId,  new Guid(transactionId), OnReportConsumableFulfillmentFinished); 
    }

    void OnReportConsumableFulfillmentFinished(CallbackResponse<FulfillmentResult> result)
    {
        if (result.Status == CallbackStatus.Success)
        {
            MessageBoxHelper.ShowModelessMessage(Constants.Display_ConsumableFulfilled); 
        }
        else
        {
            MessageBoxHelper.ShowModelessError(Constants.Display_ErrorFulfillingConsumable); 
        }
    }

    public void OnConsumablePurchase (GameObject buttonClicked)
    {
        Consume(buttonClicked, true); 
    }


     

}
