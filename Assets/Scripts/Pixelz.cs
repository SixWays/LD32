using UnityEngine;
using System.Collections;

public class Pixelz : MonoBehaviour {
	[SerializeField]
	private int _height=320;
	public int height {
		get {
			return _height;
		}
		set {
			_height = value;
			NewRT();
		}
	}
	RenderTexture scaled;
	float aspect;
	void Awake(){
		NewRT();
	}
	void OnRenderImage(RenderTexture src, RenderTexture dest){
		if (Camera.main.aspect != aspect){
			NewRT();
		}
		Graphics.Blit(src,scaled);
		Graphics.Blit(scaled,dest);
	}
	void NewRT(){
		aspect = Camera.main.aspect;
		scaled = new RenderTexture((int)(height*aspect),height,24,RenderTextureFormat.ARGB4444);
		scaled.antiAliasing = 1;
		scaled.filterMode = FilterMode.Point;
	}
}
