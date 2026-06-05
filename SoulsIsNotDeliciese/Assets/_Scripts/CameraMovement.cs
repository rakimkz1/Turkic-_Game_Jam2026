using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector2 movementSpeed;
    public Vector3 rotateLimits;
    public AnimationCurve movementCurve;

    private Vector3 initialRotate;
    private Vector3 intialPosition;
    private void Start()
    {
        intialPosition = transform.position;
        initialRotate = transform.eulerAngles;
    }

    private void Update()
    {
        Vector3 movePostion = intialPosition;
        Vector3 rotateRotation = initialRotate;
        float xMovement = Input.mousePosition.x / Screen.width;
        float yMovement = Input.mousePosition.y / Screen.height;
        movePostion += Vector3.right * movementSpeed.x * movementCurve.Evaluate(xMovement);
        movePostion += Vector3.up * movementSpeed.y * movementCurve.Evaluate(yMovement);
        rotateRotation += Vector3.up * rotateLimits.x * movementCurve.Evaluate(xMovement);
        rotateRotation += -Vector3.right * rotateLimits.y * movementCurve.Evaluate(yMovement);
        rotateRotation += Vector3.forward * rotateLimits.z * movementCurve.Evaluate(xMovement);
        transform.position = movePostion;
        transform.eulerAngles = rotateRotation;
    }
}
