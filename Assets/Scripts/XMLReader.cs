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

    }

    string ParseTag(string xmlText,XMLHashtable res) 
    {
    
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



    public bool ContainsNode(string key) 
    {
        return nodeKeys.IndexOf(key) != -1;
    }
}
