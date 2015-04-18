using UnityEngine;
using System.Collections;
using InControl;
using UnityStandardAssets.ImageEffects;

public class PlayerCharacter : MonoBehaviour {
	InputDevice id;
	float rRate;
	NoiseAndGrain nag;
	Pixelz pix;
	[SerializeField]
	private float _maxHealth=100;
	[SerializeField]
	private float _maxNoise = -2;
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
	private Light view;

	private float _health=100;
	public float health {
		get {
			return _health;
		}
		set {
			_health = value;
			if (_health < 0){
				Application.LoadLevel(0);
			} else {
				float t = 1-(_health/_maxHealth);
				nag.generalIntensity = Mathf.Lerp(_minNoise,_maxNoise,t);
				pix.height = (int)Mathf.Lerp((float)_minPix,(float)_maxPix,t);
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
		cam = Camera.main.transform;
		pix = cam.GetComponent<Pixelz>();
		_maxPix = pix.height;
		nag = cam.GetComponent<NoiseAndGrain>();
		_minNoise = nag.generalIntensity;
		rb = GetComponent<Rigidbody>();
		rb.inertiaTensor = new Vector3(1e3f,1e3f,1e3f);
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
		float angle = Vector3.Angle((col.transform.position - transform.position),transform.forward);
		float damage = Mathf.Lerp(_minDamageFactor,1,1-(angle/view.spotAngle));
		_health -= damage*_damageRate*Time.deltaTime;
	}
}
