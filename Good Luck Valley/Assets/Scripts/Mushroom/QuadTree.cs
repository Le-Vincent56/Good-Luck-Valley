using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree : MonoBehaviour
{
    //[SerializeField] private const int MaxObjects = 10;
    //[SerializeField] private const int MaxLevels = 5;

    //private int level;
    //private List<BoxCollider2D> platforms;
    //private Rect bounds;
    //private QuadTree[] nodes;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //public QuadTree(int pLevel, Rect pBounds)
    //{
    //    level = pLevel;
    //    platforms = new List<BoxCollider2D>();
    //    bounds = pBounds;
    //    nodes = new QuadTree[4];
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //// CLears quadtree
    //public void Clear()
    //{
    //    platforms.Clear();

    //    for (int i = 0; i < nodes.Length; i++)
    //    {
    //        if (nodes[i] != null)
    //        {
    //            nodes[i].Clear();
    //            nodes[i] = null;
    //        }
    //    }
    //}

    //// Splits the node into 4 subnodes
    //private void Split()
    //{
    //    int subWidth = (int)(bounds.width / 2);
    //    int subHeight = (int)(bounds.height / 2);
    //    int x = (int)bounds.x;
    //    int y = (int)bounds.y;

    //    nodes[1] = new QuadTree(level + 1, new Rect(x, y, subWidth, subHeight));
    //    nodes[0] = new QuadTree(level + 1, new Rect(x + subWidth, y, subWidth, subHeight));
    //    nodes[2] = new QuadTree(level + 1, new Rect(x, y + subHeight, subWidth, subHeight));
    //    nodes[3] = new QuadTree(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight));
    //}

    //// DEtermines which node the object belongs to. -1 means object cannot completely fit within a child nmode and is part of the parent node
    //private int GetIndex(BoxCollider2D pRect)
    //{
    //    int index = -1;
    //    double verticalMidpoint = bounds.x + (bounds.width / 2);
    //    double horizontalMidpoint = bounds.x + (bounds.height / 2);

    //    // Object can completely fit within the top quadrants
    //    bool topQuadrant = (pRect.transform.position.x < horizontalMidpoint && pRect.transform.position.y + pRect.transform.localScale.y < horizontalMidpoint);
    //    // Object can completely fit within the bottom quadrants
    //    bool bottomQuadrant = (pRect.transform.position.y > horizontalMidpoint);

    //    // Object can completely fit within the left quadrants
    //    if (pRect.transform.position.x < verticalMidpoint && pRect.transform.position.x + pRect.transform.localScale.x< verticalMidpoint)
    //    {
    //        if (topQuadrant)
    //        {
    //            index = 1;
    //        }
    //        else if (bottomQuadrant)
    //        {
    //            index = 2;
    //        }
    //    }
    //    // Object can completely fit within the right quadrants
    //    else if (pRect.transform.position.x > verticalMidpoint)
    //    {
    //        if (topQuadrant)
    //        {
    //            index = 0;
    //        }
    //        else if (bottomQuadrant)
    //        {
    //            index = 3;
    //        }
    //    }

    //    return index;
    //}

    //// Insert the object into the quadtree. If the node exceeds the capacity, it willsplit and add all objects to their corresponding nodes.
    //public void Insert(BoxCollider2D pRect)
    //{
    //    if (nodes[0] != null)
    //    {
    //        int index = GetIndex(pRect);

    //        if (index != -1)
    //        {
    //            nodes[index].Insert(pRect);

    //            return;
    //        }
    //    }

    //    platforms.Add(pRect);

    //    if (platforms.Count > MaxObjects && level < MaxLevels)
    //    {
    //        if (nodes[0] == null)
    //        {
    //            Split();
    //        }

    //        int i = 0;
    //        while (i < platforms.Count)
    //        {
    //            int index = GetIndex(platforms[i]);
    //            if (index != -1)
    //            {
    //                nodes[index].Insert(platforms[i]);
    //                platforms.RemoveAt(i);
    //            }
    //            else
    //            {
    //                i++;
    //            }
    //        }
    //    }
    //}

    //// Retyrn all objects that could collide with the given object
    //public List<BoxCollider2D> Retrieve(List<BoxCollider2D> returnObjects, BoxCollider2D pRect)
    //{
    //    int index = GetIndex(pRect);
    //    if (index != -1 && nodes[0] != null)
    //    {
    //        nodes[index].Retrieve(returnObjects, pRect);
    //    }

    //    returnObjects.AddRange(platforms);

    //    return returnObjects;
    //}
}
