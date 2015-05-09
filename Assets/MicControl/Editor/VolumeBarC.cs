using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(MicControlC))]
public class VolumeBarC : Editor
{
		
	MicControlC ListenToMic;

	/////////////////////////////////////////////////////////////////////////////////////////////////		
	public override void OnInspectorGUI ()
	{
		ListenToMic = (MicControlC)target;

		float micInputValue = MicControlC.loudness;
		ProgressBar (micInputValue, "Loudness");

		//show other variables
		
		//Redirect 3D toggle
		ListenToMic.ThreeD = GUILayout.Toggle (ListenToMic.ThreeD, new GUIContent ("3D sound", "Should the streamed audio be a 3D sound? (Only enable this if you are using the controller to stream sound (VOIP) "));

		//when 3D audio is enabled show the fall off settings
		if (ListenToMic.ThreeD) {
			ListenToMic.VolumeFallOff = EditorGUILayout.FloatField (new GUIContent ("Volume falloff", "Set the rate at wich audio volume gets lowered. A lower value will have a slower falloff and thus hearable from a greater distance, while a higher value will make the audio degrate faster and dissapear from a shorter distance"), ListenToMic.VolumeFallOff);
			ListenToMic.PanThreshold = EditorGUILayout.FloatField (new GUIContent ("PanThreshold", "Set the rate at wich audio PanThreshold gets switched between left or right ear. A lower value will have a faster transition and thus a faster switch, while a higher value will make the transition slower and smoothly switch between the ears. Don't go to smooth though as this will turn your audio to mono channel"), ListenToMic.PanThreshold);
		}
		
		//Redirect select ingame
		ListenToMic.SelectIngame = GUILayout.Toggle (ListenToMic.SelectIngame, new GUIContent ("Select in game", "select the audio source through a GUI ingame"));
		
		//Redirect Mute ingame
		ListenToMic.Mute = GUILayout.Toggle (ListenToMic.Mute, new GUIContent ("Mute", "when dissabled you can listen to a playback of the microphone"));

		//Redirect debug ingame
		ListenToMic.debug = GUILayout.Toggle (ListenToMic.debug, new GUIContent ("Debug", "This will write the gathered Loudness value to the console during playmode. This is handy if you want if statements to listen at a specific value."));
		
		//Redirect ShozDeviceName ingame
		ListenToMic.ShowDeviceName = GUILayout.Toggle (ListenToMic.ShowDeviceName, new GUIContent ("Show Device name(s)", "When selected all detected devices will be written to the console during play mode"));

		EditorUtility.SetDirty (target);
		
		// Show default inspector property editor
		DrawDefaultInspector ();
	}

	// Custom GUILayout progress bar.
	void ProgressBar (float value, string label)
	{
		// Get a rect for the progress bar using the same margins as a textfield:
		Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
		EditorGUI.ProgressBar (rect, value, label);
		EditorGUILayout.Space ();
	}
}