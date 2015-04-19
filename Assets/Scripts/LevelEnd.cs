using UnityEngine;
using System.Collections;

public class LevelEnd : MonoBehaviour {
	bool triggered=false;
	void OnTriggerEnter(Collider col){
		// Col is player
		LevelMgr.Win();
	}
}
