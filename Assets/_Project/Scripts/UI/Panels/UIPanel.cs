using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using tiplay.DatabaseSystem;
using tiplay;

public class UIPanel : MonoBehaviour
{
    protected Database database;
    [SerializeField] protected Button button;
    [SerializeField] protected GameObject panelWrapper;

    protected bool buttonClicked = false;

    void Awake()
    {
        database = GlobalData.GetInstance().database;  
        OnAwake();
    }

    public virtual void OnAwake()
    {
       

    }
    public virtual void Open()
    {
        panelWrapper.SetActive(true);
    }

    public void Close()
    {
        panelWrapper.SetActive(false);
    }

    public virtual void ButtonClicked()
    {
    }
}
