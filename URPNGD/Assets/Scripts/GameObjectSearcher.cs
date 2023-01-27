using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class GameObjectSearcher : MonoBehaviour
{
    public string searchTag;
    public List<GameObject> actors = new List<GameObject>();
     
 
    void Start()
    {
        if (searchTag != null)
        {
            FindObjectWithTag(searchTag);
        }
    }
 
    public void FindObjectWithTag(string _tag)
    {
        actors.Clear();
        Transform parent = transform;
        actors = GetChildObject(parent, _tag);
    }
 
    // get all children with a given tag
    public List<GameObject> GetChildObject(Transform parent, string _tag)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.CompareTag(_tag))
            {
                actors.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                GetChildObject(child, _tag);
            }
        }

        return actors;
    }
}