using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomControl : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private GameObject spore;
        #endregion

        #region FIELDS
        private bool canThrow = false;
        private float throwCooldown = 0.2f;
        #endregion
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void ThrowMushroom()
        {
            GameObject mushroom = Instantiate(spore, transform.position, Quaternion.identity);
            //Vector2 throwForce = forceDirection.normalized * throwMultiplier;

            //mushroom.GetComponent<Rigidbody2D>().AddForce(throwForce, ForceMode2D.Impulse);
            //mushroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            //mushroom.GetComponent<Shroom>().Rotation = -1;
        }
    }
}
