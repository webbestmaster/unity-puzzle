using UnityEngine;

public class Item : MonoBehaviour
{
    private Rigidbody rigitbody;
    private bool isActive;
    private Camera mainCamera;

    // Start is called before the first frame update
    private void Start()
    {
        rigitbody = GetComponent<Rigidbody>();
        mainCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3? pointer = GetPointPosition();

        if (!Input.GetMouseButton(0))
        {
            isActive = false;
        }

        if (pointer != null && isActive)
        {
            Debug.Log("pointer - " + pointer.Value);
            rigitbody
                .AddForce(
                    (pointer.Value - gameObject.transform.position).normalized
                    * Time.deltaTime * 1000,
                    ForceMode.Force
                );
        }
    }

    private Vector3? GetPointPosition()
    {
        Debug.Log("Input.touchCount: " + Input.touchCount);
        Debug.Log("Input.mousePosition.x: " + Input.mousePosition.x);

        /*
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            return m_MainCamera.ScreenToWorldPoint(
                new Vector3(
                    touch.position.x,
                    touch.position.y,
                    m_MainCamera.transform.position.y
                )
            );
        }
        */

        if (Input.GetMouseButton(0))
        {
            return mainCamera
                .ScreenToWorldPoint(
                    new Vector3(
                        Input.mousePosition.x,
                        Input.mousePosition.y,
                        mainCamera.transform.position.y
                    )
                );
        }

        return null;
    }
    
    void OnMouseDown()
    {
        // Destroy the gameObject after clicking on it
        isActive = true;
    }
}