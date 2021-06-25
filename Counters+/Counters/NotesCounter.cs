﻿using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using TMPro;
using UnityEngine;

namespace CountersPlus.Counters
{
    internal class NotesCounter : Counter<NoteConfigModel>, INoteEventHandler
    {
        private int goodCuts = 0;
        private int allCuts = 0;
        private TMP_Text counter;

        public override void CounterInit()
        {
            GenerateBasicText("Notes", out counter);
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            allCuts++;
            if (data.colorType != ColorType.None && info.allIsOK) goodCuts++;
            RefreshText();
        }

        public void OnNoteMiss(NoteData data)
        {
            if (data.colorType == ColorType.None) return;
            allCuts++;
            RefreshText();
        }

        private void RefreshText()
        {
            counter.text = $"{goodCuts} / {allCuts}";
            if (Settings.ShowPercentage)
            {
                float percentage = (float)goodCuts / allCuts * 100.0f;
                counter.text += $" - {percentage.ToString($"F{Settings.DecimalPrecision}")}%";
            }

            counter.color= Settings.CustomNoteColors ? Settings.GeLeftColorFromLeft((goodCuts / allCuts) * 100) : Color.white;
        }
    }
}
