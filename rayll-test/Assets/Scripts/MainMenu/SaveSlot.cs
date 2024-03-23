using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profiles")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TMP_Text percentCompleteText;

    public bool HasData { get; private set; } = false;
    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        if(data == null)
        {
            HasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        else
        {
            HasData = true;
            noDataContent.SetActive(false) ;
            hasDataContent.SetActive(true);

            percentCompleteText.text = data.GetPercentageCompleted() + "% complete";
        }
    }

    public string GetProfileId()
    {
        return this.profileId;
    }

    public void SetInteractible(bool isInteractible)
    {
        saveSlotButton.interactable = isInteractible;
    }

}
