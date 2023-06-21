using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private Text playtime;
    [SerializeField] private Text levelName;
    private Button saveSlotButton;
    private bool hasData = true;
    [SerializeField] private bool selected = false;
    #endregion

    #region FIELDS
    [SerializeField] private string profileID = "";
    public bool HasData { get { return hasData; } }
    public bool Selected { get { return selected; } set { selected = value; } }
    #endregion

    private void Start()
    {
        // Some references set in Inspector
        saveSlotButton = GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        // Check if data exists for this save slot
        if(data == null)
        {
            // If there is no data, then set noDataContent UI
            hasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        } else
        {
            // If there is, then set hasDataContent UI
            hasData = true;
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            levelName.text = data.levelName;
            playtime.text = data.playtime;
        }
    }

    public string GetProfileID()
    {
        return profileID;
    }

    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
    }
}
