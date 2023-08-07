using UnityEngine;

namespace HiveMind.Audio
{
    public class ChangeMusicTrigger : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private MusicArea area;
        #endregion

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                AudioManager.Instance.SetMusicArea(area);
            }
        }
    }
}
