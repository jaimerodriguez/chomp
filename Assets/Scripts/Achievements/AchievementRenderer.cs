using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class AchievementRenderer : MonoBehaviour {

#if UNITY_EDITOR
    public string designTimeName = "Name goes here" ;
    public string designTimeDescription = "Long description" ;
    public string designTimePoints = "20" ;
    public string designTimeImage = "Achievements/AchievementDots1"; 
      
#endif 
    Text nameTextElement, descriptionTextElement, pointsTextElement;
    Image achievementsImage; 
   
	// Use this for initialization
	void Awake () {
        achievementsImage = GetComponent<UnityEngine.UI.Image>();
        nameTextElement = transform.FindChild("AchievementNameText").GetComponent<Text>();
        descriptionTextElement= transform.FindChild("AchievementDescriptionText").GetComponent<Text>();
        pointsTextElement = transform.FindChild("AchievementPointsText").GetComponent<Text>();

#if UNITY_EDITOR
       Initialize(designTimeName, designTimeDescription, designTimePoints, designTimeImage); 
#endif 

    }

    public void Initialize ( string  name, string description, string points , string image )
    {
		Debug.Log("Initialize");
        if (nameTextElement != null)
            nameTextElement.text = name;
        if (descriptionTextElement != null)
            descriptionTextElement.text = description;
        if (pointsTextElement != null)
            pointsTextElement.text = points;
        if (achievementsImage != null )
			StartCoroutine(NavigationHelpers.GetSpriteFromUrl(image, AssignSprite));
    }

	private void AssignSprite(Sprite sprite)
	{
	    achievementsImage.sprite = sprite; 
		achievementsImage.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
