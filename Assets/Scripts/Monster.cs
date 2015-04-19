using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {
	bool active=false;
	public LayerMask playerLayer;
	public float healthBonus=20f;
	// Use this for initialization
	void Start () {
		StartCoroutine(AILoop());
		if (jumpy){
			StartCoroutine(Moving(aiRate));
		}
		SphereCollider trig=null;
		foreach (SphereCollider c in GetComponentsInChildren<SphereCollider>(true)){
			if (c.isTrigger){
				trig = c;
				break;
			}
		}
		foreach (Collider c in Physics.OverlapSphere(transform.position,trig.radius,playerLayer.value)){
			if (PlayerCharacter.CheckCollider(c)){
				OnTriggerEnter(c);
				break;
			}
		}
	}

	void OnTriggerEnter(Collider col){
		// Col is player
		active = true;
		StartCoroutine(OOS ());

	}
	
	private Vector3 crumb = new Vector3();
	private Vector3 rRate = new Vector3();
	[SerializeField]
	private float rotRate=1.5f;
	[SerializeField]
	private float speed=0.5f;
	[SerializeField]
	private bool jumpy=false;
	[SerializeField]
	private float pauseMin=0.5f;
	[SerializeField]
	private float pauseMax=2f;
	[SerializeField]
	private float moveMin=2f;
	[SerializeField]
	private float moveMax=4f;
	bool paused=false;
	private float aiRate=0.05f;

	private bool tracking=false;
	private bool vis=false;
	IEnumerator AILoop(){
		while (true){
			if (active){
				RaycastHit hit;
				if (Physics.Linecast(transform.position,PlayerCharacter.pos,out hit)){
					if (PlayerCharacter.CheckCollider(hit.collider)){
						vis=true;
						tracking = true;
						crumb = PlayerCharacter.pos;
						crumb.y = transform.position.y;
						//Debug.DrawLine(transform.position,PlayerCharacter.pos,Color.green,aiRate);
					} else {
						vis=false;
						//Debug.DrawLine(transform.position,PlayerCharacter.pos,Color.red,aiRate);
					}
				} else {
					vis=false;
				}
			}
			yield return new WaitForSeconds(aiRate);
		}
	}
	IEnumerator Pause(float dT){
		paused=true;
		float t = Random.Range(pauseMin,pauseMax);
		if (active && tracking){
			while (t>0){
				/*
				if (vis){
					break;
				}*/
				t-=dT;
				yield return new WaitForSeconds(dT);
			}
			StartCoroutine(Moving(dT));
		}
	}
	IEnumerator Moving(float dT){
		paused = false;
		float t = Random.Range(moveMin,moveMax);
		if (active && tracking){
			while (t>0){
				/*
				if (vis){
					break;
				}*/
				t-=dT;
				yield return new WaitForSeconds(dT);
			}
			StartCoroutine(Pause(dT));
		}
	}
	void FixedUpdate(){
		if (active && tracking){
			// Track crumb
			float angle = Vector3.Angle(transform.forward,(crumb-transform.position));
			Vector3 target = (crumb-transform.position).normalized;
			// Rate is per 180 degrees
			transform.forward = Vector3.SmoothDamp(transform.forward,target,ref rRate, rotRate*angle/180);

			if (!paused){
				// Check approach distance
				if (Vector3.Distance(transform.position,crumb) > (vis?1.2f:0.5f)){
					transform.position += transform.forward*speed*Time.fixedDeltaTime;
				} else if (vis){
					// Bounce!
				}
			}
		}
	}

	[SerializeField]
	private float memory=8f;
	private float timeToDie;
	private bool seen=true;
	IEnumerator OOS(){
		timeToDie = memory;
		while (timeToDie>0){
			// Only decrement when unseen
			if (!seen){
				timeToDie -= Time.fixedDeltaTime;
			}
			yield return new WaitForFixedUpdate();
		}
		Destroy(gameObject);
	}

	public Coroutine coos;
	public void OutOfSight(){
		Debug.Log("FORGETTING");
		seen=false;
	}
	public void InSight(){
		Debug.Log("OMGWTF");
		seen=true;
		timeToDie = memory;
	}
	void OnDestroy(){
		PlayerCharacter.AddHealth(healthBonus);
	}
}
