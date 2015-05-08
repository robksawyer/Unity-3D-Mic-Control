-----------------------------------------------------
Original source code (java) created by Mark Duisters.
-----------------------------------------------------


//This javascript contains the original source code a varient in C# exist and is delivered with this asset, it is created by a community user based
//on this version. In that version is shown how the script can be used to create a push to talk function. Those kind of functions do not exist in
//the javascript version because it is up to the developer himself to develope that kind of functionality based on this asset.
//There is an example scene delivered with this asset showing off object scaling based on voice strength and showing of the build in gui selection screen.
//there is also a tutorial video.

//For WebPlayer build !!! you need to add this code http://docs.unity3d.com/412/Documentation/ScriptReference/Application.RequestUserAuthorization.html?from=UserAuthorization
//This is not included this script or the source code because it troubles the normal machines builds.

Chapter 1: User accessible variables and functions
Chapter 2: Explanation of the script variables and functions (how the script works).
Chapter 3: Examples.
Chapter 4: WebPLayer setup.

Newest video tutorial: https:https:https://www.youtube.com/watch?v=pD29teWi2fg
webplayer tutorial: https://www.youtube.com/watch?v=euL41orNfPM&feature=youtu.be
older video tutorial: http://www.youtube.com/watch?v=VqHYP7iVG3E&feature=youtu.be




~~ Functionality Overview ~~

1. This script allows you to call information from a selected microphone.
2. The script can detect every microphone attached.
3. There is a GUI function to select the microphone ingame.
4. The script finds the volume of sound going through ONLY the microphone.
5. The volume of the AudioSource directly affects the loudness variable (can be set in script.
6. If only 1 microphone is connected, that device is made default.
7. Can call the input loudness from outside the script.
8. Create a GUI from any script with a build in gui function.




A script that allows you to call information from the computers microphone from any script.

It can recognize up to six different mic's.
The user can set the Sensitivity and choose to show debug information. Furthermore
is it possible to print the device number and name of all mic's into the console, this way
the user knows which slot to select for streaming, then the script knows to which device it should listen.

The maximum spectrum data that gets streamed is by default 256, there is no need to change
this value unless you really know what you are doing (increasing may cause performance issues).



~~ How To Setup ~~

1. Place script on player or empty gameObject. "(this has to be done to call it from any other script)."
2. Done

(Once placed on a player or empty gameObject all its public functions and variables can be called from any script. See the call functions below).


"~~ Chapter 1: User accessible variables and functions ~~
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//Call variables and functions. All these functions can be called from external java or C# scripts.

//Main function.
MicControl.loudness;
[This call's the volume data from your microphone in any script and converts it to the loudness value.]

//Sub functions.
MicControl.StartMicrophone();
[This will force the microphone to start recording when called from an external script or a custom mod script that detects microphones.]

MicControl.StopMicrophone();
[This will force stop the microphone from anything recording/listening. This is useful when your player dies and is not allowed to use the MicControl functions anymore.]

MicControl.MicDeviceGUI (left:float , top:float, width:float, height:float, buttonSpaceTop:float, buttonSpaceLeft:float) - 
 [This lets you create a GUI element which you can use to give you a selection of the different microphones.]
 [Both buttonSpaceTop and buttonSpaceLeft need to be, the left or top, add the space you want between each button.]
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------



"~~ Chapter 2: Explanation of all the variables and functions used in the script ~~
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
variable and function explanation:

//visible in editor var's. All vars shown in the editor are to control the input volume/data (you cannot edit the audio, only use it as a control value).

var SelectIngame: With this variable you can decide if an ingame menu should show to select your microphone or not.
                  If not selected it will use the default microphone, unless changed in the editor slot.

var ThreeD: This will enable/dissable the 3D audio function. Note that a Microphone can only be 2D and that this effect is achieved by dynamically altering of the AudioSource
			volume through scripting.
			
var VolumeFallOff: This component will be unlocked when ThreeD is active. Since the the microphone can only handle 2D audio (as mentioned above), the 3D effect is mimiced
				   by dynamically altering the volume, with the VolumeFallOff parameter you can set how heavy this effect is. A lower value means the audio can be heard from a
				   greater distance and a higher value means it will dissapear much faster.
				   
var PanThreshold: This component will be unlocked when ThreeD is active. As with VolumeFallOff this creates another part of the 3D illusion. This value will determine how
				  smooth the transition from the left speaker to the right speaker will be.				   

var InputDevice : Select the microphone you want to use (supported up to 6 to choose from). If the device 
				  has number 1 in the console, you just select default as it is the first device to be found.


 
 
var audioSource: Here the script will place its AudioSource, which it generates by itself.

var amountSamples: The maximum amount of sample data that gets loaded in, best is to 
				   leave it on 256, unless you know what you are doing. A higher number gives more accuracy but 
		 		   lowers performance allot, it is best to leave it at 256.


var sensitivity: How sensitive should the input be? 0 will detect nothing, a higher number will give a bigger
				 loudness value.

var sourceVolume: How load should the script receive my voice/Input? A lower value will result in a smaller loudness
          		  value. This also influence the hearable audio (from the microphone) ingame if turned on.



var Mute: This values determine if the user can hear himself or not. With this the user could write a script for multiplayer
	 	  So that for himself the mute is on but for other players it is not, thus creating a push to talk (this is not
     	  Included in the script, as the script is created for input 'value's only).

var debug: Writes the loudness value to the console, this can be used to see when higher pitches are used or
 	  	   whether the device is receiving input or not.

var ShowDeviceName:  When turned on the console will spit out each single device detected and its number. This
		   			 can be used to find out which slot should be selected in the editor.


//private var's (leave them untouched).

private var selectedDevice: this refers to the slot in 'enum Devices' and should be left untouched.

private var minFreq: 
		   			  Both these values are used to detect the frequency of your device. If no frequency can be detected it
		   			  will use a default of 44100. These values should be left untouched.		     
private var maxFreq: 
 

private var micSelected: This value should be left untouched as it is only used to show the GUI of 'SelectIngame'.

private var recording: This value tells the script if the mic is streaming or not.

private var ListenerDistance: this value works together with VolumeFallOff to create the correct volume drop if the MicController source is farther away
							  from the listerener.
private var ListenerPosition: this value works with PanThreshold to place the audio in the correct speaker.

private var focused:  Here we have a very important piece, with this the MicController can detect if the application is in use or not. If not
					  it will stop the stream when the application is not in use and restart/initialize the microphone when the aplication is back in use.
					  This is very important as it prevents data lag (sound playing seconds to late) and keeps the stream 'realtime'.
							 
private var Initialised: Checks if the microphone is initialized, if not it will do so.




//static var's
static var loudness: This value can be called from any script. It is read only and it gives you a single float value
		       	     based on the input data (No actual audio). This can be used to create interactive objects or
 		    		 to change objects, basically everything that uses float values can be changed with this.

		  		     Example: transform.position.x=MicControl.loudness. This will move the object over the X axis
			     	 based on the input gathered from the microphone.



// functions \\

//create a standard gui button from any script that is automatically hooked up to the Microphone selection function.
public function MicDeviceGUI: This creates a function to create microphone gui buttons on the fly.

//called only inside the script, do not touch or change anything in this function! It is it's core setup.
private  function InitMic():Sets up and initializes the microphone.

//call these functions in external scripts to start or stop the microphone (not recommended as it may distort your data stream).
public functions StartMicrophone () -
This starts the listening of audio from your microphone.

public functions StopMicrophone () - 
This stops the playing of audio from your microphone.

function OnApplicationFocus(focus: boolean)-
This will switch the 'focused' boolean to true if the application is active.

function OnApplicationPause(focus: boolean)-
This will switch the 'focused' boolean to false if the application is not active.


----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------




"~~ Chapter 3: Examples ~~
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//java
//this example will scale an object based on the values received trough the microphone. The object will scale with the strength of your voice.

var Scale=Vector3(0,0,0);

function Update(){
var Scale= Vector3(1,MicControl.loudness,1);

transform.localScale=Scale;

}


//This script will force shut down the selected microphone on start of the level and then proceeds to start it up again, this can be used on a respawn.

function Start(){
//stop current mic
MicControl.StopMicrophone();
//start fresh input.
MicControl.StartMicrophone();

}

//create a gui button that selects a microphone
MicControl.MicDeviceGUI(400, 100, 300, 100, 110, 0);

//C#
//this example will scale an object based on the values received trough the microphone. The object will scale with the strength of your voice.

void Update(){
Vector3 Scale= Vector3(1,MicControl.loudness,1);

transform.localScale=Scale;

}


//This script will force shut down the selected microphone on start of the level and then proceeds to start it up again, this can be used on a respawn.

void Start(){
//stop current mic
MicControl.StopMicrophone();
//start fresh input.
MicControl.StartMicrophone();

}

//create a gui button that selects a microphone
MicControl.MicDeviceGUI(400, 100, 300, 100, 110, 0);
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
"~~ Chapter 3: WebPlayer Setup ~~
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------

To use this script in the web player do not switch the platform to webplayer mode! As of this writing 31-07-2014 the unity editor crashes when initializing the microphone
in the editors 'webplayer' playmode.

Instead setup your microphone script trough the standalone build platform (PC, Mac, Linux). These modes will correctly initialize the microphones.
When your script is setup to your likings and all the other scripts that call to your script are working as they should do. Build to the webplayer and open the html link generated by
Unity to view your webplayer build. In here you will now see a request to use your mic, accept it and there you go a functional microphone control inside your web player.

webplayer tutorial: https://www.youtube.com/watch?v=euL41orNfPM&feature=youtu.be


-----------------------
Thank You For Reading
-----------------------

http://markduisters.blogspot.be/