using GoodLuckValley.Persistence;
using UnityEngine;

namespace GoodLuckValley.Player.Mushroom
{
    public class MushroomSaveHandler : MonoBehaviour, IBind<MushroomData>
    {
        [SerializeField] private MushroomData mushroomData;
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
        private MushroomSpawner spawner;
        private bool unlocked;
        
        private void Awake()
        {
            // Get components
            spawner = GetComponent<MushroomSpawner>();
        }
        
        public void UnlockMushroom()
        {
            // Unlock the shroo mfor use
            unlocked = true;
            spawner.CanSpawnShroom = unlocked;
            
            // Update save data
            mushroomData.Unlocked = unlocked;
        }
        
        public void Bind(MushroomData data)
        {
            // Bind the data
            mushroomData = data;
            mushroomData.ID = ID;
            
            unlocked = mushroomData.Unlocked;
            spawner.CanSpawnShroom = unlocked;
        }
    }
}
