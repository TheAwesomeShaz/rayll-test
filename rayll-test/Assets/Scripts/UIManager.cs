using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text coinsText;

    private void Start()
    {
        gameManager.OnUpdateCoinsUI += GameManager_OnUpdateCoinsUI;
    }

    private void GameManager_OnUpdateCoinsUI(string coins)
    {
        coinsText.text = $"Coins: {coins}";
    }
}
