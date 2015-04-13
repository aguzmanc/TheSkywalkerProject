using UnityEngine;
using System.Collections;

public class ROBOT_2MOTORES : MonoBehaviour 
{
	public float MotorForce = 1.0f;

	private GameObject _motorA;
	private GameObject _motorB;
	private GameObject _body;

	private Vector3 _motorForceVector;
	private Vector3 _motorForceVectorBackwards;

	void Start () 
	{
		_motorA = GameObject.Find ("MotorA");
		_motorB = GameObject.Find ("MotorB");

		_body = GameObject.Find ("MainBody");

		_motorForceVector = new Vector3(0, this.MotorForce, 0);
		_motorForceVectorBackwards = new Vector3(0, -this.MotorForce, 0);
	}
	
	void Update () 
	{
	
	}

	void FixedUpdate()
	{
		setMotorActive (_motorA, false, true);
		setMotorActive (_motorB, false, true);

		if (Input.GetKey (KeyCode.A)) 
		{
			setMotorActive(_motorA, true, true);
		}

		if (Input.GetKey (KeyCode.S)) 
		{
			setMotorActive(_motorA, true, false);
		}

		if (Input.GetKey (KeyCode.K)) 
		{
			setMotorActive(_motorB, true, true);
		}

		if (Input.GetKey (KeyCode.L)) 
		{
			setMotorActive(_motorB, true, false);
		}
	}


	private void setMotorActive(GameObject motor, bool active, bool forward)
	{
		motor.renderer.material.color = active?Color.green:Color.white;

		if (active) {
			Vector3 vector = forward ? _motorForceVector : _motorForceVectorBackwards;
			_body.rigidbody.AddForceAtPosition (vector, motor.transform.localPosition, ForceMode.Impulse);
		}
	}
}
