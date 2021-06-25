﻿using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using CountersPlus.Counters.NoteCountProcessors;
using System.Linq;
using TMPro;
using Zenject;
using UnityEngine;

namespace CountersPlus.Counters
{
    internal class NotesLeftCounter : Counter<NotesLeftConfigModel>, INoteEventHandler
    {
        [Inject] private GameplayCoreSceneSetupData setupData;
        [Inject] private NoteCountProcessor noteCountProcessor;

        private int notesLeft = 0;
        private TMP_Text counter;
        private int finishNotes = 0;

        public override void CounterInit()
        {
            if (setupData.practiceSettings != null && setupData.practiceSettings.startInAdvanceAndClearNotes)
            {
                float startTime = setupData.practiceSettings.startSongTime;
                // This LINQ statement is to ensure compatibility with Practice Mode / Practice Plugin
                notesLeft = noteCountProcessor.Data.Count(x => x.time > startTime);
            }
            else
            {
                notesLeft = noteCountProcessor.NoteCount;
            }

            if (Settings.LabelAboveCount)
            {
                GenerateBasicText("Notes Remaining", out counter);
                counter.text = notesLeft.ToString();
            }
            else
            {
                counter = CanvasUtility.CreateTextFromSettings(Settings);
                counter.text = $"Notes Remaining: {notesLeft}";
                counter.fontSize = 2;
            }

            counter.color = Settings.CustomLeftColors?Settings.Left7Color:Color.white;
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            if (data.colorType != ColorType.None && !noteCountProcessor.ShouldIgnoreNote(data))
            {
                DecrementCounter();
                finishNotes++;
            }
        }

        public void OnNoteMiss(NoteData data)
        {
            if (data.colorType != ColorType.None && !noteCountProcessor.ShouldIgnoreNote(data))
            {
                DecrementCounter();
                finishNotes++;
            }
        }

        private void DecrementCounter()
        {
            --notesLeft;
            if (Settings.LabelAboveCount) counter.text = notesLeft.ToString();
            else counter.text = $"Notes Remaining: {notesLeft}";

            counter.color = Settings.CustomLeftColors ? Settings.GetLeftColorFromLeft((notesLeft / finishNotes)*100) : Color.white;
        }
    }
}

