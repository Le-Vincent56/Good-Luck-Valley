using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GoodLuckValley.Persistence
{
    [Serializable]
    public class GameData
    {
        public int Slot;
        public long LastUpdated;
        public string Name;
        public string CurrentLevelName;
        public PlayerSaveData playerSaveData;
        public JournalSaveData journalSaveData;
        public GlobalData globalData;
        public TutorialData tutorialData;
        public List<CollectibleSaveData> collectibleSaveDatas;

        public override string ToString()
        {
            string finalString = "";

            finalString += $"Slot: {1}, Name: {Name}, Level: {CurrentLevelName}\n";
            finalString += playerSaveData.ToString() + "\n";
            finalString += journalSaveData.ToString() + "\n";
            finalString += globalData.ToString() + "\n";
            finalString += tutorialData.ToString() + "\n";

            return finalString;
        }
    }

    [Serializable]
    public class PlayerSaveData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public Vector3 position;

        public PlayerSaveData()
        {
            position = new Vector3(-122.7588f, 97.67071f, 0.0f);
        }

        public override string ToString()
        {
            string finalString = "";

            finalString += $"Position: {position}";

            return finalString;
        }
    }

    [Serializable]
    public class GlobalData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool unlockedThrow;
        public bool unlockedWallJump;

        public GlobalData()
        {
            unlockedThrow = false;
            unlockedWallJump = false;
        }

        public override string ToString()
        {
            string finalString = "";

            finalString += $"Unlocked Throw: {unlockedThrow}, ";
            finalString += $"Unlocked Wall Jump: {unlockedWallJump}";

            return finalString;
        }
    }

    [Serializable]
    public class CollectibleSaveData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool collected;

        public CollectibleSaveData()
        {
            collected = false;
        }
    }

    [Serializable]
    public class JournalSaveData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool unlocked;
        public int progressingIndex;
        public int lastOpenedIndex;
        public int notesCollectedNum;
        public int journalEntriesUnlocked;

        public JournalSaveData()
        {
            unlocked = false;
            progressingIndex = 0;
            lastOpenedIndex = 0;
            notesCollectedNum = 0;
            journalEntriesUnlocked = 0;
        }

        public override string ToString()
        {
            string finalString = "";

            finalString += $"Unlocked Journal: {unlocked}, ";
            finalString += $"Progressing Index: {progressingIndex}, ";
            finalString += $"Last Opened Index: {lastOpenedIndex}, ";
            finalString += $"Notes Collected: {notesCollectedNum}, ";
            finalString += $"Entries Unlocked: {journalEntriesUnlocked}";

            return finalString;
        }
    }

    [Serializable]
    public class PowersData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool UnlockedThrow;
        public bool UnlockedWallJump;
    }

    [Serializable]
    public class TutorialData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public bool SeenMoveTrigger;
        public bool SeenJumpTrigger;
        public bool SeenSlideTrigger;
        public bool SeenFastFallTrigger;
        public bool SeenCrawlTrigger;
        public bool SeenInteractTrigger;
        public bool SeenAimTrigger;
        public bool SeenThrowTrigger;
        public bool SeenPeekTrigger;
        public bool SeenTotalRecallTrigger;
        public bool SeenSingleRecallTrigger;
        public bool SeenQuickBounceTrigger;
        public bool SeenChainBounceTrigger;
        public bool Moved;
        public bool Jumped;
        public bool Slid;
        public bool FastFallen;
        public bool Crawled;
        public bool Interacted;
        public bool Aimed;
        public bool Thrown;
        public bool Peeked;
        public bool TotalRecall;
        public bool SingleRecall;
        public bool QuickBounced;
        public bool ChainBounced;

        public TutorialData()
        {
            SeenMoveTrigger = false;
            SeenJumpTrigger = false;
            SeenSlideTrigger = false;
            SeenFastFallTrigger = false;
            SeenCrawlTrigger = false;
            SeenInteractTrigger = false;
            SeenAimTrigger = false;
            SeenThrowTrigger = false;
            SeenPeekTrigger = false;
            SeenTotalRecallTrigger = false;
            SeenSingleRecallTrigger = false;
            SeenQuickBounceTrigger = false;
            SeenChainBounceTrigger = false;

            Moved = false;
            Jumped = false;
            Slid = false;
            FastFallen = false;
            Crawled = false;
            Interacted = false;
            Aimed = false;
            Thrown = false;
            Peeked = false;
            TotalRecall = false;
            SingleRecall = false;
            QuickBounced = false;
            ChainBounced = false;
        }

        public override string ToString()
        {
            string finalString = "";

            finalString += $"Has Moved: {Moved}, ";
            finalString += $"Has Jumped: {Jumped}, ";
            finalString += $"Has Slid: {Slid}, ";
            finalString += $"Has FastFallen: {FastFallen}, ";
            finalString += $"Has Crawled: {Crawled}, ";
            finalString += $"Has Interacted: {Interacted}, ";
            finalString += $"Has Aimed: {Aimed}, ";
            finalString += $"Has Thrown: {Thrown}, ";
            finalString += $"Has Peeked: {Peeked}, ";
            finalString += $"Has TotalRecall: {TotalRecall}, ";
            finalString += $"Has SingleRecall: {SingleRecall}, ";
            finalString += $"Has QuickBounced: {QuickBounced}, ";
            finalString += $"Has ChainBounced: {ChainBounced}";

            return finalString;
        }
    }
}
