using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject prefabBottomDesk;
    [SerializeField] private GameObject prefabBorder;
    [SerializeField] private GameObject prefabItem;
    private int fieldWidthInUnit = 16;
    private int fieldHeightInUnit = 8;
    private float fullFieldWidth = 10f;
    private float fullFieldHeight = 10f;
    private float borderWidth = 0.2f;
    private float borderHeight = 0.5f;
    private List<Vector3> spanPointList = new List<Vector3>();
    private List<GameObject> itemList = new List<GameObject>();
    private Vector3 gameRectangleLeftTop;
    private Vector3 gameRectangleBottomRight;
    [SerializeField] private Texture mainTexture1;

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
        
        Material material = bottomDesk.GetComponent<Renderer>().material;
        material.mainTexture = mainTexture1;

        material.mainTextureScale = new Vector2(-1, -1);
    }

    private void AddItemsToScene()
    {
        for (int itemIndexX = 0; itemIndexX < fieldWidthInUnit; itemIndexX++)
        {
            for (int itemIndexZ = 0; itemIndexZ < fieldHeightInUnit; itemIndexZ++)
            {
                GameObject item = AddItemToSceneByIndex(itemIndexX, itemIndexZ);
                itemList.Add(item);
            }
        }
    }

    private GameObject AddItemToSceneByIndex(int indexX, int indexZ)
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
            itemSizeY / 2, // 2 + indexZ * fieldWidthInUnit + indexX,
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

        item.GetComponent<Item>().spanPointList = spanPointList;
        item.GetComponent<Item>().cellSize = itemSize;
        item.GetComponent<Item>().itemList = itemList;

        Transform textureHolder = item.transform.Find("TextureHolder");
        Material material = textureHolder.GetComponent<Renderer>().material;
        material.mainTexture = mainTexture1;

        material.mainTextureScale = new Vector2(1f / (float)fieldWidthInUnit, 1f / (float)fieldHeightInUnit);

        material.mainTextureOffset = new Vector2(
            1f / (float)fieldWidthInUnit * (float)indexX,
            1f / (float)fieldHeightInUnit * ((float)fieldHeightInUnit - indexZ - 1)
        );

        return item;
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

                spanPointList.Add(new Vector3(-xPosition, yPosition, -zPosition));
            }
        }
    }

    private Vector3 GetItemSize()
    {
        // 3f - actually magic number, just make item more plan
        float heightCoefficient = 3f;
        float itemSizeX = Mathf.Abs(gameRectangleLeftTop.x) / (float)fieldWidthInUnit * 2;
        float itemSizeZ = Mathf.Abs(gameRectangleLeftTop.z) / (float)fieldHeightInUnit * 2;
        float itemSizeY = Mathf.Max(itemSizeX, itemSizeZ) / heightCoefficient;

        return new Vector3(itemSizeX, itemSizeY, itemSizeZ);
    }
}