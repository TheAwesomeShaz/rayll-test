using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    // We can use event handlers here but since this is simple we go with Action
    public event Action<int> OnCoinCollected;


    [SerializeField] private NavMeshAgent agent;
    private Camera mCamera;
    private RaycastHit mHit;

    private void Start()
    {
        mCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out mHit))
            {
                agent.SetDestination(mHit.point);
            }
        }
    }

    public void CollectCoin(int score)
    {
        OnCoinCollected?.Invoke(score);
    }

    public void LoadData(GameData data)
    {
        transform.position = data.playerPosition;
        transform.rotation = Quaternion.Euler(data.playerRotation.x, data.playerRotation.y, data.playerRotation.z);
    }

    public void SaveData(GameData data)
    {
        data.playerPosition = transform.position;
        data.playerRotation = transform.rotation.eulerAngles;
    }
}
