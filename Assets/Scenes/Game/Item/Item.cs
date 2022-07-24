using System;
using UnityEngine;
using System.Collections.Generic;

public class Item : MonoBehaviour
{
    private Rigidbody rigitbody;
    private bool isActive;
    private Camera mainCamera;
    public Vector3 defaultPosition;
    public List<Vector3> spanPointList = new List<Vector3>();
    public Vector3 cellSize;
    public List<GameObject> itemList = new List<GameObject>();
    private bool isHorizontalMoveAvailable;
    private bool isVerticalMoveAvailable;
    private Vector3 startMovePoint;
    private Vector3 endMovePoint;
    // private Transform textureHolder;
    // [SerializeField] private Texture mainTexture1;

    // Start is called before the first frame update
    private void Start()
    {
        DefineTexture();

        // textureHolder
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

        if (pointer != null && isActive && (isHorizontalMoveAvailable || isVerticalMoveAvailable))
        {
            // Debug.Log("pos " + transform.position);
            // Debug.Log("def " + defaultPosition);
            Vector3 moveVector =
                isHorizontalMoveAvailable
                    ? new Vector3(pointer.Value.x, transform.position.y, transform.position.z)
                    : new Vector3(transform.position.x, transform.position.y, pointer.Value.z);

            if (isHorizontalMoveAvailable)
            {
                float minPositionX = Mathf.Min(startMovePoint.x, endMovePoint.x);
                float maxPositionX = Mathf.Max(startMovePoint.x, endMovePoint.x);

                if (moveVector.x > maxPositionX || moveVector.x < minPositionX)
                {
                    return;
                }
            }
            else
            {
                float minPositionZ = Mathf.Min(startMovePoint.z, endMovePoint.z);
                float maxPositionZ = Mathf.Max(startMovePoint.z, endMovePoint.z);

                if (moveVector.z > maxPositionZ || moveVector.z < minPositionZ)
                {
                    return;
                }
            }

            rigitbody.MovePosition(moveVector);
        }
    }

    private Vector3? GetPointPosition()
    {
        // Debug.Log("Input.touchCount: " + Input.touchCount);
        // Debug.Log("Input.mousePosition.x: " + Input.mousePosition.x);

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
        isHorizontalMoveAvailable = GetIsHorizontalMoveAvailable();
        isVerticalMoveAvailable = GetIsVerticalMoveAvailable();

        startMovePoint = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z
        );
        endMovePoint = GetFreeSpanPoint();

        // Destroy the gameObject after clicking on it
        isActive = true;
    }

    void OnMouseUp()
    {
        isHorizontalMoveAvailable = false;
        isVerticalMoveAvailable = false;

        // Destroy the gameObject after clicking on it
        isActive = true;

        transform.position = GetNearestSnapPoint();
    }

    // void OnMouseExit()
    // {
    //     OnMouseUp();
    // }

    // private void OnMouseEnter()
    // {
        // OnMouseDown();
    // }

    public Vector3 GetNearestSnapPoint()
    {
        Vector3 candidat = new Vector3(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue);

        foreach (Vector3 snapPoint in spanPointList)
        {
            if (Vector3.Distance(transform.position, snapPoint) < Vector3.Distance(transform.position, candidat))
            {
                candidat = snapPoint;
            }
        }

        return candidat;
    }

    private bool GetIsHorizontalMoveAvailable()
    {
        Vector3 freeSnapPoint = GetFreeSpanPoint();

        if (Mathf.Abs(transform.position.x - freeSnapPoint.x) < cellSize.x * 1.01 &&
            Mathf.Abs(transform.position.z - freeSnapPoint.z) < cellSize.z * 0.01)
        {
            return true;
        }

        return false;
    }

    private bool GetIsVerticalMoveAvailable()
    {
        Vector3 freeSnapPoint = GetFreeSpanPoint();

        if (Mathf.Abs(transform.position.x - freeSnapPoint.x) < cellSize.x * 0.01 &&
            Mathf.Abs(transform.position.z - freeSnapPoint.z) < cellSize.z * 1.01)
        {
            return true;
        }

        return false;
    }

    private Vector3 GetFreeSpanPoint()
    {
        foreach (Vector3 snapPoint in spanPointList)
        {
            if (!GetIsSpanPointUnderItem(snapPoint))
            {
                return snapPoint;
            }
        }

        return new Vector3(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue);
    }

    private bool GetIsSpanPointUnderItem(Vector3 spanPoint)
    {
        // less than 0.01% of diagonal
        float minDistance = cellSize.magnitude / 10000;

        foreach (GameObject item in itemList)
        {
            float distance = Vector3.Distance(spanPoint, item.transform.position);

            if (distance < minDistance && item.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    private void DefineTexture()
    {
        // textureHolder = transform.Find("TextureHolder");
        // Material material = textureHolder.GetComponent<Renderer>().material;
        // material.mainTexture = mainTexture1;

        // material.mainTextureScale = new Vector2(2f, 2f);
        // material.mainTextureOffset = new Vector2(UnityEngine.Random.Range(0f, 10f), UnityEngine.Random.Range(0f, 10f));
    }
}