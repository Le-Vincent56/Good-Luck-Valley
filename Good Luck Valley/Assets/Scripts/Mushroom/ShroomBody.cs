using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.Events;
using UnityEngine.UIElements;

namespace GoodLuckValley.Mushroom
{
    public struct BodyData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float ContactPointX;

        public BodyData(Vector3 position, Quaternion rotation, float contactPointX)
        {
            Position = position;
            Rotation = rotation;
            ContactPointX = contactPointX;
        }
    }

    public class ShroomBody : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private GameEvent spawnMushroom;
        #endregion

        #region FIELDS
        [SerializeField] private bool hasRotated;
        [SerializeField] private bool flipRotation;
        [SerializeField] private float rotateAngle;
        [SerializeField] private float rotation;
        #endregion

        #region PROPERTIES
        public bool HasRotated { get { return hasRotated; }  set { hasRotated = value; } }
        #endregion

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void RotateAndFreeze()
        {
            // Saves the colliders of the platforms the shroom is coming into contact with intos an array
            ContactPoint2D[] contactsTemp = new ContactPoint2D[10];
            ContactPoint2D[] contacts = new ContactPoint2D[GetComponent<CircleCollider2D>().GetContacts(contactsTemp)];
            GetComponent<CircleCollider2D>().GetContacts(contacts);

            // The direction vector that the mushroom needs to point towards,
            //      contacts[0].point is the point the shroom is touching the platform at
            //      mushroom.transform.position is the mushroom's position,
            //          casted to a vector 2 so it can be subtracted from the contact point
            ContactPoint2D contactPoint = contacts[0];
            for (int i = 0; i < contacts.Length; i++)
            {
                if (contacts[i].rigidbody.CompareTag("Collidable"))
                {
                    contactPoint = contacts[i];
                }
            }
            Vector2 direction = contactPoint.normal;

            // The angle that the shroom is going to rotate at
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // The quaternion that will rotate the shroom
            Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            transform.rotation = rotation;

            // Freezes shroom movement and rotation, and sets hasRotated to true
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            hasRotated = true;

            // Build body data to send
            BodyData newShroomData = new BodyData(transform.position, rotation, contactPoint.point.x);

            spawnMushroom.Raise(this, newShroomData);
        }
    }
}
