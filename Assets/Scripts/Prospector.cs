using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public enum ScoreEvent 
{
    draw,
    mine,
    mineGold,
    gameLose,
    gameWin
}

public class Prospector : MonoBehaviour
{
    public static Prospector S;
    public static int HIGH_SCORE_PRE_ROUND;
    public static int HIGH_SCORE_THIS_ROUND;
    public static int HIGH_SCORE;
    public static int SCORE_CONTINUE;



    public Transform curScorePos;
    public Transform endScorePos;
    
    public Vector3 fsPosMid = new Vector3(0.5f,0.90f,0);
    public Vector3 fsPosRun = new Vector3(0.5f, 0.75f, 0);
    public Vector3 fsPosMid2 = new Vector3(0.5f, 0.5f, 0);
    public Vector3 fsPosEnd = new Vector3(1.0f, 0.65f, 0);
    public FloatingScore fsRun;
    public FloatingScore fsEndRun;

    public Deck deck;
    public TextAsset xmlText;

    public Layout layout;
    public TextAsset layoutText;

    //用于设置布局的变量
    public Vector3 layoutCenter;
    public float xoffset = 3f;
    public float yoffset = -2.5f;
    public Transform layoutAnchor;
    public bool isLayoutReady;
    public int isFirstLoad = 0;

    public CardProspector target;
    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;
    public List<CardProspector> drawPile;



    //发牌计时器
    public float timer = 2f;
    public float timeBefore;
    public float delayReload = 4f;

    // Start is called before the first frame update

    void Awake()
    {
        //if (PlayerPrefs.HasKey("isFirstLoad")) 
        //{
        //    isFirstLoad = PlayerPrefs.GetInt("isFirstLoad");
        //}
        //Debug.Log(isFirstLoad);   
        S = this;
        if (PlayerPrefs.HasKey("HIGH_SCORE_PRE_ROUND"))
        {
            HIGH_SCORE_PRE_ROUND = PlayerPrefs.GetInt("HIGH_SCORE_PRE_ROUND");
            Debug.Log("上一轮分数" + HIGH_SCORE_PRE_ROUND);
            //if (HIGH_SCORE_PRE_ROUND!=0&&isFirstLoad==1) 
            //{                                
            //    //fsRun = GameObject.Find("curScore").GetComponent<FloatingScore>();
            //    Debug.Log("We Find it");
            //}                                  
        }
        if (GameObject.Find("curScore")!=null) 
        {
            fsRun = GameObject.Find("curScore").GetComponent<FloatingScore>();
            Debug.Log("Find it");
        }
        SCORE_CONTINUE = 0;
    }

    void Start()
    {
        ScoreBoard.S.score = HIGH_SCORE;
        //初始化记录纸牌
        deck = GetComponent<Deck>();
        deck.InitDeck(xmlText.text);
        deck.ShuffCard(ref deck.cards);
        drawPile = ConvertCardToCardProspector(deck.cards);
        //初始化记录布局
        layout = GetComponent<Layout>();
        layout.ReadLayOut(layoutText.text);

        LayGame();
    }

    // Update is called once per frame
    void Update()
    {
//#if UNITY_EDITOR
//        if (!UnityEditor.EditorApplication.isPlaying) 
//        {
//            Debug.Log("Kill Application");
//            PlayerPrefs.DeleteKey("HIGH_SCORE_PRE_ROUND");
//        }
//#else
//        if (!Application.isPlaying) 
//        {
//            Debug.Log("Kill Application ");
//            PlayerPrefs.DeleteKey("HIGH_SCORE_PRE_ROUND");
//        }
//#endif
    }

    public void CardClicked(CardProspector cd)
    {
        switch (cd.state)
        {
            case CardState.target:

                break;
            case CardState.drawpile:
                MoveToDiscard(target);
                MoveToTarget(Draw());
                UpdateDrawPile();
                ScoreManger(ScoreEvent.draw);
                checkWin();
                break;
            case CardState.tableau:

                bool validMatch = true;
                if (cd.FaceUp == false) validMatch = false;
                if (!AdjacentRank(cd, target)) validMatch = false;
                if (!validMatch)
                {
                    return;
                }
                //Debug.Log("TouchTableU");
                tableau.Remove(cd);
                MoveToTarget(cd);
                ScoreManger(ScoreEvent.mine);
                confirmCover();
                checkWin();
                break;
        }
    }

    public void checkWin()
    {
        bool Over = true;
        //bool gameOver = false;
        foreach (CardProspector cd in tableau)
        {
            if (cd.FaceUp == true)
            {
                if (cd.rank - 1 == target.rank || cd.rank + 1 == target.rank)
                {
                    Over = false;
                    break;
                    //return false;
                }
            }
        }

        if (Over)
        {
            //Debug.Log("You need Draw A New Card");
        }
        else
        {
            //Debug.Log("Not over Yet");
        }

        if (tableau.Count == 0)
        {

            //gameOver = true;
            GameOver(true);

        }
        if (drawPile.Count <= 0&&Over)
        {
            //gameOver = false;
            GameOver(false);
        }
    }

    void ScoreManger(ScoreEvent sevt)
    {
        List<Vector3> fsPts;
        Vector3 curPos;
        
        Vector3 midPos;
        switch (sevt)
        {
            case ScoreEvent.draw:
                SCORE_CONTINUE = 0;
                break;
            case ScoreEvent.mine:
                SCORE_CONTINUE += 1;
                HIGH_SCORE_THIS_ROUND += SCORE_CONTINUE;

                AdjustHighScore(HIGH_SCORE_THIS_ROUND);

                //取得当前的分数终点位置
                curPos = this.curScorePos.transform.position;
                FloatingScore fs;
                //从MousePosition移动到fsPosRun
                Vector3 p0 = Input.mousePosition;
                //p0.x /= Screen.width;
                //p0.y /= Screen.height;
                fsPts = new List<Vector3>();
                fsPts.Add(p0);
                                
                midPos = (p0 + curPos) / 2;
                fsPts.Add(midPos);
                fsPts.Add(curPos);
                fs = ScoreBoard.S.CreateFloatingScore(SCORE_CONTINUE, fsPts,"curScore");
                fs.fontSizes = new List<float>(new float[] { 10, 100, 65 });
                if (fsRun == null)
                {
                    fsRun = fs;
                    fsRun.reportFinshTo = null;
                }
                else 
                {
                    //这里变成了fs是传递过去然后销毁的，给的是fsRun增加分数，fsRun固定不变
                    fs.reportFinshTo = fsRun.gameObject;
                }
                //Debug.Log("连续得分:" + SCORE_CONTINUE);                
                //Debug.Log("当前得分" + (HIGH_SCORE_PRE_ROUND + HIGH_SCORE_THIS_ROUND).ToString());
                //Debug.Log("最高分" + HIGH_SCORE);
                break;
            case ScoreEvent.mineGold:
                break;
            default:
                break;
        }
        switch (sevt)
        {
            case ScoreEvent.gameLose:
                //Debug.Log("HighScoreThisRound:" + HIGH_SCORE_THIS_ROUND);
                if (HIGH_SCORE_THIS_ROUND<HIGH_SCORE) 
                {
                    Debug.Log("HighThisRound:" + HIGH_SCORE_THIS_ROUND);
                    Destroy(fsRun.gameObject);
                    return;
                }
                AdjustHighScore(HIGH_SCORE_PRE_ROUND);
                
                PlayerPrefs.DeleteKey("HIGH_SCORE_PRE_ROUND");
                curPos = this.curScorePos.transform.position;
                Vector3 endPos = endScorePos.transform.position;
                midPos = (curPos + endPos) / 2;
                fsPts = new List<Vector3>();
                FloatingScore fs;

                fsPts.Add(curPos);
                fsPts.Add(midPos);
                fsPts.Add(endPos);
                
                if (GameObject.Find("HighScore") != null)
                {
                    fs = ScoreBoard.S.CreateFloatingScore(HIGH_SCORE_THIS_ROUND, fsPts, "tempScore");
                    fsEndRun = GameObject.Find("HighScore").GetComponent<FloatingScore>();
                    fs.endReportFinish = fsEndRun.gameObject;
                }
                else
                {
                    fs = ScoreBoard.S.CreateFloatingScore(HIGH_SCORE, fsPts, "HighScore");
                    fsEndRun = fs;
                    fsEndRun.reportFinshTo = null;
                }
                //fs = ScoreBoard.S.CreateFloatingScore(HIGH_SCORE, fsPts,"HighScore");
                fs.fontSizes = new List<float>(new float[] { 100, 50, 65 });
                //DontDestroyOnLoad(fsRun);
                fs.Init(fsPts, 0, 1);
                //同时调整fontSize
                

                //if (fsEndRun == null)
                //{
                //    fsEndRun = fs;
                //    fsEndRun.reportFinshTo = null;
                //}
                //else 
                //{
                //    fs.endReportFinish = fsEndRun.gameObject;
                //}
                
                Destroy(fsRun.gameObject);
                HIGH_SCORE_PRE_ROUND = 0;
                HIGH_SCORE_THIS_ROUND = 0;
                Debug.Log("High Score:" + HIGH_SCORE);
                break;
            case ScoreEvent.gameWin:
                AdjustHighScore(HIGH_SCORE_PRE_ROUND);
                PlayerPrefs.SetInt("HIGH_SCORE_PRE_ROUND", HIGH_SCORE_THIS_ROUND + HIGH_SCORE_PRE_ROUND);
                break;
        }
    }

    void AdjustHighScore(int highScorePreRound) 
    {
        if (highScorePreRound > HIGH_SCORE)
        {
            HIGH_SCORE = highScorePreRound;
            //if (fsRun!=null) 
            //{
            //    fsRun.reportFinshTo = ScoreBoard.S.gameObject;
            //}            
        }
        PlayerPrefs.SetInt("HIGH_SCORE", HIGH_SCORE);
    }

    void KeepHighScore() 
    {
        
    }

    public void GameOver(bool gameOver) 
    {
        if (gameOver)
        {
            ScoreManger(ScoreEvent.gameWin);
            Debug.Log("You Win!!!!!");
        }
        else
        {
            ScoreManger(ScoreEvent.gameLose);
            Debug.Log("You Lose");
        }

        Invoke("ReloadLevel", delayReload);
    }

    void ReloadLevel() 
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void confirmCover() 
    {
        foreach (CardProspector temp in tableau) 
        {
            bool up = true;
            foreach (CardProspector cover in temp.hiddenBy) 
            {
                if (cover.state == CardState.tableau) 
                {
                    up = false;
                }
            }
            temp.FaceUp = up;
        }
    }

    public bool AdjacentRank(CardProspector cd,CardProspector target) 
    {
        //两个中有一个朝上，即返回false,感叹号代入到变量去思考
        if (!cd.FaceUp||!target.FaceUp) 
        {
            return false;
        }

        if (Mathf.Abs(cd.rank - target.rank)==1) 
        {
            return true;
        }
        if (cd.rank == 1 && target.rank == 13) return true;
        if (cd.rank == 13 && target.rank == 1) return true;

        return false;
    }

    void MoveToDiscard(CardProspector cd) 
    {
        cd.state = CardState.discard;
        discardPile.Add(cd);
        cd.transform.parent = layoutAnchor;
        //定位到弃牌堆
        cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID + 0.5f);
        cd.FaceUp = true;
        cd.SetAllSpriteRendererName(layout.discardPile.layerName);
        cd.SetAllSpriteRendererOrder(-100 + discardPile.Count);
    }

    void MoveToTarget(CardProspector cd) 
    {
        if (target!=null) 
        {
            MoveToDiscard(target);
        }

        target = cd;
        cd.state = CardState.target;
        cd.transform.parent = layoutAnchor;
        //移动到目标位置
        cd.transform.localPosition = new Vector3(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID + 0.5f);
        cd.FaceUp = true;
        //设置深度排序
        cd.SetAllSpriteRendererName(layout.discardPile.layerName);
        cd.SetAllSpriteRendererOrder(0);
    }

    void UpdateDrawPile() 
    {
        CardProspector cd;
        //遍历所有储备牌，即Deck强转过来的drawPile
        for (int i = 0; i < drawPile.Count; i++)
        {
            cd = drawPile[i];
            cd.transform.parent = layoutAnchor;

            Vector2 dpStagger = layout.drawPile.xStagger;

            cd.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
                layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
                -layout.drawPile.layerID + 0.1f * i
                );


            cd.FaceUp = false;
            cd.state = CardState.drawpile;

            //设置深度层
            cd.SetAllSpriteRendererName(layout.drawPile.layerName);
            cd.SetAllSpriteRendererOrder(-10 * i);
        }
    }





    //转换纸牌类型
    List<CardProspector> ConvertCardToCardProspector(List<Card>cards) 
    {
        List<CardProspector> cardProspectors = new List<CardProspector>();
        CardProspector CP;
        foreach  (Card card in cards)
        {
            CP = card as CardProspector;
            cardProspectors.Add(CP);
        }
        return cardProspectors;
    }

    //我的回合！抽卡！
    CardProspector Draw() 
    {
        //随机抽走了布局的卡片
        CardProspector card = drawPile[0];        
        drawPile.RemoveAt(0);
        //Debug.Log(drawPile.Count);
        return card;
    }

    CardProspector FindCardBySlotID(int slotID) 
    {
        foreach (CardProspector cp in tableau) 
        {
            if (cp.slotID == slotID) 
            {
                return cp;
            }
        }
        return null;
    }

    //创建游戏布局
    void LayGame() 
    {
        if (layoutAnchor==null) 
        {
            GameObject anchorGameobject = new GameObject("_anchorGameobject");
            
            layoutAnchor = anchorGameobject.transform;
            //定位
            layoutAnchor.transform.position = layoutCenter;
        }

        CardProspector cp;
        float timer = 0;
        foreach (SlotDef sDef in layout.SlotDefs)
        {
            if (sDef.type == "drawpile"||sDef.type == "discardpile") 
            {
                continue;
            }
            cp = Draw();
            cp.FaceUp = sDef.faceUp;
            cp.transform.parent = layoutAnchor;
            IEnumerator putCard = PutCard(cp, sDef,timer);
            //使用协程发牌
            StartCoroutine(putCard);
            //cp.transform.localPosition = new Vector3(layout.multiplier.x * sDef.x, layout.multiplier.y * sDef.y, -sDef.layerID);
            cp.LayerID = sDef.layerID;
            cp.slotID = sDef.slotID;

            //输出层级，和层级ID
            //Debug.Log(sDef.layerName);
            //Debug.Log(sDef.layerID);
            cp.slotDef = sDef;
            cp.SetAllSpriteRendererName(sDef.layerName);
            cp.state = CardState.tableau;

            tableau.Add(cp);
            timer += 0.1f;
        }

        //添加hiddenBy关系
        foreach (CardProspector tcp in tableau)
        {
            foreach (int hiddenID in tcp.slotDef.hiddenBy)
            {
                CardProspector hiddenCp =  FindCardBySlotID(hiddenID);
                tcp.hiddenBy.Add(hiddenCp);
            }
        }

        //移动一张牌到目标区域（弃牌区）
        //对牌堆进行一次排列
        MoveToTarget(Draw());
        UpdateDrawPile();
    }

    IEnumerator PutCard(CardProspector cp,SlotDef slotDef,float timer) 
    {
        yield return new WaitForSeconds(1f+timer);
        
        cp.transform.localPosition = new Vector3(layout.multiplier.x * slotDef.x, layout.multiplier.y * slotDef.y, -slotDef.layerID);
        //Debug.Log(cp.transform.localPosition);
    }
}
