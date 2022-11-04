using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LobbyPlayerPanel : MonoBehaviour
{
    [SerializeField] public TMPro.TMP_Text txtName;
    [SerializeField] public TMPro.TMP_Text txtReady;
    [SerializeField] public GameObject pnlColor;

    private bool isReady = false;

    public void Start()
    {
        SetReady(isReady);
    }

    public void SetName(string newName) {
        txtName.text = newName;
    }

    public string GetName() {
        return txtName.text;
    }

    public void SetColor(Color c) {
        pnlColor.GetComponent<Image>().color = c;
    }

    public void SetReady(bool ready)
    {
        isReady = ready;
        if (isReady)
        {
            txtReady.text = "Ready";
        }
        else
        {
            txtReady.text = "Not Ready";
        }
    }
}