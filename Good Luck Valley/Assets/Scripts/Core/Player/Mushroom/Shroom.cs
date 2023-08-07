using UnityEngine;
using UnityEngine.UI;
using HiveMind.Audio;
using HiveMind.Events;

namespace HiveMind.Mushroom
{
    public abstract class Shroom : MonoBehaviour
    {
        #region REFERENCES
        protected MushroomManager mushMan;
        [SerializeField] protected GameObject shroomIcon;
        [SerializeField] protected GameObject regShroom;
        [SerializeField] protected GameObject wallShroom;
        #endregion

        #region FIELDS
        protected ShroomType shroomType;
        [SerializeField] protected MovementScriptableObj movementEvent;
        [SerializeField] protected DisableScriptableObj disableEvent;
        protected bool hasRotated;
        [SerializeField] protected float rotateAngle;
        [SerializeField] protected bool bouncing = false;
        protected float bouncingTimer = 0.1f;
        [SerializeField] protected float durationTimer;
        protected bool onScreen;
        [SerializeField] protected bool isShroom;
        protected Color defaultColor;
        protected ParticleSystem shroomParticles;
        protected bool playShroomParticle = true;
        [SerializeField] protected float particleTime;
        protected bool updateCounter;
        protected float spawnedLifeTime;
        [SerializeField] protected float bounceForce;
        [SerializeField] protected bool onCooldown = false;
        protected float cooldown = 0.1f;
        protected bool flipRotation;
        protected float rotation;
        #endregion

        #region PROPERTIES
        public bool HasRotated { get { return hasRotated; } set { hasRotated = value; } }
        public float RotateAngle { get { return rotateAngle; } set { rotateAngle = value; } }
        public bool Bouncing { get { return bouncing; } set { bouncing = value; } }
        public float BouncingTimer { get { return bouncingTimer; } set { bouncingTimer = value; } }
        public bool OnScreen { get { return onScreen; } set { onScreen = value; } }
        public bool IsShroom { get { return isShroom; } set { isShroom = value; } }
        public float DurationTimer { get { return durationTimer; } set { durationTimer = value; } }
        public Color ShroomColor { get { return defaultColor; } set { defaultColor = value; } }
        public ParticleSystem ShroomParticles { get { return shroomParticles; } set { shroomParticles = value; } }
        public float ParticleTime { get { return particleTime; } set { particleTime = value; } }
        public GameObject ShroomIcon { get { return shroomIcon; } set { shroomIcon = value; } }
        public float SpawnedLifeTime { get { return spawnedLifeTime; } set { spawnedLifeTime = value; } }
        public GameObject RegularMushroom { get { return regShroom; } set { regShroom = value; } }
        public GameObject WallMushroom { get { return wallShroom; } set { wallShroom = value; } }
        public bool FlipRotation { get { return flipRotation; } set { flipRotation = value; } }
        public float Rotation { get { return rotation; } set { rotation = value; } }
        #endregion

        public void Start()
        {
            cooldown = 0.1f;
        }

        /// <summary>
        /// Updates shroom counter's filling and timer
        /// </summary>
        public abstract void UpdateShroomCounter();

        protected void SetCooldown()
        {
            // Set a cooldown for when the player can bounce on the shroom again
            if (onCooldown)
            {
                cooldown -= Time.deltaTime;
            }

            if (cooldown <= 0)
            {
                onCooldown = false;
                cooldown = 0.1f;
            }
        }

        /// <summary>
        /// Resets the mushroom counter to filled and plays the particle effect
        /// </summary>
        /// <param name="shroomIcon"></param>
        public void ResetCounter()
        {
            ShroomIcon.GetComponent<Image>().fillAmount = 1;
            ShroomIcon.GetComponent<ParticleSystem>().Play();
        }

        /// <summary>
        /// Starts the mushroom counter's filling
        /// </summary>
        /// <param name="shroomIcon"></param>
        public void StartCounter()
        {
            ShroomIcon.GetComponent<Image>().fillAmount = 0;
        }

        /// <summary>
        /// Bounces the player in the way the shroom wants
        /// </summary>
        public abstract void Bounce();

        /// <summary>
        /// When the mushroom hits an object
        /// </summary>
        public abstract void OnCollisionEnter2D(Collision2D collision);

        /// <summary>
        /// Rotates and Freezes the mushroom 
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
            GetComponent<Shroom>().HasRotated = true;

            GameObject shroom;

            // Get shroom type and instantiate shrooms
            switch (shroomType)
            {
                case ShroomType.Regular:
                    shroom = Instantiate(regShroom, transform.position, rotation);
                    break;

                case ShroomType.Wall:
                    Vector3 diff = new Vector3(0.1f, 0, 0);
                    if (contactPoint.point.x < transform.position.x)
                    {
                        diff *= -1;
                    }
                    shroom = Instantiate(wallShroom, transform.position + diff, rotation);
                    break;

                default:
                    shroom = Instantiate(regShroom, transform.position, rotation);
                    break;
            }

            // Play the shroom sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ShroomPlant, transform.position);

            shroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            shroom.GetComponent<Shroom>().HasRotated = true;
            shroom.GetComponent<Shroom>().ShroomIcon = gameObject.GetComponent<Shroom>().ShroomIcon;
            mushMan.ChangeShroomIndexes[mushMan.MushroomList.IndexOf(gameObject)] = shroom;

            // Set the MushroomInfo angle to the calculated angle
            shroom.GetComponent<Shroom>().RotateAngle = angle;
            //if (angle >= 0)
            //{
            //    shroom.GetComponent<Shroom>().flipRotation = true;
            //}
            hasRotated = true;
        }

        /// <summary>
        /// Rotates and Freezes the mushroom using a override rotation, use this if you want to manually set the rotation
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
            //Vector2 direction = contactPoint.normal;
            Vector3 diff = new Vector3(0.1f, 0, 0);
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

            // The angle that the shroom is going to rotate at
            //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // The quaternion that will rotate the shroom
            Quaternion rotation = Quaternion.AngleAxis(overrideRotation, Vector3.forward);
            transform.rotation = rotation;

            // Freezes shroom movement and rotation, and sets hasRotated to true
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            GetComponent<Shroom>().HasRotated = true;

            GameObject shroom;

            // Get shroom type and instantiate shrooms
            switch (shroomType)
            {
                case ShroomType.Regular:
                    shroom = Instantiate(regShroom, transform.position, rotation);
                    break;

                case ShroomType.Wall:
                    shroom = Instantiate(wallShroom, transform.position + diff, rotation);

                    // Adjusts angle for the wall shroom specifically
                    if (overrideRotation == -1)
                    {
                        Debug.Log("HUHH");
                    }
                    overrideRotation = (overrideRotation * -1) - 90;
                    break;

                default:
                    shroom = Instantiate(regShroom, transform.position, rotation);
                    break;
            }

            // Play the shroom sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ShroomPlant, transform.position);

            shroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            shroom.GetComponent<Shroom>().HasRotated = true;
            shroom.GetComponent<Shroom>().ShroomIcon = shroomIcon;
            mushMan.ChangeShroomIndexes[mushMan.MushroomList.IndexOf(gameObject)] = shroom;

            // Set the MushroomInfo angle to the calculated angle
            shroom.GetComponent<Shroom>().RotateAngle = overrideRotation;
            hasRotated = true;
        }
    }
}
