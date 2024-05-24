using GoodLuckValley.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    public class NoteModel
    {
        public readonly ObservableList<Tab> tabs = new ObservableList<Tab>();
        public readonly ObservableList<Note> notes = new ObservableList<Note>();

        public void AddTab(Tab tab) => tabs.Add(tab);
        public void AddNote(Note note) => notes.Add(note);
    }

    public class Note
    {
        public readonly NoteData data;

        public Note(NoteData data)
        {
            this.data = data;
        }
    }
}