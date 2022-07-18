using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject bottomDesk;
    [SerializeField] private GameObject border;

    // Start is called before the first frame update
    private void Start()
    {
        MakeBottomDask();
        MakeBorders();
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 scale = Helper.getScaleForSize(bottomDesk, new Vector3(16, 1, 16));
        Debug.Log("scale: " + scale);
    }

    private void MakeBorders()
    {
        GameObject topBorder = Instantiate(border);
        topBorder.transform.localScale = new Vector3(10, 1, 1);
        topBorder.transform.position = new Vector3(0, 0, 4.5f);
        
        GameObject bottomBorder = Instantiate(border);
        bottomBorder.transform.localScale = new Vector3(10, 1, 1);
        bottomBorder.transform.position = new Vector3(0, 0, -4.5f);
        
        GameObject leftBorder = Instantiate(border);
        leftBorder.transform.localScale = new Vector3(1, 1, 10);
        leftBorder.transform.position = new Vector3(-4.5f, 0, 0);
        
        GameObject rightBorder = Instantiate(border);
        rightBorder.transform.localScale = new Vector3(1, 1, 10);
        rightBorder.transform.position = new Vector3(4.5f, 0, 0);
    }
    private void MakeBottomDask()
    {
        Instantiate(bottomDesk);
    }
}
