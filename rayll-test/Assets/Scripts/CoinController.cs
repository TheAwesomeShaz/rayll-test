
using UnityEngine;

public class CoinController : MonoBehaviour
{
    [SerializeField] private int coinValue;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Collider coinCollider;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            player.CollectCoin(coinValue);
            Destroy(gameObject);
        }
    }
}
