using UnityEngine;

public class Item : MonoBehaviour
{
    private Rigidbody rigitbody;
    [SerializeField] private Camera m_MainCamera;

    // Start is called before the first frame update
    private void Start()
    {
        rigitbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3? pointer = GetPointPosition();

        if (pointer != null)
        {
            Debug.Log("pointer - " + pointer.Value);
            rigitbody.AddForce(
                (pointer.Value - gameObject.transform.position).normalized
                * Time.deltaTime * 1000
            );
        }
    }

    private Vector3? GetPointPosition()
    {
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

        if (Input.GetMouseButton(0))
        {
            return m_MainCamera
                .ScreenToWorldPoint(
                    new Vector3(
                        Input.mousePosition.x,
                        Input.mousePosition.y,
                        m_MainCamera.transform.position.y
                    )
                );
        }

        return null;
    }
}