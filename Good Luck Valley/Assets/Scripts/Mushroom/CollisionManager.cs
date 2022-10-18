using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    //[SerializeField] private GameObject background;
    //private QuadTree quad;
    //[SerializeField] private List<BoxCollider2D> platforms;
    //private MushroomManager mushMan;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    quad = new QuadTree(0, new Rect(background.transform.position.x - background.transform.localScale.x / 2, 
    //                                             background.transform.position.y - background.transform.localScale.y / 2, 
    //                                             background.transform.localScale.x,
    //                                             background.transform.localScale.y));
    //    mushMan = GetComponent<MushroomManager>();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (mushMan.MushroomList.Count > 0)
    //    {
    //        quad.Clear();
    //        for (int i = 0; i < platforms.Count; i++)
    //        {
    //            quad.Insert(platforms[i].GetComponent<BoxCollider2D>());
    //        }

    //        List<BoxCollider2D> returnObjects = new List<BoxCollider2D>();
    //        for (int i = 0; i < platforms.Count; i++)
    //        {
    //            returnObjects.Clear();
    //            quad.Retrieve(returnObjects, platforms[i].GetComponent<BoxCollider2D>());

    //            for (int x = 0; x < returnObjects.Count; x++)
    //            {
    //                foreach (GameObject m in mushMan.MushroomList)
    //                {
    //                    if (m.GetComponent<BoxCollider2D>().IsTouching(returnObjects[x]))
    //                    {
    //                        m.transform.rotation = returnObjects[x].transform.rotation;
    //                        m.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}
