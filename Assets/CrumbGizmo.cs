#if UNITY_EDITOR
using UnityEngine;

using UnityEditor;

using System.Collections;

public class CrumbGizmo : MonoBehaviour {
	void OnDrawGizmos(){
		Gizmos.DrawIcon(transform.position, "g1.psd", true);
	}
}
#endif