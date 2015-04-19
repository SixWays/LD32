using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour {
	[SerializeField]
	private GameObject prefab;
	// Use this for initialization
	void Start () {
		Instantiate(prefab,transform.position,transform.rotation);
	}
}
