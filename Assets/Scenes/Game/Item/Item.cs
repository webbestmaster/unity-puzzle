using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    private Rigidbody rigitbody;

    [SerializeField, Header("List of stone move sound")]
    private AudioClip[] audioClipList;

    private bool isActive;
    private Camera mainCamera;
    public Vector3 defaultPosition;
    public List<Vector3> spanPointList = new List<Vector3>();
    public Vector3 cellSize;
    public List<GameObject> itemList = new List<GameObject>();
    private Vector3 startMovePoint;
    public Action OnGameEnd;
    private Vector3 endMovePoint;

    private void Start()
    {
        rigitbody = GetComponent<Rigidbody>();
        mainCamera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        if (isActive == false)
        {
            return;
        }

        Vector3? pointer = GetPointPosition();

        if (pointer == null)
        {
            return;
        }


        float minX = Mathf.Min(startMovePoint.x, endMovePoint.x);
        float maxX = Mathf.Max(startMovePoint.x, endMovePoint.x);
        float minZ = Mathf.Min(startMovePoint.z, endMovePoint.z);
        float maxZ = Mathf.Max(startMovePoint.z, endMovePoint.z);

        float endX = Helper.limitFloat(minX, pointer.Value.x, maxX);
        float endZ = Helper.limitFloat(minZ, pointer.Value.z, maxZ);

        gameObject.transform.position = new Vector3(
            endX,
            gameObject.transform.position.y,
            endZ
        );
    }

    private void PlaySshhh()
    {
        AudioSource audio = GetComponent<AudioSource>();

        if (audio != null && audio.isPlaying != true)
        {
            audio.clip = audioClipList[Random.Range(0, audioClipList.Length)];
            audio.Play();
        }
    }
    
    private Vector3? GetPointPosition()
    {
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
        startMovePoint = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z
        );
        endMovePoint = GetFreeSpanPoint();

        if ((startMovePoint - endMovePoint).magnitude > cellSize.x * 1.01)
        {
            return;
        }

        isActive = true;

        // PlaySshhh();
        
        Outline outline = gameObject.GetComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = new Color(1f, 1f, 1f, 1f);
        outline.OutlineWidth = 4f;
    }

    void OnMouseUp()
    {
        isActive = false;

        transform.position = GetNearestSnapPoint();

        Outline outline = gameObject.GetComponent<Outline>();
        outline.OutlineWidth = 0f;

        // PlaySshhh();

        if (GetIsPuzzleSolved())
        {
            OnGameEnd();
        }
    }

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

    private bool GetIsPuzzleSolved()
    {
        Debug.Log("isPuzzleSolved");

        foreach (GameObject item in itemList)
        {
            if (item.GetComponent<Item>().GetIsOnDefaultPlace() == false)
            {
                return false;
            }
        }

        return true;
    }

    public bool GetIsOnDefaultPlace()
    {
        return (defaultPosition - transform.position).magnitude < (cellSize.magnitude * 0.001);
    }
}