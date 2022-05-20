using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FSState 
{
    idle,
    pre,
    active,
    post
}

public class FloatingScore : MonoBehaviour
{
    public FSState fsState = FSState.idle;

    [SerializeField]
    private int _score;
    public string s_score;

    public int score 
    {
        get { return _score; }
        set 
        {
            _score = value;
            s_score = Utils.AddCommasToNumber(_score);
            GetComponent<Text>().text = s_score;
            
        }
    }

    public List<Vector3> bezierPts;
    public List<float> fontSizes;
    public float timeStart = -1f;
    public float timeDuration = 1f;
    public string easingCuve = Easing.InOut;

    //移动完成时，游戏对象将接受SendMessage();
    public GameObject reportFinshTo = null;
    public GameObject endReportFinish = null;

    //设置floatingScore和移动

    public void Init(List<Vector3> ePts, float eTimeS = 0, float eTimeD = 1) 
    {
        bezierPts = new List<Vector3>(ePts);

        if (ePts.Count == 1)//如果只有一个坐标 
        {
            //只运行至此
            transform.position = ePts[0];
            return;
        }

        //如果eTimeS为默认值,则从当前时间开始
        if (eTimeS == 0) 
        {
            eTimeS = Time.time;
        }
        timeStart = eTimeS;
        timeDuration = eTimeD;

        fsState = FSState.pre; 
    }

    //被SendMessage调用
    public void FSCallback(FloatingScore fs) 
    {
        Debug.Log("分数增加,游戏物体名称："+this.gameObject.name);
        score += fs.score;
    }

    public void FSEndCallback(FloatingScore fs) 
    {
        score = fs.score;
    }

    void Update()
    {
        if (fsState == FSState.idle) 
        {
            return;
        }

        float u = (Time.time - timeStart) / timeDuration;
        //Debug.Log(u);
        float uC = Easing.Ease(u, easingCuve);

        if (u < 0)
        {
            fsState = FSState.idle;
            //移动到初始坐标
            transform.position = bezierPts[0];
        }
        else 
        {
            if (u >= 1)
            {
                uC = 1;
                fsState = FSState.post;
                if (reportFinshTo != null)
                {
                    if (endReportFinish != null)
                    {
                        endReportFinish.SendMessage("FSEndCallback", this);
                        Destroy(gameObject);
                    }
                    else 
                    {
                        reportFinshTo.SendMessage("FSCallback", this);
                        Destroy(gameObject);
                    }                    
                }
                else
                {
                    fsState = FSState.idle;
                }
            }
            else 
            {
                fsState = FSState.active;
            }
        }
        //使用Bezier曲线将当前对象移动到正确坐标
        Vector3 pos = Utils.Bezier(uC, bezierPts);
        transform.position = pos;
        if (fontSizes!=null&&fontSizes.Count>0) 
        {
            //如果fontSizes有值，那么将调整text的FontSizes
            int size = Mathf.RoundToInt(Utils.Bezier(uC, fontSizes));
            GetComponent<Text>().fontSize = size;
            GetComponent<Text>().color = Color.black;
        }
    }

}
