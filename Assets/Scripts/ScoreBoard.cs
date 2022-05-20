using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public static ScoreBoard S;
    public GameObject prefabFloatingScore;
    public Canvas canvas;

    public bool __________;


    private int _score;
    public string _scoreString;



    public int score 
    {
        get { return _score; }
        set 
        {
            _score = value;
            _scoreString = Utils.AddCommasToNumber(_score);
        }
    }

    public string scoreString 
    {
        get { return _scoreString; }
        set 
        {
            _scoreString = value;
            GetComponent<Text>().text = _scoreString;
        }
    }

    void Awake()
    {
        S = this;
        canvas = FindObjectOfType<Canvas>();
    }

    public void FSCallback(FloatingScore fs) 
    {
        Debug.Log("ScoreBoard CallBack");
        score += fs.score;
    }

    public FloatingScore CreateFloatingScore(int amt,List<Vector3>pts,string name) 
    {
        GameObject go = Instantiate(prefabFloatingScore);
        go.name = name;
        go.transform.SetParent(canvas.transform);
        FloatingScore fs = go.GetComponent<FloatingScore>();
        fs.score = amt;
        fs.reportFinshTo = this.gameObject;
        fs.Init(pts);
        return fs;
    }
}
