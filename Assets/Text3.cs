using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Text3 : MonoBehaviour
{
    public static Text3 T3;

    void Awake()
    {
        T3 = this;
    }

    public void MoveToTheMousePosition(Vector3 pos)
    {
        this.transform.position = pos;
        Debug.Log("Text3Pos:" + pos);
    }
}
