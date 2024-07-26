using UnityEngine;
using GoodLuckValley.Mushroom;
using GoodLuckValley.Events;
using GoodLuckValley.Player.Input;
using UnityEngine.VFX;
using System.Collections;

namespace GoodLuckValley.UI
{
    public class ThrowLine : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GameEvent onRequestPlayerController;
        [SerializeField] private GameEvent onRequestThrowData;
        [SerializeField] private GameEvent onShowIndicator;
        [SerializeField] private GameEvent onHideIndicator;

        [Header("References")]
        [SerializeField] private InputReader input;
        //[SerializeField] private VisualEffect aimLineVFX;

        private float width;
        private int segments;
        private LineRenderer lineRenderer = null;
        private Vector3[] lineRendererStartingPoints = null;
        private ShroomSpawnData currentSpawnData;

        [Header("Layer Masks")]
        [SerializeField] private LayerMask shroomableLayer;
        [SerializeField] private LayerMask unshroomableLayer;

        [Header("Details")]
        [SerializeField] private bool show;
        [SerializeField] private float throwMultiplier;
        [SerializeField] private float throwChangeAmount;
        [SerializeField] private float multMax;
        [SerializeField] private float multMin;

        [Header("Vectors")]
        [SerializeField] private Vector2 cursorPosition;
        [SerializeField] private Vector2 throwDirection;
        [SerializeField] private Vector2 launchForce;
        [SerializeField] private Vector2 playerPos;
        [SerializeField] private Vector2 offset;

        private void OnEnable()
        {
            input.Scroll += ChangeMultiplier;
        }

        private void OnDisable()
        {
            input.Scroll -= ChangeMultiplier;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Width of the line
            width = 0.1f;

            // Number of segments for the trajectory line
            segments = 10000;

            // Gets the LineRenderer component from the lineRenderer game object applied in inspector
            lineRenderer = gameObject.GetComponent<LineRenderer>();

            // Sets the number of segmens in the lineRenderer using segments field
            lineRenderer.positionCount = segments;

            lineRendererStartingPoints = new Vector3[segments + 1];

            // Sets the with in the lineRenderer using width field

            // Tells the lineRenderer to use worldspace for defining segmentsx`
            lineRenderer.useWorldSpace = true;
        }

        private void Update()
        {
            // Return if not being rendered
            if (!show) return;

            // Show the line
            PlotTrajectory();

            //Vector3[] points = new Vector3[lineRenderer.positionCount];
            //lineRenderer.GetPositions(points);
            //StartCoroutine(SpawnUIParticles(points));
        }

        public void ChangeMultiplier(float scrollDeltaY)
        {
            throwMultiplier += (scrollDeltaY * throwChangeAmount);
            throwMultiplier = Mathf.Clamp(throwMultiplier, multMin, multMax);
        }

        /// <summary>
        /// Plots the trajeectory of the mushroom during aiming using the pleyer's position, 
        ///     launch force on the mushroom, the offset for spawning, and whether the 
        ///     player is facing right or left
        /// </summary>
        /// <param name="playerPos"> The position of the player AKA starting position for
        ///                             the lineRenderer</param>
        /// <param name="launchForce"> The force the mushroom is being launched at so the
        ///                              lineRenderer can simulate a throw when plotting</param>
        /// <param name="offset"> The offset used when spawning mushrooms</param>
        /// <param name="facingRight"> Whether the player is facing left or right</param>
        public void PlotTrajectory()
        {
            // Get line data
            // Calls to:
            //   - PlayerController.ReturnThrowData()
            //   - GameCursor.ReturnThrowData()
            onRequestThrowData.Raise(this, this);

            // Set data
            playerPos = (Vector2)transform.position;
            throwDirection = (cursorPosition - playerPos).normalized;
            launchForce = throwDirection * throwMultiplier;

            // Sets the position count to be the segment count
            lineRenderer.positionCount = segments;
            lineRenderer.material.mainTextureScale = new Vector2(1f, 1f);
            lineRenderer.startWidth = width;

            // Gravity acting on the shroom when it is being thrown
            const float g = 9.8f;

            // Determines how trajectoy line will be
            float timeStep = .04f;

            // Total time that has passed since the lineRenderer started rendering
            float tT = 0f;

            // X and Y for each segment
            float x;
            float y;

            // Sets the position for the first segment using player position
            Vector3 start = new Vector3(playerPos.x, playerPos.y, 0);
            lineRenderer.SetPosition(0, start);

            // Runs a loop for rendering each segment in the trajectory
            for (int i = 1; i < segments; i++)
            {
                // Total time passed
                tT += timeStep;

                // x position is determined by player x + velocity X multiplied
                //  by the amount of time passed
                x = (playerPos.x + (launchForce.x) * (tT));

                // y position is determined by player x + velocity x multiplied
                //  by time passed - half of gravity multiplied by twice the time passed
                y = (playerPos.y + (launchForce.y) * (tT) - 0.5f * g * (tT) * (tT));

                // Sets the position for this segment using the x and y generated above
                lineRenderer.SetPosition(i, new Vector3(x, y, 0));
                lineRendererStartingPoints[i] = new Vector3(x, y, 0);
            }

            // Creates collided bool, sets to false,
            //  will be used to determin if we should update the segments or not later on
            bool collided = false;

            // Creates hit info variable for storing information about raycast hit
            RaycastHit2D shroomableHitInfo;
            RaycastHit2D unshroomableHitInfo;

            // Create new array for storing the new points we will draw with
            Vector3[] newPoints = null;

            // Collision data
            ShroomSpawnData spawnData = new ShroomSpawnData(
                Vector2.zero,
                0,
                false
            );

            // Loops for each point in the previous frame's array of points
            for (int i = 1; i < lineRendererStartingPoints.Length -1; i++)
            {
                // Sets hit info to the return value of the linecast method,
                //   using the current point on the line and the next point as the locations to check between
                shroomableHitInfo = Physics2D.Linecast(lineRendererStartingPoints[i], lineRendererStartingPoints[i + 1], shroomableLayer);
                unshroomableHitInfo = Physics2D.Linecast(lineRendererStartingPoints[i], lineRendererStartingPoints[i + 1], unshroomableLayer);

                // Check shroomable hits
                if (shroomableHitInfo)
                {
                    // Handle valid raycasts
                    HandleRaycastValid(i, shroomableHitInfo, out newPoints, out spawnData, out collided);
                    break;
                }

                // Check unshroomeable hits
                if (unshroomableHitInfo)
                {
                    // handle invalid raycasts
                    HandleRaycastInvalid(i, unshroomableHitInfo, out newPoints, out spawnData, out collided);
                    break;
                }
            }

            // If collided is true
            if (collided)
            {
                // Sets segments to be equal to the length of the new array so
                //  the line is drawn properly when the top of the method is called
                segments = newPoints.Length;

                // Set collision data
                currentSpawnData = spawnData;

                // Create the indicator at the collision
                // Calls to:
                //  - MushroomIndicator.ShowIndicator();
                onShowIndicator.Raise(this, spawnData);
            }
            else
            {
                // Otherwise, segments is set back to its original value of 300
                segments = 300;
            }
        }
        
        /// <summary>
        /// Handle collisions for mushroom spawning for a valid raycast
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hitInfo"></param>
        /// <param name="newPoints"></param>
        /// <param name="collisionData"></param>
        /// <param name="collided"></param>
        public void HandleRaycastValid(int count, RaycastHit2D hitInfo, out Vector3[] newPoints, out ShroomSpawnData spawnData, out bool collided)
        {
            // Initializes new points array to be the current iteratin number + 2
            newPoints = new Vector3[count + 2];

            // Loops through each point in the new points array
            for (int k = 0; k < newPoints.Length; k++)
            {
                // Sets the values in the new points array to match the values in the prev points array
                newPoints[k] = lineRendererStartingPoints[k];
            }

            // Sets the last position in the new array to be the location the hit occured
            newPoints[count] = hitInfo.point;

            // Collided is true
            collided = true;

            // Set collision data
            spawnData.Point = hitInfo.point;
            spawnData.Rotation = (int)Vector2.Angle(hitInfo.normal, Vector2.up) * -(int)Mathf.Sign(hitInfo.normal.x);
            spawnData.Valid = true;
        }

        /// <summary>
        /// Handle collisions for mushroom spawning for an invalid raycast
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hitInfo"></param>
        /// <param name="newPoints"></param>
        /// <param name="spawnData"></param>
        /// <param name="collided"></param>
        public void HandleRaycastInvalid(int count, RaycastHit2D hitInfo, out Vector3[] newPoints, out ShroomSpawnData spawnData, out bool collided)
        {
            newPoints = new Vector3[count + 2];

            // Loops through each point in the new points array
            for (int k = 0; k < newPoints.Length; k++)
            {
                // Sets the values in the new points array to match the values in the prev points array
                newPoints[k] = lineRendererStartingPoints[k];
            }

            // Sets the last position in the new array to be the location the hit occured
            newPoints[count] = hitInfo.point;

            // Collided is true
            collided = true;

            // Set collision data
            spawnData.Point = hitInfo.point;
            spawnData.Rotation = (int)Vector2.Angle(hitInfo.normal, Vector2.up);
            spawnData.Valid = false;
        }

        /// <summary>
        /// Removes the line
        /// </summary>
        public void DeleteLine()
        {
            if (!lineRenderer) return;

            lineRenderer.positionCount = 0;
            //aimLineVFX.Stop();
            // Debug.Log("Deleting Line");

            // Hide the indicator
            // Calls to:
            //  - MushroomIndicator.HideIndicator()
            onHideIndicator.Raise(this, null);
        }

        /// <summary>
        /// Get the direction of the throw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void GetLineDirection(Component sender, object data)
        {
            if (sender is not MushroomThrow) return;

            ((MushroomThrow)sender).SetThrowMultiplier(throwMultiplier);
            ((MushroomThrow)sender).SetThrowDirection(throwDirection);
        }

        /// <summary>
        /// Set the throw path of the current line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void GetSpawnData(Component sender, object data)
        {
            // Check if the data is the correct type
            if (sender is not MushroomThrow) return;

            ((MushroomThrow)sender).SetSpawnData(currentSpawnData);
        }

        /// <summary>
        /// Show the Throw UI Line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void ShowLine(Component sender, object data)
        {
            // Check if the data is the correct type
            if (data is not bool) return;

            show = (bool)data;
            //aimLineVFX.Play();
            //Debug.Log("Showing Line");
        }

        /// <summary>
        /// Hide the Throw UI Line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void DisableLine(Component sender, object data)
        {
            if (data is not bool) return;

            show = (bool)data;
            DeleteLine();
        }

        /// <summary>
        /// Set cursor position
        /// </summary>
        /// <param name="cursorPosition">Cursor position to set</param>
        public void SetCursorPosition(Vector2 cursorPosition)
        {
            this.cursorPosition = cursorPosition;
        }

        //private IEnumerator SpawnUIParticles(Vector3[] points)
        //{
        //    for (int i = 0; i < points.Length; i++)
        //    {
        //        aimLineVFX.SetVector2("SpawnPosition", new Vector2(points[i].x, points[i].y));
        //        yield return null;
        //    }
        //}
    }
}
