
using UnityEngine;

public class CoinController : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;
    [ContextMenu("Generate Guid for Id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [SerializeField] private int coinValue;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Collider coinCollider;
    [SerializeField] private GameObject visual;

    private bool collected = false;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            if (!collected)
            {
                player.CollectCoin(coinValue);
                collected = true;
                visual.SetActive(false);
            }
        }
    }

    public void LoadData(GameData data)
    {
        data.coinsCollected.TryGetValue(id,out collected);
        if (collected)
        {
            visual.SetActive(false);
        }
    }

    public void SaveData(GameData data)
    {
        if (data.coinsCollected.ContainsKey(id))
        {
            data.coinsCollected.Remove(id);
        }
        data.coinsCollected.Add(id, collected);
    }
}
