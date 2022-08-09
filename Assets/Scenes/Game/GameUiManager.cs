using Unity.VisualScripting;
using UnityEngine;

public class GameUiManager : MonoBehaviour
{
    public GameObject gameOverCanvas;

    public ModelValue<bool> gameOverPopupOpen = new ModelValue<bool>(false);

    private void Start()
    {
        gameOverPopupOpen.AddListener(() => { gameOverCanvas.SetActive(gameOverPopupOpen.GetValue()); });
    }

    void OnDestroy()
    {
        gameOverPopupOpen.OnDestroy();
    }
}