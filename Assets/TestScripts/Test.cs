using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public static Test T;
    public GameObject TextPrefabs;
    public GameObject GamePrefabs;
    public Canvas canvas;

    void Awake()
    {
        T = this;    
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToTheMousePosition(Vector3 pos) 
    {
        //float newX = pos.x /= Screen.width;
        //float newY = pos.y /= Screen.height;
        //Vector3 gVector = new Vector3(newX, newY, 0);

        //GameObject go1 = Instantiate(GamePrefabs);

        //go1.transform.SetParent(canvas.transform);
        
        //go1.transform.position = pos;
        GameObject go = Instantiate(TextPrefabs);
        go.transform.SetParent(canvas.transform);
        Vector2 uiPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), pos, null, out uiPos);
        go.GetComponent<Text>().gameObject.transform.position = pos;
        //go.transform.position = uiPos;
        Debug.Log("uiPos:"+pos);
    }
}
