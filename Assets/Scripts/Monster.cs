using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {
	bool active=false;

	// Use this for initialization
	void Start () {
		StartCoroutine(AILoop());
	}

	void OnTriggerEnter(Collider col){
		// Col is player
		active = true;
	}

	IEnumerator AILoop(){
		while (true){
			if (active){

			}
			yield return new WaitForSeconds(0.05f);
		}
	}

	[SerializeField]
	private float memory=8f;
	private float timeToDie;
	IEnumerator OOS(){
		timeToDie = memory;
		while (timeToDie>0){
			timeToDie -= Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Destroy(gameObject);
	}

	public Coroutine coos;
	public void OutOfSight(){
		coos = StartCoroutine(OOS());
	}
	public void InSight(){
		StopCoroutine(coos);
	}
	void OnDestroy(){
		PlayerCharacter.AddHealth(20);
	}
}
