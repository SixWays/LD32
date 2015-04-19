using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityStandardAssets.ImageEffects;

public class Crumb {
	public Vector3 pos;
}
public class PlayerCharacter : MonoBehaviour {
	public static void PlayDeathSound(AudioClip ac){
		_instance.cam.GetComponent<AudioSource>().PlayOneShot(ac,0.3f);
	}
	public AudioMixer mixer;
	public float _maxBkgVol=0;
	private float _minBkgVol=-34;
	private static PlayerCharacter _instance;
	public static List<Crumb> crumbs;
	public static void AddHealth(float h){
		_instance.AddMyHealth(h);
		LevelMgr.Heal();
	}
	private void AddMyHealth(float h){
		StartCoroutine(HealthLoop(h));
	}
	IEnumerator HealthLoop(float h){
		float hpt = (h/2)*Time.fixedDeltaTime;
		while (h>0){
			health+=hpt;
			h-=hpt;
			yield return new WaitForFixedUpdate();
		}
	}
	public static bool CheckCollider(Collider c){
		return (_instance.myCol == c);
	}
	public static Vector3 pos {
		get {
			return _instance.transform.position;
		}
	}
	public bool invuln=false;
	float rRate;
	NoiseAndGrain nag;
	Pixelz pix;
	[SerializeField]
	private float _maxHealth=100;
	[SerializeField]
	private float _maxNoise = -3;
	private float _minNoise = -0.2f;
	[SerializeField]
	private float _peakNoise = -10;
	[SerializeField]
	private float _damageRate=5;
	[SerializeField]
	private float _minDamageFactor=0.2f;
	private int _maxPix=320;
	[SerializeField]
	private int _minPix=64;
	[SerializeField]
	private float _maxDmgDist=1;
	[SerializeField]
	private float _minDmgDist=0.5f;

	[SerializeField]
	private Light view;

	private float _health=100;
	public float health {
		get {
			return _health;
		}
		set {
			if (!invuln){
				_health = value;
				if (_health < 0){
					LevelMgr.Die();
				} else {
					if (_health>_maxHealth){
						_health = _maxHealth;
					}
					float t = _health/_maxHealth;
					nag.intensityMultiplier = Mathf.Lerp(_minNoise,_maxNoise,1-t);
					mixer.SetFloat("BkgVol",Mathf.Lerp(_minBkgVol,_maxBkgVol,1-t));
					pix.height = (int)Mathf.Lerp((float)_minPix,(float)_maxPix,t);
					LevelMgr.SetDmg(1-(_health/_maxHealth));
				}
			}
		}
	}
	public float speed=1f;
	public float lockFactor=0.66f;
	public float drag=0.1f;
	Vector3 moveRate = new Vector3();
	public float rSmooth=0.1f;
	public float dead=0.1f;
	public float camLag=0.5f;
	Vector3 camRate = new Vector3();
	Transform cam;
	Rigidbody rb;
	float diff;
	float angle;
	[SerializeField]
	private Collider myCol;
	void Awake(){
		_instance = this;
		crumbs = new List<Crumb>();
		cam = Camera.main.transform;
		pix = cam.GetComponent<Pixelz>();
		_maxPix = pix.height;
		nag = cam.GetComponent<NoiseAndGrain>();
		_minNoise = nag.intensityMultiplier;
		rb = GetComponent<Rigidbody>();
		rb.inertiaTensor = new Vector3(1e3f,1e3f,1e3f);
		GetComponentInChildren<LightShafts>().m_Cameras = new Camera[]{Camera.main};
		// Reset effects
		health = _maxHealth;
	}
	IEnumerator LeaveCrumbs(){
		int num=0;
		while(true){
			if (crumbs.Count < 20){
				crumbs.Add(GetCrumb());
			} else {
				for (int i=0; i<crumbs.Count-1; ++i){
					// Pull each entry toward front
					crumbs[i]=crumbs[i+1];
				}
				// Push to back
				crumbs[crumbs.Count-1]=GetCrumb();
			}
			yield return new WaitForSeconds(0.2f);
		}
	}
	public static Crumb GetCrumb(){
		Crumb result = new Crumb();
		result.pos = new Vector3(pos.x,pos.y,pos.z);
		return result;
	}
	public static Crumb NextCrum(Crumb c){
		if (crumbs.Contains(c)){
			int i = crumbs.IndexOf(c);
			if (i<(crumbs.Count-1)){
				return crumbs[i+1];
			}
		}
		return GetCrumb();
	}

	void Update () {
		InputDevice device = InputManager.ActiveDevice;
		// Movement
		float n = device.GetControl(InputControlType.LeftStickY).Value 
			+ device.GetControl(InputControlType.RightStickY).Value
			+ Input.GetAxis("Vertical");
		float e = device.GetControl(InputControlType.LeftStickX).Value
			+ device.GetControl(InputControlType.RightStickX).Value
			+ Input.GetAxis("Horizontal");
		bool l = device.GetControl(InputControlType.Action1).State
			|| device.GetControl(InputControlType.Action2).State
			|| device.GetControl(InputControlType.RightTrigger).State
			|| device.GetControl(InputControlType.RightBumper).State
			|| Input.GetButton("Jump");
		Vector3 dir = Vector3.zero;
		if ((Mathf.Abs(e)>dead || Mathf.Abs(n)>dead)){
			dir = new Vector3(e,0,n);
			if (!l){
				angle = Vector3.Angle(Vector3.forward,dir.normalized);
				if (e<0){
					angle = 360-angle;
				}
			} else {
				angle = transform.eulerAngles.y;
			}
			diff = Vector3.Angle(transform.eulerAngles,dir.normalized);

			//transform.eulerAngles = ea;
		}
		Vector3 ea = transform.eulerAngles;
		// Rate is 0.1s per 180 degrees
		ea.y = Mathf.SmoothDampAngle(ea.y,angle,ref rRate,rSmooth*diff/180);
		Quaternion q = Quaternion.Euler(ea);
		rb.rotation = q;
		rb.velocity = Vector3.SmoothDamp(rb.velocity,
		                                 Vector3.ClampMagnitude(dir,1) * speed * (l?lockFactor:1),
		                                 ref moveRate,
		                                 drag);
		Vector3 camPos = Vector3.SmoothDamp(cam.position,transform.position,ref camRate,camLag);
		camPos.y = cam.position.y;
		cam.position = camPos;
	}

	void OnTriggerStay(Collider col){
		RaycastHit hit;
		if (Physics.Linecast(transform.position,col.transform.position,out hit)){
			if (hit.collider == col){
				float dist = Vector3.Distance(transform.position,col.transform.position);
				float dmg;
				float angle=-1;
				if (dist < _maxDmgDist){
					dmg = _damageRate;
				} else {
					angle = Vector3.Angle((col.transform.position - transform.position),transform.forward);
					dmg = Mathf.Lerp(_minDamageFactor,1,1-(angle/(view.spotAngle/2)));
					dmg = Mathf.Clamp(dmg*_damageRate*1.2f,0,_damageRate);
				}
				health -= dmg*Time.deltaTime;
				//Debug.Log(angle+" "+dmg);
			}
		}
	}
	void OnTriggerEnter(Collider col){
		col.GetComponent<Monster>().InSight();
	}
	void OnTriggerExit(Collider col){
		col.GetComponent<Monster>().OutOfSight();
	}
}
