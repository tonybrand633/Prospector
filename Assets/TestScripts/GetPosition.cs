using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPosition : MonoBehaviour
{
    public void OnMouseUpAsButton()
    {
        Vector3 pos = Input.mousePosition;
        Test.T.MoveToTheMousePosition(pos);
        Text3.T3.MoveToTheMousePosition(pos);
        //Debug.Log("MouseDown"+pos);
    }
}
