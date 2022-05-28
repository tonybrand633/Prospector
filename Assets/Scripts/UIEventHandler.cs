using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIEventHandler : MonoBehaviour, IPointerClickHandler
{
    public delegate void UIEventProxy(bool gameRes);

    public event UIEventProxy OnClickWin;

    public event UIEventProxy OnClickLose;

    //public event UIEventProxy OnEnter;

    //public event UIEventProxy OnExit;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickWin != null && eventData.button == PointerEventData.InputButton.Left)
        {
            //PointerEventData.InputButton temp = eventData.button;
            Debug.Log("WinClick");
            OnClickWin(true);
        } else if (OnClickLose!= null && eventData.button == PointerEventData.InputButton.Left) 
        {
            OnClickLose(false);
        }

    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    //if (OnEnter!=null) 
    //    //{
    //    //    OnEnter();
    //    //}
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    //if (OnExit!=null) 
    //    //{            
    //    //    //Debug.Log("Exit Time:" + time);
    //    //    OnExit();
    //    //}
    //}
}
