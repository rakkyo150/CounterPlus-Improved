﻿using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using CountersPlus.Utils;
using TMPro;
using UnityEngine;
using Zenject;

namespace CountersPlus.Counters
{
    /// <summary>
    /// The base class for a counter, which supplies basic utilities for that counter to use.
    /// This is barebones, and only contains events pertaining to the initialization and destruction of a counter.
    /// To access more events, inherit from <see cref="IEventHandler"/>s.
    /// </summary>
    internal class Counter<T> : ICounter where T : ConfigModel
    {
        [Inject] protected T Settings;
        [Inject] protected CanvasUtility CanvasUtility;

        public virtual void CounterInit() { }

        public virtual void CounterDestroy() { }

        protected void GenerateBasicText(string labelText, out TMP_Text count)
        {
            TMP_Text label = CanvasUtility.CreateTextFromSettings(Settings);
            label.fontSize = 3;
            label.text = labelText;

            HUDCanvas canvas = CanvasUtility.GetCanvasSettingsFromID(Settings.CanvasID);

            count = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0, -0.4f * (10 / canvas.PositionScale), 0));
            count.text = "0";
            count.fontSize = 4;
        }
    }
}
