using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {
	bool active=false;
	public LayerMask playerLayer;
	public float healthBonus=20f;
	// Use this for initialization
	void Start () {
		StartCoroutine(AILoop());
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
	}
	
	private Vector3 crumb = new Vector3();
	private Vector3 rRate = new Vector3();
	[SerializeField]
	private float rotRate=1.5f;
	[SerializeField]
	private float speed=0.5f;

	private bool vis=false;
	IEnumerator AILoop(){
		while (true){
			if (active){
				RaycastHit hit;
				if (Physics.Linecast(transform.position,PlayerCharacter.pos,out hit)){
					if (PlayerCharacter.CheckCollider(hit.collider)){
						vis=true;
						crumb = PlayerCharacter.pos;
						crumb.y = transform.position.y;
						Debug.DrawLine(transform.position,PlayerCharacter.pos,Color.green,0.05f);
					} else {
						vis=false;
						Debug.DrawLine(transform.position,PlayerCharacter.pos,Color.red,0.05f);
					}
				} else {
					vis=false;
				}
			}
			yield return new WaitForSeconds(0.05f);
		}
	}
	void FixedUpdate(){
		if (active){
			// Track crumb
			float angle = Vector3.Angle(transform.forward,(crumb-transform.position));
			Vector3 target = (crumb-transform.position).normalized;
			// Rate is per 180 degrees
			transform.forward = Vector3.SmoothDamp(transform.forward,target,ref rRate, rotRate*angle/180);

			// Check approach distance
			if (vis){

			}
			if (Vector3.Distance(transform.position,crumb) > (vis?1.2f:0.5f)){
				transform.position += transform.forward*speed*Time.fixedDeltaTime;
			}
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
		if (coos != null){
			StopCoroutine(coos);
		}
	}
	void OnDestroy(){
		PlayerCharacter.AddHealth(healthBonus);
	}
}
