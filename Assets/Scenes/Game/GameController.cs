using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject prefabBottomDesk;
    [SerializeField] private GameObject prefabBorder;
    [SerializeField] private GameObject prefabItem;
    [SerializeField] private GameObject GameUiManager;
    private int fieldWidthInUnit = 3;
    private int fieldHeightInUnit = 3;
    private float fullFieldWidth = 10f;
    private float fullFieldHeight = 10f;
    private float borderWidth = 0.2f;
    private float borderHeight = 0.5f;
    private List<Vector3> spanPointList = new List<Vector3>();
    private List<GameObject> itemList = new List<GameObject>();
    private Vector3 gameRectangleLeftTop;
    private Vector3 gameRectangleBottomRight;
    private Texture mainTexture;
    private bool isGameStarted = false;

    // Start is called before the first frame update
    private void Start()
    {
        fieldWidthInUnit = PlayerPrefs.GetInt(SavedDataKey.WidthInUnit);
        fieldHeightInUnit = PlayerPrefs.GetInt(SavedDataKey.HeightInUnit);

        String spriteName = PlayerPrefs.GetString(SavedDataKey.SpriteName);
        
        Sprite sprite = Resources.Load<Sprite>("Cat/" + spriteName);
        
        mainTexture = sprite.texture;
        
        // DefineGameRectangle - should be first to define the available to game area 
        DefineGameRectangle();

        // make point to snap
        PopulateSnapPoints();

        AddBottomDaskToScene();
        AddBordersToScene();
        AddItemsToScene();

        for (int i = 0; i < 10; i++)
        {
            ItemMoveRandom();
        }

        isGameStarted = true;
    }

    private void DefineGameRectangle()
    {
        float aspectRatio = (float)fieldWidthInUnit / fieldHeightInUnit;

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
        float aspectRatio = (float)fieldWidthInUnit / fieldHeightInUnit;

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

        float aspectRatio = (float)fieldWidthInUnit / fieldHeightInUnit;

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

        Item itemScript = item.GetComponent<Item>();
        
        itemScript.spanPointList = spanPointList;
        itemScript.cellSize = itemSize;
        itemScript.itemList = itemList;
        itemScript.OnGameEnd = OnGameEnd;

        Transform textureHolder = item.transform.Find("TextureHolder");
        Material material = textureHolder.GetComponent<Renderer>().material;
        material.mainTexture = mainTexture;

        material.mainTextureScale = new Vector2(1f / fieldWidthInUnit, 1f / fieldHeightInUnit);

        material.mainTextureOffset = new Vector2(
            1f / fieldWidthInUnit * indexX,
            1f / fieldHeightInUnit * ((float)fieldHeightInUnit - indexZ - 1)
        );

        return item;
    }

    public void OnGameEnd()
    {
        GameUiManager.GetComponent<GameUiManager>().gameOverPopupOpen.SetValue(true);

        Debug.Log("GameController OnGameEnd");
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
        float itemSizeX = Mathf.Abs(gameRectangleLeftTop.x) / fieldWidthInUnit * 2;
        float itemSizeZ = Mathf.Abs(gameRectangleLeftTop.z) / fieldHeightInUnit * 2;
        float itemSizeY = borderHeight * 1.2f;

        return new Vector3(itemSizeX, itemSizeY, itemSizeZ);
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
        Vector3 itemSize = GetItemSize();

        // less than 0.01% of diagonal
        float minDistance = itemSize.magnitude / 10000;

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

    [CanBeNull]
    private GameObject GetItemByPosition(Vector3 itemPosition)
    {
        Vector3 itemSize = GetItemSize();

        // less than 0.01% of diagonal
        float minDistance = itemSize.magnitude / 10000;

        foreach (GameObject item in itemList)
        {
            float distance = Vector3.Distance(itemPosition, item.transform.position);

            if (distance < minDistance && item.activeSelf)
            {
                return item;
            }
        }

        return null;
    }

    private bool ItemMove(int xUnit, int zUnit)
    {
        Vector3 freeSnapPoint = GetFreeSpanPoint();
        Vector3 itemSize = GetItemSize();
        Vector3 deltaVector = new Vector3(itemSize.x * xUnit, 0f, itemSize.z * zUnit);
        Vector3 itemPosition = freeSnapPoint + deltaVector;

        GameObject? item = GetItemByPosition(itemPosition);

        if (item != null)
        {
            item.transform.position -= deltaVector;
            return true;
        }

        return false;
    }

    private bool ItemMoveRandom()
    {
        int direction = Random.Range(0, 4);

        switch (direction)
        {
            case 0:
                // Up
                return ItemMove(0, -1);
            case 1:
                // Right
                return ItemMove(-1, 0);
            case 2:
                // Down
                return ItemMove(0, 1);
            case 3:
                // Left
                return ItemMove(1, 0);
            default:
                Debug.LogWarning("Can not detect direction: " + direction);
                break;
        }

        return false;
    }
}