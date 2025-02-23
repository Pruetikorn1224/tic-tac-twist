using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;  // The point to rotate around (e.g., origin)

    GameEvent gameEventScript;

    private Vector3 previousMousePosition;

    private bool isAbleToDrag = true;

    private int cubeLength;

    private void Start()
    {
        gameEventScript = GameObject.Find("Game Event").GetComponent<GameEvent>();

        cubeLength = (int)Mathf.Pow(target.childCount, 1f/3f);

        transform.position = new Vector3(0f, 0f, 8f);
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    private void Update()
    {
        if (gameEventScript.gameState == 1)
        {
            if (gameEventScript.controlTypeMouse)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider != null)
                        {
                            isAbleToDrag = false;
                        }
                        else
                        {
                            isAbleToDrag = true;
                        }
                    }
                }

                // On left mouse button drag
                if (Input.GetMouseButton(0) && isAbleToDrag)
                {
                    // Get current mouse position
                    Vector3 currentMousePosition = Input.mousePosition;

                    // If it's not the first frame of dragging
                    if (previousMousePosition != Vector3.zero)
                    {
                        // Calculate the difference in mouse movement
                        Vector3 delta = currentMousePosition - previousMousePosition;

                        // Rotate the camera based on the mouse movement
                        float rotationX = -delta.x * gameEventScript.sensitivity * Time.deltaTime;  // Invert horizontal rotation
                        float rotationY = -delta.y * gameEventScript.sensitivity * Time.deltaTime;

                        // Apply the rotation around the target
                        transform.RotateAround(target.position, Vector3.up, rotationX);  // Horizontal rotation (Y-axis)
                        transform.RotateAround(target.position, transform.right, rotationY);  // Vertical rotation (X-axis)
                    }

                    // Update previous mouse position
                    previousMousePosition = currentMousePosition;
                }
                else
                {
                    // Reset previous mouse position when the mouse button is not pressed
                    previousMousePosition = Vector3.zero;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    isAbleToDrag = true;
                }
            }
            else
            {
                float horizontalRotate = Input.GetAxis("Horizontal") * gameEventScript.sensitivity * 10f * Time.deltaTime;
                float verticalRotate = Input.GetAxis("Vertical") * gameEventScript.sensitivity * 10f * Time.deltaTime;

                transform.RotateAround(target.position, Vector3.up, horizontalRotate);  // Horizontal rotation (Y-axis)
                transform.RotateAround(target.position, transform.right, verticalRotate);  // Vertical rotation (X-axis)
            }

            float distance = Vector3.Distance(transform.position, target.position);
            float scrollInput = Input.mouseScrollDelta.y;

            if (scrollInput > 0 && distance > 1.5f * cubeLength)
                transform.position += transform.forward * scrollInput * gameEventScript.sensitivity / 5f;
            else if (scrollInput < 0 && distance < 3f * cubeLength)
                transform.position += transform.forward * scrollInput * gameEventScript.sensitivity / 5f;
        }
    }
}
