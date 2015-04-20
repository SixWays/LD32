using UnityEngine;
using System.Collections;

public class LevelMgr : MonoBehaviour {
	public bool last=false;
	private static LevelMgr _instance;
	[SerializeField]
	private float time=1f;
	private float t;
	[SerializeField]
	private GameObject fader;
	[SerializeField]
	private GameObject dmg;
	private Material fadeMat;
	[SerializeField]
	private GameObject heal;
	private Material healMat;
	private Material dmgMat;
	[SerializeField]
	private Color dmgColEnd;
	private Color dmgColStart;
	[SerializeField]
	private Color healColEnd;
	private Color healColStart;
	[SerializeField]
	private Color win;
	[SerializeField]
	private Color die;
	
	private bool active=false;

	void Awake(){
		if (last){
			Destroy(GameObject.Find("Menu"));
			Application.LoadLevel(0);
		}
		_instance = this;
		dmgMat = dmg.GetComponent<Renderer>().material;
		dmgColStart = dmgColEnd;
		dmgColStart.a=0;
		dmgMat.color=dmgColStart;
		dmg.SetActive(true);
		healMat = heal.GetComponent<Renderer>().material;
		healColStart = healColEnd;
		healColStart.a = 0;
		healMat.color = healColStart;
		heal.SetActive(true);
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
		Application.LoadLevel(level);
	}
	public static void SetDmg(float factor){
		_instance.SetMyDmg(factor);
	}
	private void SetMyDmg(float factor){
		dmgMat.color = Color.Lerp(dmgColStart,dmgColEnd,factor);
	}
	public static void Heal(){
		_instance.HealMe();
	}
	void HealMe(){
		StartCoroutine(HealLoop());
	}
	IEnumerator HealLoop(){
		float myt=time;
		while (myt>0){
			healMat.color = Color.Lerp(healColStart,healColEnd,(myt/time));
			myt-=Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}
}
