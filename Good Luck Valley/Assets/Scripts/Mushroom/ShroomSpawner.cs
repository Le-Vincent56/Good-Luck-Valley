using GoodLuckValley.Mushroom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class ShroomSpawner : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private GameObject regularShroom;
    [SerializeField] private GameObject wallShroom;
    #endregion

    #region FIELDS
    [SerializeField] private ShroomType shroomType;
    [SerializeField] private Vector2 wallShroomDiff;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnShroom(Component sender, object data)
    {
        // Return if not the correct type
        if (data is not BodyData) return;

        // Cast data
        BodyData bodyData = (BodyData)data;

        GameObject shroom;

        // Get shroom type and instantiate shrooms
        switch (shroomType)
        {
            case ShroomType.Regular:
                shroom = Instantiate(regularShroom, bodyData.Position, bodyData.Rotation);
                break;

            case ShroomType.Wall:
                Vector3 diff = wallShroomDiff;
                if (bodyData.ContactPointX < bodyData.Position.x)
                {
                    diff *= -1;
                }
                shroom = Instantiate(wallShroom, bodyData.Position + diff, bodyData.Rotation);
                break;

            default:
                shroom = Instantiate(regularShroom, bodyData.Position, bodyData.Rotation);
                break;
        }

        // Play the shroom sound
        //AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ShroomPlant, transform.position);

        //shroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        //shroom.GetComponent<ShroomBody>().HasRotated = true;
        //shroom.GetComponent<Shroom>().ShroomIcon = gameObject.GetComponent<Shroom>().ShroomIcon;
        //mushMan.ChangeShroomIndexes[mushMan.MushroomList.IndexOf(gameObject)] = shroom;

        // Set the MushroomInfo angle to the calculated angle
        //shroom.GetComponent<Shroom>().RotateAngle = angle;
        //if (angle >= 0)
        //{
        //    shroom.GetComponent<Shroom>().flipRotation = true;
        //}
        //hasRotated = true;
    }
}
