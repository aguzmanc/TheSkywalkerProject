using UnityEngine;
using System.Collections;

public class ColisseumCamera : MonoBehaviour 
{
	public GameObject Target;

	void Start () 
	{
		if (Target == null) {
			Target = GameObject.FindGameObjectWithTag("modulo_espacial");
		}
	}
	
	void Update () 
	{
		if (Target != null) {
			transform.Rotate(Vector3.up, 0.01f);
			Camera.main.transform.LookAt(Target.transform.position);
		}
	}
}
