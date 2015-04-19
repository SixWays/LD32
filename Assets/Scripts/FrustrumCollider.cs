using UnityEngine;
using System.Collections;

public class FrustrumCollider : MonoBehaviour {
	Mesh col;
	public float range=6;
	// Use this for initialization
	void Start () {
		Light view = GetComponentInChildren<Light>();
		col = new Mesh();

		float x = Mathf.Sin(0.9f*view.spotAngle/2)*range;

		Vector3[] v = new Vector3[8];
		v[0] = new Vector3(-0.5f,0.5f,0.5f);
		v[1] = new Vector3(0.5f,0.5f,0.5f);
		v[2] = new Vector3(0.5f,-0.5f,0.5f);
		v[3] = new Vector3(-0.5f,-0.5f,0.5f);

		float z = range/2;

		v[4] = new Vector3(-x,0.5f,z);
		v[5] = new Vector3(x,0.5f,z);
		v[6] = new Vector3(x,-0.5f,z);
		v[7] = new Vector3(-x,-0.5f,z);

		int[] t = new int[]{
			// Back
			0,1,2,
			2,3,0,

			// Top
			0,4,1,
			0,5,1,

			// Bottom
			3,7,2,
			3,6,2,

			// Left
			0,4,7,
			0,3,7,

			// Right
			1,5,6,
			1,2,6,

			// Front
			4,5,6,
			6,7,4
		};

		col.vertices = v;
		col.triangles = t;

		GetComponentInChildren<MeshCollider>().sharedMesh = col;
	}
}
