using UnityEngine;
using System.Collections;

public class AudioControlled : MonoBehaviour
{	

	public MicControlC micControl;

	// Use this for initialization
	void Start ()
	{
		micControl = GetComponent<MicControlC> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Vector3 newScale = new Vector3 (0f, MicControlC.loudness, 0f);
		//transform.localScale (newScale);
	}
}
