using UnityEngine;

namespace GoodLuckValley.Cameras.Objects
{
    public class YFollowObject : MonoBehaviour
    {
        [SerializeField] private bool updatePosition = false;

        public void OnPlayerMove(Component sender, object data)
        {
            // Verify that the correct data was sent or that the object should be
            // updating
            if (data is not Vector2 || !updatePosition) return;

            // Cast the data
            Vector2 givenPos = (Vector2)data;

            Vector2 newPos = new Vector2(transform.position.x, givenPos.y);

            // Set the position
            transform.position = newPos;
        }

        public void OnSetUpdatePosition(Component sender, object data)
        {
            // Verify that the correct data was sent
            if (data is not bool) return;

            // Cast and set the data
            updatePosition = (bool)data;
        }
    }
}