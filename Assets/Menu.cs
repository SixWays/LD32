using UnityEngine;
using System.Collections;
using InControl;

public class Menu : MonoBehaviour {
	private static Menu _instance;
	private bool open=false;
	void Awake(){
		_instance = this;
		Open();
		DontDestroyOnLoad(gameObject);
	}
	public void Restart(){
		Application.LoadLevel(0);
	}
	public void Resume(){
		Close();
	}
	public void Quit(){
		Application.Quit();
	}
	void Close(){
		Time.timeScale = 1;
		GetComponent<Canvas>().enabled = false;
		open=false;
	}
	void Open(){
		Time.timeScale = 1e-6f;
		GetComponent<Canvas>().enabled = true;
		open=true;
	}
	/*
	public static void OpenMenu(){
		_instance.Open();
	}
	public static void CloseMenu(){
		_instance.Close();
	}*/
	void Update(){
		InputDevice device = InputManager.ActiveDevice;
		if (device.GetControl(InputControlType.Menu).WasPressed || Input.GetKeyDown(KeyCode.Escape)){
			if (open){
				Close ();
			} else {
				Open ();
			}
		}
	}
}
