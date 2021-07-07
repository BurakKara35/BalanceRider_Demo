using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour
{

	private WheelCollider[] wheels;

	[SerializeField] private float maxAngle = 30;
	[SerializeField] private float maxTorque = 300;
	[SerializeField] private GameObject wheelShape;

	private float horizontal = 0;
	private float vertical = 0;

    private bool swipe = false;
	private bool swipeFinished = true;
	private float swipeFirstPosition;
	private float differenceBetweenSwipePositions;
	private float swipingInSeconds = 0.1f;
	private IEnumerator swipeCoroutine;

	private enum Direction { None, Left, Right }
	private Direction direction;

	[SerializeField] private Transform chassis;

	private void Start()
	{
		AttachWheels();
	}

	private void Update()
	{
		InputHandler();
	}

    private void FixedUpdate()
    {
		Movement();
    }

    private void AttachWheels()
    {
		wheels = GetComponentsInChildren<WheelCollider>();

		for (int i = 0; i < wheels.Length; ++i)
		{
			var wheel = wheels[i];

			// create wheel shapes only when needed
			if (wheelShape != null)
			{
				var ws = GameObject.Instantiate(wheelShape);
				ws.transform.parent = wheel.transform;
			}
		}
	}

	private void InputHandler()
    {
        if (Input.GetMouseButtonDown(0))
		{
			swipe = true;
			swipeFirstPosition = Input.mousePosition.x;
			swipeCoroutine = Swiping();
			StartCoroutine(swipeCoroutine);
		}
		if (Input.GetMouseButton(0) && swipe)
		{
			swipeFinished = false;
		}
		if (Input.GetMouseButtonUp(0))
		{
			swipe = false;
			swipeFinished = true;
			direction = Direction.None;
			StopCoroutine(swipeCoroutine);
		}

		if (direction == Direction.Left)
			TurnLeft();
		else if (direction == Direction.Right)
			TurnRight();
		else
			ResetWheelsAndChassis();
	}

	public IEnumerator Swiping()
	{
		yield return new WaitForSeconds(swipingInSeconds);
		if (swipe)
		{
			differenceBetweenSwipePositions = Input.mousePosition.x - swipeFirstPosition;

			if (differenceBetweenSwipePositions < 0)
				direction = Direction.Left;
			else if (differenceBetweenSwipePositions > 0)
				direction = Direction.Right;

			if (!swipeFinished)
			{
				swipeCoroutine = Swiping();
				StartCoroutine(swipeCoroutine);
			}
		}
	}

	private void Movement()
    {
		float angle = maxAngle * horizontal;
		float torque = maxTorque * VerticalMovementValue();

		foreach (WheelCollider wheel in wheels)
		{
			// a simple car where front wheels steer while rear ones drive
			if (wheel.transform.localPosition.z > 0)
				wheel.steerAngle = angle;

			if (wheel.transform.localPosition.z < 0)
				wheel.motorTorque = torque;

			// update visual wheels if any
			if (wheelShape)
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose(out p, out q);

				// assume that the only child of the wheelcollider is the wheel shape
				Transform shapeTransform = wheel.transform.GetChild(0);
				shapeTransform.position = p;
				shapeTransform.rotation = q;
			}

		}
	}

	private void TurnLeft()
    {
		if (horizontal > -1)
        {
			horizontal -= Time.fixedDeltaTime * 2;
			chassis.Rotate(new Vector3(0, 0, horizontal));
		}
	}

	private void TurnRight()
	{
		if (horizontal < 1)
        {
			horizontal += Time.fixedDeltaTime * 2;
			chassis.Rotate(new Vector3(0, 0, horizontal));
		}
	}

	private float VerticalMovementValue()
    {
		if (vertical < 1)
			return vertical += Time.fixedDeltaTime * 2;
		else
			return 1;
    }

	private void ResetWheelsAndChassis()
    {
		if (horizontal < -0.1)
		{
			horizontal += Time.fixedDeltaTime * 2;
			chassis.Rotate(new Vector3(0, 0, -horizontal));
		}
		else if (horizontal > 0.1)
		{
			horizontal -= Time.fixedDeltaTime * 2;
			chassis.Rotate(new Vector3(0, 0, -horizontal));
		}
		else
        {
			horizontal = 0;
			chassis.localRotation = Quaternion.identity;
		}
	}
}