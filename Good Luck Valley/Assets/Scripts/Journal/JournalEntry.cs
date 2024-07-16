using System;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    [Serializable]
    public class JournalEntry
    {
        [SerializeField] private string title;
        [SerializeField] private string content;
        [SerializeField] private int progress;
        [SerializeField] private bool completed;

        public JournalEntry(string title, string content)
        {
            this.title = title;
            this.content = content;
            progress = 0;
            completed = false;
        }

        public string Title { get => title; }
        public string Content { get => content; }
        public int Progress { get => progress; }
        public bool Completed { get => completed; }

        /// <summary>
        /// Add content to the Journal Entry
        /// </summary>
        /// <param name="content"></param>
        public void AddContent(string content) => this.content += $" {content}";

        public void UpdateProgress(int progress)
        {
            this.progress = progress;

            if (progress == 3)
                Complete();
        }

        /// <summary>
        /// Complete the Journal Entry
        /// </summary>
        public void Complete() => completed = true;

        /// <summary>
        /// Print the note
        /// </summary>
        public void Print()
        {
            Debug.Log($"Journal Entry Title: {title} \n" +
                $"Journal Entry Content: {content}");
        }
    }
}