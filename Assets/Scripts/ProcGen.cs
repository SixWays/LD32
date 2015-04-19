using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ProcGen : MonoBehaviour {
	public int biome=1;
	public int width=3;
	public int height=4;
	public int unit=20;
	public float branchChance=0.25f;
	public GameObject[] chunks;
	public List<GameObject> n;
	public List<GameObject> s;
	public List<GameObject> e;
	public List<GameObject> w;
	public List<GameObject> nsew;
	private GameObject _start;
	private GameObject _end;

	private class Node {
		public bool n=false;
		public bool s=false;
		public bool w=false;
		public bool e=false;
		public int x=0;
		public int y=0;
		public bool start=false;
		public bool end=false;
	}
	private Node[,] matrix;
	private enum Dir {N,S,E,W}

	void Awake(){
		chunks = Resources.LoadAll<GameObject>("Chunks/B"+biome.ToString());
		foreach (GameObject go in chunks){
			if (go.name.Contains("Start")){
				_start = go;
			} else if (go.name.Contains("End")){
				_end = go;
			} else {
				if (go.name.Contains("_N")){
					n.Add(go);
				}
				if (go.name.Contains("_S")){
					s.Add(go);
				}
				if (go.name.Contains("_E")){
					e.Add(go);
				}
				if (go.name.Contains("_W")){
					w.Add(go);
				}
				if (go.name.Contains("_N_S_E_W")){
					nsew.Add(go);
				}
			}
		}
		matrix = new Node[width,height];
	}
	// Use this for initialization
	void Start () {
		// X pos of start tile
		int startX = Random.Range(0,width-1);
		// X pos of end tile
		int endX = Random.Range(0,width-1);

		// Generate map first, then fill with chunks
		// Zeroth row is start tile
		Node start = new Node();
		start.start = true;
		start.n = true;
		start.x = startX;
		matrix[startX,0] = start; 
		// Last row is end tile
		Node end = new Node();
		end.end = true;
		end.s = true;
		end.x = endX;
		end.y = height-1;
		matrix[endX,height-1] = end;

		// Make initial straight line between
		for (int i=1; i<height; ++i){
			Node temp = new Node();
			temp.x = startX;
			temp.y = i;
			matrix[startX,i] = temp;
		}
		int j=startX;
		int dir = (int)Mathf.Sign(endX - startX);
		while (j!=endX){
			j+=dir;
			Node temp = new Node();
			temp.x = j;
			temp.y = height-2;
			matrix[j,height-2] = temp;
		}

		for (int i=0; i<width; ++i){
			for (int k=0; k<height; ++k){
				if (matrix[i,k]!=null){
					GameObject c=null;
					if (k==0){
						c = GameObject.Instantiate(_start) as GameObject;
					} else if (k==height-1){
						c = GameObject.Instantiate(_end) as GameObject;
						c.GetComponentInChildren<LightShafts>().m_Cameras = new Camera[]{Camera.main};
					} else {
						c = GameObject.Instantiate(nsew[Random.Range(0,nsew.Count)]) as GameObject;
					}
					c.transform.position += new Vector3(matrix[i,k].x*unit,0,matrix[i,k].y*unit);
				}
			}
		}
		/*

		bool ended=false;
		int col = startX;
		int row = 1;
		while (!ended){
			matrix[col][row] = new Node();
			// Room off west?
			if (col > 0){
				if (Random.Range(0,2)==0)
			}
			// Room off east?
			if (col < width){

			}
			// Room off north?

		}
		// Populate
		for (int i=0; i<height; ++i){

		}*/


	}
	/*
	void AddRoom(int col, int row, int maxCol, int maxRow, Dir last){
		Node n = new Node();

		bool canNorth=(row<maxRow && last!=Dir.S);
		bool canSouth=(row>0 && last!=Dir.N);
		bool canWest=(col>0 && last!=Dir.E);
		bool canEast=(col<maxCol && last!=Dir.W);

		Dir next = NextDir(canNorth,canSouth,canEast,canWest);

		switch (next){
		case Dir.E:
			n.e = true;
			++col;
			break;
		case Dir.N:
			n.n = true;
			++row;
			break;
		case Dir.S:
			n.s = true;
			--row;
			break;
		case Dir.W:
			n.w = true;
			--col;
			break;
		}

		matrix[col][row] = n;
	}
	Dir NextDir(bool canNorth, bool canSouth, bool canEast, bool canWest){
		while (true){
			Dir next = (Dir)Random.Range(0,5);
			if ((next==Dir.N && canNorth)
			    || (next==Dir.S && canSouth)
			    || (next==Dir.E && canEast)
			    || (next==Dir.W && canWest)
			    ){
				return next;
			}
		}
	}*/
}
