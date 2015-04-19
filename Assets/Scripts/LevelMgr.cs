using UnityEngine;
using System.Collections;

public class LevelMgr : MonoBehaviour {
	private static LevelMgr _instance;
	[SerializeField]
	private float time=1f;
	private float t;
	[SerializeField]
	private GameObject fader;
	private Material mat;
	[SerializeField]
	private Color win;
	[SerializeField]
	private Color die;
	
	private bool active=false;

	void Awake(){
		_instance = this;
	}

	public static void Win(){
		_instance.OnWin();
	}
	public static void Die(){
		_instance.OnDie();
	}

	private void OnWin(){
		if (!active){
			mat = fader.GetComponent<Renderer>().material;
			StartCoroutine(Fade(win,Application.loadedLevel+1));
		}
		active = true;
	}
	private void OnDie(){
		if (!active){
			mat = fader.GetComponent<Renderer>().material;
			StartCoroutine(Fade(die,Application.loadedLevel));
		}
		active = true;
	}
	IEnumerator Fade(Color c, int level){
		t=time;
		Color end = c;
		end.a = 1;
		Color start = c;
		start.a = 0;
		while (t>0){
			mat.color = Color.Lerp(start,end,1-(t/time));
			t-=Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		//Application.LoadLevel(level);
	}
}
