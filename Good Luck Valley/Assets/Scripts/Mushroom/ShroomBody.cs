using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.Events;

namespace GoodLuckValley.Mushroom
{
    public struct BodyData
    {
        public Vector3 Position;
        public Vector3 Offset;
        public Quaternion Rotation;
        public float RotateAngle;
        public float ContactPointX;
        public bool ManualRotation;

        public BodyData(Vector3 position, Vector3 offset, Quaternion rotation, 
            float rotateAngle, float contactPointX, bool manualRotation)
        {
            Position = position;
            Rotation = rotation;
            RotateAngle = rotateAngle;
            ContactPointX = contactPointX;
            Offset = offset;
            ManualRotation = manualRotation;
        }
    }

    public class ShroomBody : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private GameEvent onSpawnMushroom;
        #endregion

        #region FIELDS
        [SerializeField] private Vector2 wallShroomDiff;
        [SerializeField] private bool hasRotated;
        [SerializeField] private bool flipRotation;
        [SerializeField] private float rotateAngle;
        [SerializeField] private float rotation;
        #endregion

        #region PROPERTIES
        public bool HasRotated { get { return hasRotated; }  set { hasRotated = value; } }
        public bool FlipRotation { get { return flipRotation; } set {  flipRotation = value; } }
        public float RotateAngle { get { return rotateAngle; } set { rotateAngle = value; } }
        #endregion

        /// <summary>
        /// Rotates and freezes the mushroom automatically
        /// </summary>
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

            // Create offset for wall shrooms
            Vector3 diff = wallShroomDiff;
            if (contactPoint.point.x < transform.position.x)
            {
                diff *= -1;
            }

            // Construct body data
            BodyData newShroomData = new BodyData(transform.position, diff, rotation, 
                angle, contactPoint.point.x, false);

            // Spawn the mushroom
            onSpawnMushroom.Raise(this, newShroomData);
        }

        /// <summary>
        /// Rotates and freezes the mushroom manually given a rotation
        /// </summary>
        public void RotateAndFreeze(float overrideRotation)
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

            Vector3 diff = wallShroomDiff;
            if (contactPoint.point.x < transform.position.x)
            {
                diff *= -1;
            }

            // If we have manually set the override rotation to -1 (in throwMushroom)
            if (overrideRotation == -1)
            {
                // Check if the wall is to the left
                if (diff.x < 0)
                {
                    // Set the rotation to be -90
                    overrideRotation = -90;
                }
                else
                {
                    // Set the rotation to be -90
                    overrideRotation = 90;
                }
            }

            // The quaternion that will rotate the shroom
            Quaternion rotation = Quaternion.AngleAxis(overrideRotation, Vector3.forward);
            transform.rotation = rotation;

            // Freezes shroom movement and rotation, and sets hasRotated to true
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            hasRotated = true;

            // Construct body data
            BodyData newShroomData = new BodyData(transform.position, diff, rotation, 
                overrideRotation, contactPoint.point.x, true);

            // Spawn the mushroom
            onSpawnMushroom.Raise(this, newShroomData);
        }
    }
}
