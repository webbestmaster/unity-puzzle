using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject prefabBottomDesk;
    [SerializeField] private GameObject prefabBorder;
    [SerializeField] private GameObject prefabItem;
    private int fieldWidthInUnit = 3;
    private int fieldHeightInUnit = 5;
    private int fullFieldWidth = 10;
    private int fullFieldHeight = 10;
    private float borderWidth = 0.2f;
    private float borderHeight = 0.5f;

    // Start is called before the first frame update
    private void Start()
    {
        AddBottomDaskToScene();
        AddBordersToScene();
        AddItemsToScene();
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 scale = Helper.getScaleForSize(prefabBottomDesk, new Vector3(16, 1, 16));
        // Debug.Log("scale: " + scale);
    }

    private void AddBordersToScene()
    {
        float aspectRatio = (float)fieldWidthInUnit / (float)fieldHeightInUnit;

        GameObject topBorder = Instantiate(prefabBorder);
        GameObject bottomBorder = Instantiate(prefabBorder);
        GameObject leftBorder = Instantiate(prefabBorder);
        GameObject rightBorder = Instantiate(prefabBorder);

        if (aspectRatio >= 1)
        {
            topBorder.transform.localScale = new Vector3(fullFieldWidth + borderWidth * 2, borderHeight, borderWidth);
            topBorder.transform.position = new Vector3(
                0,
                borderHeight,
                fullFieldHeight / aspectRatio + borderWidth
            ) / 2;

            leftBorder.transform.localScale = new Vector3(borderWidth, borderHeight, fullFieldHeight / aspectRatio);
            leftBorder.transform.position = new Vector3(-fullFieldWidth - borderWidth, borderHeight, 0) / 2;
        }
        else
        {
            topBorder.transform.localScale =
                new Vector3(fullFieldWidth * aspectRatio + borderWidth * 2, borderHeight, borderWidth);
            topBorder.transform.position = new Vector3(
                0,
                borderHeight,
                fullFieldHeight + borderWidth
            ) / 2;

            leftBorder.transform.localScale = new Vector3(borderWidth, borderHeight, fullFieldHeight);
            leftBorder.transform.position =
                new Vector3(-fullFieldWidth * aspectRatio - borderWidth, borderHeight, 0) / 2;
        }

        bottomBorder.transform.localScale = topBorder.transform.localScale;
        bottomBorder.transform.position = new Vector3(
            topBorder.transform.position.x,
            topBorder.transform.position.y,
            -topBorder.transform.position.z
        );

        rightBorder.transform.localScale = leftBorder.transform.localScale;
        rightBorder.transform.position = new Vector3(
            -leftBorder.transform.position.x,
            leftBorder.transform.position.y,
            leftBorder.transform.position.z
        );
    }

    private void AddBottomDaskToScene()
    {
        GameObject bottomDesk = Instantiate(prefabBottomDesk);

        float aspectRatio = (float)fieldWidthInUnit / (float)fieldHeightInUnit;

        Vector3 neededSize = aspectRatio >= 1
            ? new Vector3(fullFieldWidth + borderWidth * 2, 1, fullFieldHeight / aspectRatio + borderWidth * 2)
            : new Vector3(fullFieldWidth * aspectRatio + borderWidth * 2, 1, fullFieldHeight + borderWidth * 2);
        Vector3 newScale = Helper.getScaleForSize(prefabBottomDesk, neededSize);

        bottomDesk.transform.localScale = newScale;
    }

    private void AddItemsToScene()
    {
        int itemCount = fieldWidthInUnit * fieldHeightInUnit - 1;

        for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
        {
            AddItemToSceneByIndex(itemIndex);
        }

        // Instantiate(prefabBottomDesk);
    }

    private void AddItemToSceneByIndex(int index)
    {
        Debug.Log(index);
    }
}