using GoodLuckValley.Mushroom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class ShroomSpawner : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private GameObject regularShroom;
        [SerializeField] private GameObject wallShroom;
        #endregion

        #region FIELDS
        [SerializeField] private ShroomType shroomType;
        #endregion

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
                    shroom = Instantiate(wallShroom, bodyData.Position + bodyData.Offset, bodyData.Rotation);

                    if (bodyData.ManualRotation)
                    {
                        float overrideRotation = (bodyData.RotateAngle * -1) - 90;
                    }
                    break;

                default:
                    shroom = Instantiate(regularShroom, bodyData.Position, bodyData.Rotation);
                    break;
            }

            // Play the shroom sound
            //AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ShroomPlant, transform.position);

            // Set the new mushroom's body info
            shroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            shroom.GetComponent<ShroomBody>().HasRotated = true;
            shroom.GetComponent<ShroomBody>().RotateAngle = bodyData.RotateAngle;
            if (bodyData.RotateAngle >= 0)
            {
                shroom.GetComponent<ShroomBody>().FlipRotation = true;
            }
        }
    }
}
