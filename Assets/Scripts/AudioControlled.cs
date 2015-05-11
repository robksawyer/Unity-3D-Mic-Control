using UnityEngine;
using System.Collections;

public class AudioControlled : MonoBehaviour
{

	public MicControlC micControl;
	public float speed = 0.1f;

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		if (MicControlC.loudness > 0.0f) {
			Vector3 toRotation = new Vector3 (1.0f, MicControlC.loudness, 1.0f);
			Quaternion.Lerp(transform.rotation, toRotation,Time.time * speed);
			//Debug.Log (MicControlC.loudness);
		}
	}
}
