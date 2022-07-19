using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject prefabBottomDesk;
    [SerializeField] private GameObject prefabBorder;
    [SerializeField] private GameObject prefabItem;
    private int fieldWidthInUnit = 5;
    private int fieldHeightInUnit = 3;
    private int fullFieldWidth = 10;
    private int fullFieldHeight = 10;
    private float borderWidth = 0.2f;
    private float borderHeight = 0.5f;
    private List<Vector3> spanPoints = new List<Vector3>();
    private Vector3 gameRectangleLeftTop;
    private Vector3 gameRectangleBottomRight;

    // Start is called before the first frame update
    private void Start()
    {
        // DefineGameRectangle - should be first to define the available to game area 
        DefineGameRectangle();

        // make point to snap
        PopulateSnapPoints();

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

    private void DefineGameRectangle()
    {
        float aspectRatio = (float)fieldWidthInUnit / (float)fieldHeightInUnit;

        if (aspectRatio >= 1)
        {
            gameRectangleLeftTop = new Vector3(-fullFieldWidth, 0, fullFieldHeight / aspectRatio) / 2;
        }
        else
        {
            gameRectangleLeftTop = new Vector3(-fullFieldWidth * aspectRatio, 0, fullFieldHeight) / 2;
        }

        gameRectangleBottomRight = -gameRectangleLeftTop;
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
        for (int itemIndexX = 0; itemIndexX < fieldWidthInUnit; itemIndexX++)
        {
            for (int itemIndexZ = 0; itemIndexZ < fieldHeightInUnit; itemIndexZ++)
            {
                AddItemToSceneByIndex(itemIndexX, itemIndexZ);
            }
        }
    }

    private void AddItemToSceneByIndex(int indexX, int indexZ)
    {
        Vector3 itemSize = GetItemSize();

        float itemWidth = itemSize.x;
        float itemHeight = itemSize.z;
        float itemSizeY = itemSize.y;
        float xPosition = indexX * itemWidth + itemWidth / 2f + gameRectangleLeftTop.x;
        float zPosition = indexZ * itemHeight + itemHeight / 2f - gameRectangleLeftTop.z;
        float spaceBetweenItems = Mathf.Max(itemHeight, itemWidth) / 50;

        GameObject item = Instantiate(prefabItem);
        item.transform.position = new Vector3(
            xPosition,
            2 + indexZ * fieldWidthInUnit + indexX,
            -zPosition
            );
        item.transform.localScale = new Vector3(
            itemWidth - spaceBetweenItems,
            itemSizeY,
            itemHeight - spaceBetweenItems
        );

        item.GetComponent<Item>().defaultPosition = new Vector3(
            xPosition,
            itemSizeY / 2,
            -zPosition
            );

        if (indexX + 1 == fieldWidthInUnit && indexZ + 1 == fieldHeightInUnit)
        {
            item.SetActive(false);
        }
    }

    private void PopulateSnapPoints()
    {
        Vector3 itemSize = GetItemSize();

        float itemWidth = itemSize.x;
        float itemHeight = itemSize.z;
        float yPosition = itemSize.y / 2;

        for (int itemIndexX = 0; itemIndexX < fieldWidthInUnit; itemIndexX++)
        {
            for (int itemIndexZ = 0; itemIndexZ < fieldHeightInUnit; itemIndexZ++)
            {
                float xPosition = itemIndexX * itemWidth + itemWidth / 2f + gameRectangleLeftTop.x;
                float zPosition = itemIndexZ * itemHeight + itemHeight / 2f - gameRectangleLeftTop.z;

                // make smth like this
                // [[0,0], [0,1], [0,2]],
                // [[1,0], [1,1], [1,2]],
                // [[2,0], [2,1], [2,2]],
                spanPoints.Add(new Vector3(xPosition, yPosition, -zPosition));
            }
        }
    }

    private Vector3 GetItemSize()
    {
        // 3f - actually magic number, just make item more plan
        float heightCoefficient = 3f;
        float itemSizeX = -gameRectangleLeftTop.x / (float)fieldWidthInUnit * 2;
        float itemSizeZ = gameRectangleLeftTop.z / (float)fieldHeightInUnit * 2;
        float itemSizeY = Mathf.Max(itemSizeX, itemSizeX) / heightCoefficient;

        return new Vector3(itemSizeX, itemSizeY, itemSizeZ);
    }
}