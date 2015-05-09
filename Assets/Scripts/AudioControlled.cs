using UnityEngine;
using System.Collections;

public class AudioControlled : MonoBehaviour
{

	public MicControlC micControl;

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		if (MicControlC.loudness > 0.0f) {
			Vector3 newScale = new Vector3 (1.0f, MicControlC.loudness, 1.0f);
			transform.localScale = newScale;
			//Debug.Log (MicControlC.loudness);
		}
	}
}
