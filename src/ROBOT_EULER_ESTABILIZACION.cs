using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ROBOT_EULER_ESTABILIZACION : MonoBehaviour 
{
	private const int TOTAL_LAST_MAGNITUDES = 20;
	private const float STABLE_THRESHOLD = 0.01f;
	private const float MAX_TIME_BEFORE_MOTOR_CHANGE = 3.0f;

	private const float MIN_TIME_BEFORE_MOTOR_SWITCH = 0.5f;

	public float MotorForce = 1.0f;
	
	private GameObject _motorX, _motorX2;
	private GameObject _motorY;
	private GameObject _motorZ;
	private GameObject _body;
	
	private Vector3 _motorForceVector;

	private float[] _lastMagnitudes;
	private int _lastMagnitudeIndex = 0;

	private float _timeToMotorChange = MAX_TIME_BEFORE_MOTOR_CHANGE;

	private float _timeToMotorSwitch = MIN_TIME_BEFORE_MOTOR_SWITCH;
	
	void Start () 
	{
		_motorX = GameObject.Find ("MotorX");
		_motorX2 = GameObject.Find ("MotorX_2");
		_motorY = GameObject.Find ("MotorY");
		_motorZ = GameObject.Find ("MotorZ");

		_currentStabilizerMotor = _motorX;
		
		_body = GameObject.Find ("MainBody");
		
		_motorForceVector = new Vector3(0, this.MotorForce, 0);

		Vector3 torque = new Vector3 (Random.Range(0,300),Random.Range(0,300),Random.Range(0,300));
		_body.rigidbody.AddRelativeTorque(torque, ForceMode.Force);

		_lastMagnitudes = new float[TOTAL_LAST_MAGNITUDES];
		for (int i=0; i<TOTAL_LAST_MAGNITUDES; i++)
			_lastMagnitudes [i] = 9999;
	}
	
	void Update () 
	{
	}

	bool _stabilizing = false;

	bool _isForward = true;
	float _lastMagnitude = 999;
	GameObject _currentStabilizerMotor;
	
	void FixedUpdate()
	{
		if (_lastMagnitude == 999) {
			_lastMagnitude = _body.rigidbody.angularVelocity.magnitude;
			return;
		}

		bool isIncreasing = (_body.rigidbody.angularVelocity.magnitude > _lastMagnitude);

		_lastMagnitudes[_lastMagnitudeIndex++] = Mathf.Abs(_lastMagnitude - _body.rigidbody.angularVelocity.magnitude);
		_lastMagnitudeIndex %= TOTAL_LAST_MAGNITUDES;

		_lastMagnitude = _body.rigidbody.angularVelocity.magnitude;

//		Debug.Log (_lastMagnitude);
		_timeToMotorSwitch -= Time.fixedDeltaTime;

		setMotorActive (_motorX, false, true);
		setMotorActive (_motorX2, false, true);
		setMotorActive (_motorY, false, true);
		setMotorActive (_motorZ, false, true);

		if (false == _stabilizing)
			return;

		if (isIncreasing) {
			if(_timeToMotorSwitch <=  0.0f) {
				_isForward = !_isForward;
				_timeToMotorSwitch = MIN_TIME_BEFORE_MOTOR_SWITCH;
			}
		}

		setMotorActive (_currentStabilizerMotor, true, _isForward);


		_timeToMotorChange -= Time.fixedDeltaTime;


		float sum = 0.0f;
		for (int i=0; i<TOTAL_LAST_MAGNITUDES; i++)
			sum+=_lastMagnitudes[i];
		sum /= TOTAL_LAST_MAGNITUDES;

		if (sum < STABLE_THRESHOLD || _timeToMotorChange <= 0.0f) {
			if(_currentStabilizerMotor.Equals(_motorX))
				_currentStabilizerMotor = _motorY;
			else if(_currentStabilizerMotor.Equals(_motorY))
				_currentStabilizerMotor = _motorZ;
			else if(_currentStabilizerMotor.Equals(_motorZ))
				_currentStabilizerMotor = _motorX;

			for (int i=0; i<TOTAL_LAST_MAGNITUDES; i++)
				_lastMagnitudes [i] = 9999;

			_timeToMotorChange = MAX_TIME_BEFORE_MOTOR_CHANGE;
		}
	}
	
	private void setMotorActive(GameObject motor, bool active, bool forward)
	{
		motor.renderer.material.color = active ? Color.green:Color.white;
		
		if (active) {
			Vector3 torque = Vector3.forward;
			float mult = forward?MotorForce:-MotorForce;
			
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


	void OnMouseDown()
	{
		_stabilizing = true;
	}

}
