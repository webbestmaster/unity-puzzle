using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectImage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleSelectImage()
    {
        Image image = GetComponent<Image>();
        Debug.Log(image.sprite.rect.position);

        SceneManager.LoadScene("Game");

    }
}
