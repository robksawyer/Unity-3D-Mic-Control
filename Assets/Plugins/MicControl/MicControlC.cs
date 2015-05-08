using UnityEngine;
using System.Collections;

//The GameObject requires a AudioSource component
[RequireComponent (typeof(AudioSource))]

public class MicControlC : MonoBehaviour
{

	private string selectedDevice;
	private int minFreq = 0;
	private bool micSelected = false;
	private bool recording = true;
	private float ListenerDistance;
	private Vector3 ListenerPosition;
	private bool focused = false;
	private bool Initialised = false;

	private	int position = 0;
	private	int sampleRate = 0;
	private	float frequency = 440;

	AudioSource audioSource;

	//if false the below will override and set the mic selected in the editor
	//Select the microphone you want to use (supported up to 6 to choose from). If the device has number 1 in the console, you should select default as it is the first defice to be found.
	public enum Devices
	{
		DefaultDevice,
		Second,
		Third,
		Fourth,
		Fifth,
		Sixth
	}

	public Devices InputDevice;
	public Transform audioListener;

	//The main entry point to the input signal
	public static float loudness = 0.0f;

	//The maximum amount of sample data that gets loaded in, best is to leave it on 256, unless you know what you are doing. A higher number gives more accuracy but
	//lowers performance allot, it is best to leave it at 256.
	public int amountSamples = 256;

	public int maxFreq = 44100;

	public float sensitivity = 0.4f;
	public float sourceVolume = 100f;

	//if true a menu will apear ingame with all the microphones
	[HideInInspector]
	public bool
		SelectIngame = false;

	[HideInInspector]
	public bool
		ThreeD = false;

	[HideInInspector]
	public float
		VolumeFallOff = 1.0f;

	[HideInInspector]
	public float
		PanThreshold = 1.0f;

	[HideInInspector]
	public bool
		Mute = true;

	[HideInInspector]
	public bool
		debug = false;

	[HideInInspector]
	public bool
		ShowDeviceName = false;

	void Awake ()
	{
		audioSource = GetComponent<AudioSource> ();
	}

	IEnumerator Start ()
	{
		// Request permission to use both webcam and microphone.
		if (Application.isWebPlayer) {
			yield return Application.RequestUserAuthorization (UserAuthorization.Microphone);
			if (Application.HasUserAuthorization (UserAuthorization.Microphone)) {
				InitMic ();
				Initialised = true;
			} else {
				Debug.Log ("Unable to access the microphone.");
				return false;
			}
		} else {
			InitMic ();
			Initialised = true;
		}

	}


	/*
	 * Apply the mic input data stream to a float.
	 */
	void  Update ()
	{
		//pause everything when not focused on the app and then re-initialize.
		if (!focused) {
			StopMicrophone ();
			Initialised = false;
		}
		if (!Application.isPlaying) {
			//don't stop the microphone if you are clicking inside the editor
			StopMicrophone ();
			Initialised = false;
		} else {
			if (!Initialised) {
				InitMic ();
				Initialised = true;
			}
		}


		if (Microphone.IsRecording (selectedDevice)) {
			loudness = GetDataStream () * sensitivity * (sourceVolume / 10);

		}
		if (debug) {
			Debug.Log (loudness);
		}

		//the source volume
		if (sourceVolume > 100) {
			sourceVolume = 100;
		}

		if (sourceVolume < 0) {
			sourceVolume = 0;
		}

		//when 3D is enabled adjust the volume based on distance.
		if (ThreeD) {

			ListenerDistance = Vector3.Distance (transform.position, audioListener.position);
			ListenerPosition = audioListener.InverseTransformPoint (transform.position);

			audioSource.volume = (sourceVolume / 100 / (ListenerDistance * VolumeFallOff));
			audioSource.panStereo = (ListenerPosition.x / PanThreshold);

		} else {
			audioSource.volume = (sourceVolume / 100);
		}


	}

	float GetDataStream ()
	{
		if (Microphone.IsRecording (selectedDevice)) {
			float[] dataStream = new float[amountSamples]; //Converts to a float
			float audioValue = 0.0f;
			audioSource.GetOutputData (dataStream, 0);

			foreach (int i in dataStream) {
				audioValue += Mathf.Abs (i);
			}
			return audioValue / amountSamples;
		} else {
			return 0.0f;
		}
	}

	/*
	 * select device ingame
	*/
	void OnGUI ()
	{
		if (SelectIngame == true) {
			if (Microphone.devices.Length > 0 && micSelected == false)//If there is more than one device, choose one.
				for (int i = 0; i < Microphone.devices.Length; ++i)
					if (GUI.Button (new Rect (400, 100 + (110 * i), 300, 100), Microphone.devices [i].ToString ())) {
						StopMicrophone ();
						selectedDevice = Microphone.devices [i].ToString ();
						GetMicCaps ();
						StartMicrophone ();
						micSelected = true;

					}

			if (Microphone.devices.Length < 1 && micSelected == false) {//If there is only 1 decive make it default
				selectedDevice = Microphone.devices [0].ToString ();
				GetMicCaps ();
				micSelected = true;
			}
		}

	}

	/*
	 *
	 * Initialize microphone!
	 *
	 */
	private void InitMic ()
	{
		//select audio source
		if (!audioSource) {
			audioSource = GetComponent<AudioSource> ();
		}

		//only Initialize microphone if a device is detected
		if (Microphone.devices.Length >= 0) {

			int i = 0;
			//count amount of devices connected
			foreach (string device in Microphone.devices) {
				i++;
				if (ShowDeviceName) {
					Debug.Log ("Devices number " + i + " Name" + "=" + device);
				}
			}

			if (SelectIngame == false) {
				//select the device if possible else give error
				if (InputDevice == Devices.DefaultDevice) {
					if (i >= 1) {
						selectedDevice = Microphone.devices [0];
					} else {
						Debug.LogError ("No device detected on this slot. Check input connection");
					}

				}

				if (InputDevice == Devices.Second) {
					if (i >= 2) {
						selectedDevice = Microphone.devices [1];
					} else {
						Debug.LogError ("No device detected on this slot. Check input connection");
					}

				}

				if (InputDevice == Devices.Third) {
					if (i >= 3) {
						selectedDevice = Microphone.devices [2];
					} else {
						Debug.LogError ("No device detected on this slot. Check input connection");
						return;
					}
				}


				if (InputDevice == Devices.Fourth) {
					if (i >= 4) {
						selectedDevice = Microphone.devices [2];
					} else {
						Debug.LogError ("No device detected on this slot. Check input connection");
						return;
					}
				}
				if (InputDevice == Devices.Fifth) {
					if (i >= 5) {
						selectedDevice = Microphone.devices [2];
					} else {
						Debug.LogError ("No device detected on this slot. Check input connection");
						return;
					}
				}

				if (InputDevice == Devices.Sixth) {
					if (i >= 6) {
						selectedDevice = Microphone.devices [2];
					} else {
						Debug.LogError ("No device detected on this slot. Check input connection");
						return;
					}
				}

			}

			//detect the selected microphone
			audioSource.clip = Microphone.Start (selectedDevice, true, 10, maxFreq);

			//loop the playing of the recording so it will be realtime
			audioSource.loop = true;
			//if you only need the data stream values  check Mute, if you want to hear yourself ingame don't check Mute.
			audioSource.mute = Mute;

			//don't do anything until the microphone started up
			while (!(Microphone.GetPosition(selectedDevice) > 0)) {
				if (debug) {
					Debug.Log ("Awaiting connection");
				}
			}
			if (debug) {
				Debug.Log ("Connected");
			}

			//Put the clip on play so the data stream gets ingame on realtime
			audioSource.Play ();
			recording = true;
		}

	}
	/* End Microphone Initialization */



	/*
	 * For the above control the mic start or stop
	 */
	public void  StartMicrophone ()
	{
		audioSource.clip = Microphone.Start (selectedDevice, true, 10, maxFreq);//Starts recording
		while (!(Microphone.GetPosition(selectedDevice) > 0)) {
		} // Wait until the recording has started
		audioSource.Play (); // Play the audio source!

	}

	public void  StopMicrophone ()
	{
		audioSource.Stop ();//Stops the audio
		Microphone.End (selectedDevice);//Stops the recording of the device

	}

	void  GetMicCaps ()
	{
		Microphone.GetDeviceCaps (selectedDevice, out minFreq, out maxFreq); //Gets the frequency of the device

		//These 2 lines of code are mainly for windows computers
		if ((minFreq + maxFreq) == 0) {
			maxFreq = 44100;
		}
	}


	/*
	 * Create a gui button in another script that calls to this script
	 */
	public void  MicDeviceGUI (float left, float top, float width, float height, float buttonSpaceTop, float buttonSpaceLeft)
	{
		if (Microphone.devices.Length > 1 && micSelected == false)//If there is more than one device, choose one.
			for (int i=0; i < Microphone.devices.Length; ++i)
				if (GUI.Button (new Rect (left + (buttonSpaceLeft * i), top + (buttonSpaceTop * i), width, height), Microphone.devices [i].ToString ())) {
					StopMicrophone ();
					selectedDevice = Microphone.devices [i].ToString ();
					GetMicCaps ();
					StartMicrophone ();
					micSelected = true;
				}
		if (Microphone.devices.Length < 2 && micSelected == false) {//If there is only 1 decive make it default
			selectedDevice = Microphone.devices [0].ToString ();
			GetMicCaps ();
			micSelected = true;
		}
	}

	/*
	 * flush the date through a custom created audio clip, this controls the data flow of that clip
	 * Creates a 1 sec long audioclip, with a 440hz sinoid
	 */
	void  OnAudioRead (float[] data)
	{
		for (int count = 0; count < data.Length; count++) {
			data [count] = Mathf.Sign (Mathf.Sin (2 * Mathf.PI * frequency * position / sampleRate));
			position++;
		}
	}

	void  OnAudioSetPosition (int newPosition)
	{
		position = newPosition;
	}

	/*
	 * Start or stop the script from running when the state is paused or not.
	 */
	void  OnApplicationFocus (bool focus)
	{
		focused = focus;
	}

	void  OnApplicationPause (bool focus)
	{
		focused = focus;
	}

}