using UnityEngine;
using System.Collections;

//The GameObject requires a AudioSource component
[RequireComponent (typeof(AudioSource))]

public class MicControlC : MonoBehaviour
{
	
	private string selectedDevice;
	private int minFreq = 0;
	private float ListenerDistance;
	private Vector3 ListenerPosition;
	private bool micSelected = false;
	private bool recording = true;
	private bool focused = false;
	private bool Initialised = false;

	private float[] freqData;
	private int nSamples = 1024;
	private float fMax;
	
	private	int position = 0;
	private	int sampleRate = 0;
	private	float frequency = 440;
	private int fallbackMaxFreq = 44100;
	
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
	
	//The maximum amount of sample data that gets loaded in, best is to leave it on 256, unless you know what you are doing. 
	//A higher number gives more accuracy but lowers performance alot, it is best to leave it at 256.
	public int amountSamples = 256;
	
	public int maxFreq = 44100;//48000;
	
	public float sensitivity = 0.4f;
	public float sourceVolume = 100f;
	
	//if true a menu will apear ingame with all the microphones
	[HideInInspector()]
	public bool
		SelectIngame = false;
	
	[HideInInspector()]
	public bool
		ThreeD = false;
	
	[HideInInspector()]
	public float
		VolumeFallOff = 1.0f;
	
	[HideInInspector()]
	public float
		PanThreshold = 1.0f;
	
	[HideInInspector()]
	public bool
		Mute = true;
	
	[HideInInspector()]
	public bool
		debug = false;
	
	[HideInInspector()]
	public bool
		ShowDeviceName = false;

	void Start ()
	{
		//select audio source
		if (!audioSource) {
			audioSource = GetComponent<AudioSource> ();
			//audioSource.mute = true;
			audioSource.playOnAwake = false;
		}

		InitMic ();
		Initialised = true;
	}
	
	
	/*
	 * Apply the mic input data stream to a float.
	 */
	void Update ()
	{
		//pause everything when not focused on the app and then re-initialize.
//		if (!focused) {
//			StopMicrophone ();
//			Initialised = false;
//		}
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
		
		audioSource.mute = Mute;
	}

	/*
	 * The main data stream from the microphone
	 */
	float GetDataStream ()
	{
		if (Microphone.IsRecording (selectedDevice)) {
			float[] samples = new float[amountSamples]; //Converts to a float
			//float[] samples = new float[audioSource.clip.samples * audioSource.clip.channels];

			audioSource.clip.GetData (samples, 0);
			return Sum (samples) / amountSamples;
		} else {
			Debug.Log ("The active microphone is not recording.");
			return 0.0f;
		}
	}


	private float Sum (params float[] samples)
	{
		float result = 0.0f;
		for (int i = 0; i < samples.Length; i++) {
			result += Mathf.Abs (samples [i]);
		}
		return result;
	}

	private float Average (params float[] samples)
	{
		float sum = Sum (samples);
		float result = (float)sum / samples.Length;
		return result;
	}
	
	/*
	 * select device ingame
	*/
	void OnGUI ()
	{
		if (SelectIngame == true) {
			//If there is more than one device, choose one.
			if (Microphone.devices.Length > 0 && micSelected == false) {
				for (int i = 0; i < Microphone.devices.Length; ++i) {
					if (GUI.Button (new Rect (400, 100 + (110 * i), 300, 100), Microphone.devices [i].ToString ())) {
						StopMicrophone ();
						selectedDevice = Microphone.devices [i].ToString ();
						GetMicCaps ();
						StartMicrophone ();
						micSelected = true;
					
					}
				}
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
					}
				}
				
				
				if (InputDevice == Devices.Fourth) {
					if (i >= 4) {
						selectedDevice = Microphone.devices [2];
					} else {
						Debug.LogError ("No device detected on this slot. Check input connection");
					}
				}
				if (InputDevice == Devices.Fifth) {
					if (i >= 5) {
						selectedDevice = Microphone.devices [2];
					} else {
						Debug.LogError ("No device detected on this slot. Check input connection");
					}
				}
				
				if (InputDevice == Devices.Sixth) {
					if (i >= 6) {
						selectedDevice = Microphone.devices [2];
					} else {
						Debug.LogError ("No device detected on this slot. Check input connection");
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
				if (debug && Time.deltaTime >= 0.1f) {
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
	public void StartMicrophone ()
	{
		//Starts recording
		audioSource.clip = Microphone.Start (selectedDevice, true, 10, maxFreq);

		if (debug) {
			Debug.Log ("Selected device: " + selectedDevice);
		}

		// Wait until the recording has started
		while (!(Microphone.GetPosition(selectedDevice) > 0)) {
			if (debug) {
				Debug.Log ("Waiting on recording to start...");
			}
		} 

		if (debug) {
			Debug.Log ("Playing the recorded audio...");
		}
		// Play the audio recording
		audioSource.Play (); 	
	}
	
	public void StopMicrophone ()
	{
		if (debug) {
			Debug.Log ("Stopping the microphone...");
		}

		//Stops the audio
		audioSource.Stop ();

		//Stops the recording of the device
		Microphone.End (selectedDevice);
		
	}
	
	void GetMicCaps ()
	{
		//Gets the frequency of the device
		Microphone.GetDeviceCaps (selectedDevice, out minFreq, out maxFreq);
		
		//These 2 lines of code are mainly for windows computers
		if ((minFreq + maxFreq) == 0) {
			maxFreq = fallbackMaxFreq;
		}
	}
	
	
	/*
	 * Create a gui button in another script that calls to this script
	 */
	public void MicDeviceGUI (float left, float top, float width, float height, float buttonSpaceTop, float buttonSpaceLeft)
	{
		//If there is more than one device, choose one.
		if (Microphone.devices.Length > 1 && micSelected == false) {
			for (int i=0; i < Microphone.devices.Length; ++i) {
				if (GUI.Button (new Rect (left + (buttonSpaceLeft * i), top + (buttonSpaceTop * i), width, height), Microphone.devices [i].ToString ())) {
					StopMicrophone ();
					selectedDevice = Microphone.devices [i].ToString ();
					GetMicCaps ();
					StartMicrophone ();
					micSelected = true;
				}
			}
		}

		//If there is only 1 microphone make it default
		if (Microphone.devices.Length < 2 && micSelected == false) {
			selectedDevice = Microphone.devices [0].ToString ();
			GetMicCaps ();
			micSelected = true;
		}
	}
	
	/*
	 * Flush the data through the custom created audio clip. This controls the data flow of that clip
	 * Creates a 1 sec long audioclip, with a 440hz sinoid
	 */
	void OnAudioFilterRead (float[] data, int channels)
	{
		//audioSource.spatialBlend
		/*for (int count = 0; count < data.Length; count++) {
			data [count] = Mathf.Sign (Mathf.Sin (2 * Mathf.PI * frequency * position / sampleRate));
			position++;
		}*/
		//Debug.Log ("OnAudioFilterRead(): " + data + " : " + channels);
		//Debug.Log (data);
	}

	void PCMReaderCallback (float[] data)
	{
		if (debug) {
			Debug.Log ("PCMReaderCallback()");
			Debug.Log (data);
			Debug.Log ("-----");
		}
	}

	void PCMSetPositionCallback (int newPosition)
	{
		if (debug) {
			Debug.Log ("PCMSetPositionCallback()");
			Debug.Log (newPosition);
			Debug.Log ("=====");
		}
		position = newPosition;
	}
	
	/*
	 * Start or stop the script from running when the state is paused or not.
	 */
	void OnApplicationFocus (bool focus)
	{
		focused = focus;
	}
	
	void  OnApplicationPause (bool focus)
	{
		focused = focus;
	}

}