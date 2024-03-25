using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private GameObject loadingUI;

    private void Awake()
    {
        gameManager.OnUpdateCoinsUI += GameManager_OnUpdateCoinsUI;
    }

    private void GameManager_OnUpdateCoinsUI(string coins)
    {
        if (loadingUI.activeInHierarchy)
        {
            loadingUI.SetActive(false);
        }
        coinsText.text = $"Coins: {coins}";
    }
}
