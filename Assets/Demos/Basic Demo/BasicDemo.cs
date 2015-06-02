using UnityEngine;
using System.Collections;
using System.Text;

public class BasicDemo : MonoBehaviour {

	string fromArduino = "" ;
	byte [] buffer = new byte[6];
	string stringToEdit = "HC-06";


	void Start () {
		//use one of the following two methods to change the default bluetooth module.
		//BtConnector.moduleMAC("00:13:12:09:55:17");
		BtConnector.moduleName ("HC-06");
	}

	void OnGUI(){
		GUI.Label(new Rect(0, 0, Screen.width*0.15f, Screen.height*0.1f),"Module Name ");

		stringToEdit = GUI.TextField(new Rect(Screen.width*0.15f, 0, Screen.width*0.8f, Screen.height*0.1f), stringToEdit);
		GUI.Label(new Rect(0,Screen.height*0.2f,Screen.width,Screen.height*0.1f),"Arduino Says : " + ByteArrayToString(buffer));
		GUI.Label(new Rect(0,Screen.height*0.3f,Screen.width,Screen.height*0.1f),"from PlugIn : " + BtConnector.readControlData ());

		if(GUI.Button(new Rect(0,Screen.height*0.6f,Screen.width,Screen.height*0.1f), "Connect")) 
		{
				if (!BtConnector.isBluetoothEnabled ()){
					BtConnector.askEnableBluetooth();
				} else BtConnector.connect();
		}

	
		///the hidden code here let you connect directly without askin the user
		/// if you want to use it, make sure to hide the code from line 23 to lin 33
		/*
		if( GUILayout.Button ("Connect")){ 
			
			startConnection = true; 
			
		} 
		
		if(GUI.Button(new Rect(0,Screen.height*0.4f,Screen.width,Screen.height*0.1f), "Connect")) 
		{
			if (!BtConnector.isBluetoothEnabled ()){ 
				BtConnector.enableBluetooth(); 
				
			} else {  
				
				BtConnector.connect(); 
				
				startConnection = false; 
				
			} 
			
		} 
		*/
		/////////////
		/*if(GUI.Button(new Rect(0,Screen.height*0.6f,Screen.width,Screen.height*0.1f), "sendChar")) {
			 if(BtConnector.isConnected()){
				BtConnector.sendChar('h');
				BtConnector.sendChar('e');
				BtConnector.sendChar('l');
				BtConnector.sendChar('l');
				BtConnector.sendChar('o');
				BtConnector.sendChar ('\n');//because we are going to read it using .readLine() which reads lines.

			}
				
		}
		if(GUI.Button(new Rect(0,Screen.height*0.5f,Screen.width,Screen.height*0.1f), "sendString")) {
			
			if(BtConnector.isConnected()){
				BtConnector.sendString("Hii");
				BtConnector.sendString("you can do this");
			}
		}*/


	
		if(GUI.Button(new Rect(0,Screen.height*0.7f,Screen.width,Screen.height*0.1f), "Close")) {
			BtConnector.close();
		}

		if(GUI.Button(new Rect(0,Screen.height*0.8f,Screen.width,Screen.height*0.1f), "readData")) {
			//fromArduino = BtConnector.readLine();
			buffer = BtConnector.readBuffer();

		}

		if (GUI.Button (new Rect (0, Screen.height * 0.9f, Screen.width, Screen.height * 0.1f), "change ModuleName")) {
			BtConnector.moduleName(stringToEdit);

		}




	}

	public static string ByteArrayToString(byte[] ba)
	{
		string completedString = "";
		string singleString;
		byte byteArrayIndex = 0;
		foreach (byte b in ba) {
			singleString = "[" + byteArrayIndex.ToString() + "]" + b.ToString() + " ";
			completedString += singleString;
			byteArrayIndex++;
		}
		return completedString;

		/*StringBuilder hex = new StringBuilder(ba.Length * 2);
		foreach (byte b in ba)
			hex.AppendFormat("{0:x2}", b);
		return hex.ToString();*/
	}
	
	
}


