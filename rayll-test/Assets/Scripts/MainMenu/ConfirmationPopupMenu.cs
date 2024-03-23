using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationPopupMenu : Menu
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public void ActivateMenu(string displayString, UnityAction confirmAction, UnityAction cancelAction)
    {
        gameObject.SetActive(true);
        displayText.text = displayString;

        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            confirmAction();
        });
        
        cancelButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            cancelAction();
        });

    }

    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }
}
