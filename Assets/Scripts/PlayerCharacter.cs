using UnityEngine;
using System.Collections;
using InControl;
using UnityStandardAssets.ImageEffects;

public class PlayerCharacter : MonoBehaviour {
	private static PlayerCharacter _instance;
	public static void AddHealth(float h){
		_instance.health += h;
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
					Application.LoadLevel(0);
				} else {
					float t = _health/_maxHealth;
					nag.intensityMultiplier = Mathf.Lerp(_minNoise,_maxNoise,1-t);
					pix.height = (int)Mathf.Lerp((float)_minPix,(float)_maxPix,t);
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
	void Awake(){
		_instance = this;
		cam = Camera.main.transform;
		pix = cam.GetComponent<Pixelz>();
		_maxPix = pix.height;
		nag = cam.GetComponent<NoiseAndGrain>();
		_minNoise = nag.intensityMultiplier;
		rb = GetComponent<Rigidbody>();
		rb.inertiaTensor = new Vector3(1e3f,1e3f,1e3f);
		GetComponentInChildren<LightShafts>().m_Cameras = new Camera[]{Camera.main};
	}

	void FixedUpdate () {
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
		Debug.Log(angle+" "+dmg);
	}
	void OnTriggerEnter(Collider col){
		col.GetComponent<Monster>().InSight();
	}
	void OnTriggerExit(Collider col){
		col.GetComponent<Monster>().OutOfSight();
	}
}
