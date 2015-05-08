/*using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

[CustomEditor(typeof(MicControlC))]
public class MicControlImporter : Editor
{
	//apply only to this audio source
	MicControlC CurrentMicController;
	
	void  OnInspectorGUI ()
	{
		CurrentMicController = (MicControlC)target;

		//Redirect 3D toggle
		CurrentMicController.GetComponent<MicControlC> ().ThreeD = GUILayout.Toggle (CurrentMicController.ThreeD, new GUIContent ("3D sound", "Should the streamed audio be a 3D sound? (Only enable this if you are using the controller to stream sound (VOIP) "));
		
	}
}*/