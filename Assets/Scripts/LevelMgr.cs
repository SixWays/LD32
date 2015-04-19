using UnityEngine;
using System.Collections;

public class LevelMgr : MonoBehaviour {
	private static LevelMgr _instance;
	[SerializeField]
	private float time=1f;
	private float t;
	[SerializeField]
	private GameObject fader;
	[SerializeField]
	private GameObject dmg;
	private Material fadeMat;
	private Material dmgMat;
	[SerializeField]
	private Color dmgColEnd;
	private Color dmgColStart;
	[SerializeField]
	private Color win;
	[SerializeField]
	private Color die;
	
	private bool active=false;

	void Awake(){
		_instance = this;
		dmgMat = dmg.GetComponent<Renderer>().material;
		dmgColStart = dmgColEnd;
		dmgColStart.a=0;
		dmgMat.color=dmgColStart;
		dmg.SetActive(true);
	}

	public static void Win(){
		_instance.OnWin();
	}
	public static void Die(){
		_instance.OnDie();
	}

	private void OnWin(){
		if (!active){
			fader.SetActive(true);
			fadeMat = fader.GetComponent<Renderer>().material;
			StartCoroutine(Fade(win,Application.loadedLevel+1));
		}
		active = true;
	}
	private void OnDie(){
		if (!active){
			fader.SetActive(true);
			fadeMat = fader.GetComponent<Renderer>().material;
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
			fadeMat.color = Color.Lerp(start,end,1-(t/time));
			t-=Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		Application.LoadLevel(Application.loadedLevel);
	}
	public static void SetDmg(float factor){
		_instance.SetMyDmg(factor);
	}
	private void SetMyDmg(float factor){
		dmgMat.color = Color.Lerp(dmgColStart,dmgColEnd,factor);
	}
}
