using UnityEngine;
using System.Collections;

public class MaterialMove : MonoBehaviour
{
	public float SpeedX = 0.5f;
	public float SpeedY = 0.5f;
	Renderer render;
    [SerializeField] int materialNum = 0;
    [SerializeField] string texType = "_MainTex";

	void Start ()
	{
		render = GetComponent<Renderer> ();
	}

	void LateUpdate ()
	{
		render.materials[materialNum].SetTextureOffset(texType,new Vector2 (Time.time * SpeedX, Time.time * SpeedY));
	}
}
