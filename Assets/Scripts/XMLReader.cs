using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMLReader 
{
    static bool SHOW_COMMENTS = false;

    public string XMLText;

    public XMLHashtable res;


    public void Parse(string xmlText) 
    {
        XMLText = xmlText;
        res = new XMLHashtable();
        Parse(XMLText, res);
    }

    public void Parse(string xmlText,XMLHashtable res) 
    {
        xmlText.Trim();
        while (xmlText.Length>0) 
        {
            xmlText = ParseTag(xmlText, res);
            xmlText = xmlText.Trim();
        }

    }

    string ParseTag(string xmlText, XMLHashtable res)
    {
        int index = xmlText.IndexOf("<");
        if (index == -1)
        {

        }

        //注入标题
        if (xmlText[index + 1] == '?')
        {
            int index2 = xmlText.IndexOf("?>");
            string header = xmlText.Substring(index, index2 - index + 2);
            res.header = header;
            return xmlText.Substring(index2 + 2);
        }

        //读取评论
        if (xmlText[index + 1] == '!')
        {
            int ndx2 = xmlText.IndexOf("-->");
            string comment = xmlText.Substring(index, ndx2 - index + 3);
            if (SHOW_COMMENTS) Debug.Log("XMl Comment: " + comment);
            //eH["@XML_Header"] = header;
            return (xmlText.Substring(ndx2 + 3));
        }

        int end1 = xmlText.IndexOf(' ', index);
        int end2 = xmlText.IndexOf('/', index);
        int end3 = xmlText.IndexOf('>', index);

        if (end1 == -1) end1 = int.MaxValue;
        if (end2 == -1) end2 = int.MaxValue;
        if (end3 == -1) end3 = int.MaxValue;

        int end = Mathf.Min(end1, end2, end3);
        string tag = xmlText.Substring(index + 1, end - index - 1);
        //Debug.Log(tag);

        if (!res.ContainsNode(tag))
        {
            res[tag] = new XMLHashList();
        }
        //取得这个HashList
        XMLHashList tHashList = res[tag];
        XMLHashtable tHashtable = new XMLHashtable();

        tHashList.Add(tHashtable);

        string attr = "";
        if (end1 < end3)
        {
            //说明现在处理的是在一个特性里的
            try
            {
                attr = xmlText.Substring(end1, end3 - end1);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                Debug.Log("break");
            }
        }

        string attKey;
        string val;
        int eqIdx;
        int spIdx;
        //说明还有未处理的attr
        while (attr.Length > 0)
        {
            attr = attr.Trim();
            eqIdx = attr.IndexOf("=");
            if (eqIdx == -1) 
            {
                break;
            }
            attKey = attr.Substring(0, eqIdx);
            spIdx = attr.IndexOf(" ", eqIdx);
            if (spIdx == -1)
            {
                val = attr.Substring(eqIdx + 1);
                if (val[val.Length - 1] == '/')
                {
                    val = val.Substring(0, val.Length - 1);
                }
                attr = "";
            }
            else 
            {
                val = attr.Substring(eqIdx + 1, spIdx - eqIdx - 2);
                attr = attr.Substring(spIdx);
            }

            val = val.Trim('\"');
            tHashtable.SetAttr(attKey, val);
        }
        string sub = "";
        string leftoverString = "";

        bool singleLine = (end2 == end3 - 1);
        if (!singleLine)
        {
            int closeIndex = xmlText.IndexOf("</" + tag + ">");
            if (closeIndex == -1)
            {
                Debug.LogError("XMLReader ERROR:XML not well formed.Close tag </" + tag + ">missing");
                return "";
            }
            sub = xmlText.Substring(end3 + 1, closeIndex - end3 - 1);
            leftoverString = xmlText.Substring(xmlText.IndexOf(">", closeIndex) + 1);

        }
        else 
        {
            leftoverString = xmlText.Substring(end3 + 1);
        }
        sub = sub.Trim();
        if (sub.Length>0) 
        {
            Parse(sub, tHashtable);
        }

        return leftoverString;
    }

}

public class XMLHashList 
{
    public ArrayList list = new ArrayList();

    public XMLHashtable this[int index] 
    {
        get 
        {
            return list[index] as XMLHashtable;
        }
        set 
        {
            list[index] = value;
        }
    }

    public void Add(XMLHashtable xh) 
    {
        list.Add(xh);
    }

    public int Count 
    {
        get 
        {
            return list.Count;
        }
    }
}

public class XMLHashtable
{
    public List<string> nodeKeys = new List<string>();
    public List<XMLHashList> nodeList = new List<XMLHashList>();
    public List<string> attrKeys = new List<string>();
    public List<string> attr = new List<string>();


    public int nodeIndex(string key)
    {
        return nodeKeys.IndexOf(key);
    }


    public XMLHashList GetNode(string key)
    {
        int index = nodeIndex(key);
        if (index != -1)
        {
            return nodeList[index];
        }
        else
        {
            return null;
        }
    }

    public void SetNode(string key, XMLHashList target)
    {
        int index = nodeIndex(key);
        if (index != -1)
        {
            nodeList[index] = target;
        }
        else
        {
            nodeKeys.Add(key);
            nodeList.Add(target);
        }
    }


    public int attrIndex(string key)
    {
        return attrKeys.IndexOf(key);
    }

    public string GetAttr(string key) 
    {
        int index = attrIndex(key);
        if (index !=-1) 
        {
            return attr[index];
        }
        return "";
    }

    public void SetAttr(string key,string value) 
    {
        int index = attrIndex(key);
        if (index != -1)
        {
            attr[index] = value;
        }
        else 
        {
            attrKeys.Add(key);
            attr.Add(value);
        }
    }

    public XMLHashList this[string index] 
    {
        get 
        {
            return GetNode(index);
        }
        set 
        {
            SetNode(index, value);
        }
    }

    public string header 
    {
        get 
        {
            int index = attrIndex("XML_header");
            if (index == -1) 
            {
                return "";
            }
            return attr[index];
        }

        set 
        {
            int index = attrIndex("XML_header");
            if (index == -1)
            {
                attrKeys.Add("XML_header");
                attr.Add(value);
            }
            else 
            {
                attr[index] = value;
            }
        }
    }



    public bool ContainsNode(string key) 
    {
        return nodeKeys.IndexOf(key) != -1;
    }
}
