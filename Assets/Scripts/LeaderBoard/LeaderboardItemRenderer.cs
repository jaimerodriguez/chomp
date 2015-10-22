using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeaderboardItemRenderer : MonoBehaviour {

#if UNITY_EDITOR
    public string designTimeRank = "1";
    public string designTimeGamertag = "Gamertag";
    public string designTimeValue = "20";
#endif 

    Text rankElement, gamertagElement, valueElement;

	void Awake()
	{
        rankElement = transform.FindChild("Rank").GetComponent<Text>();
        gamertagElement = transform.FindChild("Gamertag").GetComponent<Text>();
        valueElement = transform.FindChild("Value").GetComponent<Text>();

#if UNITY_EDITOR
       Initialize(designTimeRank, designTimeGamertag, designTimeValue); 
#endif 

	}

	// Use this for initialization
	void Start () {
	
	}

	public void Initialize(string rank, string gamertag, string value)
	{
		Debug.Log("Initialize");
        if (rankElement != null)
            rankElement.text = rank;
        if (gamertagElement != null)
            gamertagElement.text = gamertag;
        if (valueElement != null)
            valueElement.text = value;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
