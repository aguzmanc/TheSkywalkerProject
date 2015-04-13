using UnityEngine;
using System.Collections;

public class ROBOT_EULER : MonoBehaviour 
{
	public float MotorForce = 1.0f;
	
	private GameObject _motorX, _motorX2;
	private GameObject _motorY;
	private GameObject _motorZ;
	private GameObject _body;

	private Vector3 _motorForceVector;

	void Start () 
	{
		_motorX = GameObject.Find ("MotorX");
		_motorX2 = GameObject.Find ("MotorX_2");
		_motorY = GameObject.Find ("MotorY");
		_motorZ = GameObject.Find ("MotorZ");

		_body = GameObject.Find ("MainBody");
		
		_motorForceVector = new Vector3(0, this.MotorForce, 0);
	}
	
	void Update () 
	{
	}

	void FixedUpdate()
	{
		setMotorActive (_motorX, false, true);
		setMotorActive (_motorX2, false, true);
		setMotorActive (_motorY, false, true);
		setMotorActive (_motorZ, false, true);

		bool forward = Input.GetKey (KeyCode.LeftShift);
		
		if (Input.GetKey (KeyCode.X)) 
			setMotorActive (_motorX, true, forward);

		if (Input.GetKey (KeyCode.Y)) 
			setMotorActive(_motorY, true, forward);

		if (Input.GetKey (KeyCode.Z)) 
			setMotorActive(_motorZ, true, forward);

		if (Input.GetKey (KeyCode.F)) {
			_body.rigidbody.AddRelativeForce (Vector3.forward);
			_motorX.renderer.material.color = Color.green;
			_motorX2.renderer.material.color = Color.green;
		}
	}

	private void setMotorActive(GameObject motor, bool active, bool forward)
	{
		motor.renderer.material.color = active ? Color.green:Color.white;
		
		if (active) {
			Vector3 torque = Vector3.forward;
			float mult = forward?1.0f:-1.0f;

			if(motor.Equals(_motorX))
				torque = Vector3.right * mult;

			if(motor.Equals(_motorX2))
				torque = Vector3.left * mult;

			if(motor.Equals(_motorY))
				torque = Vector3.up * mult;

			if(motor.Equals(_motorZ))
				torque = Vector3.forward * mult;

			_body.rigidbody.AddRelativeTorque(torque, ForceMode.Acceleration);
		}
	}
}
