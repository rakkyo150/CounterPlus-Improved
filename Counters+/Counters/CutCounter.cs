﻿using System.Collections.Generic;
using System.Globalization;
using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using TMPro;
using UnityEngine;

namespace CountersPlus.Counters
{
    internal class CutCounter : Counter<CutConfigModel>, INoteEventHandler, ISaberSwingRatingCounterDidFinishReceiver
    {
        private TMP_Text cutCounterLeft;
        private TMP_Text cutCounterRight;
        private int totalCutCountLeft = 0;
        private int totalCutCountRight = 0;
        private int[] totalScoresLeft = new int[] { 0, 0, 0 }; // [0]=beforeCut, [1]=afterCut, [2]=cutDistance
        private int[] totalScoresRight = new int[] { 0, 0, 0 };

        // For handling cut calculations
        private Dictionary<ISaberSwingRatingCounter, NoteCutInfo> noteCutInfos = new Dictionary<ISaberSwingRatingCounter, NoteCutInfo>();

        public override void CounterInit()
        {
            string defaultValue = FormatLabel(0, 0, Settings.AveragePrecision);

            var label = CanvasUtility.CreateTextFromSettings(Settings);
            label.text = "Average Cut";
            label.fontSize = 3;
            label.color = Color.white;

            Vector3 leftOffset = Vector3.up * -0.2f;
            TextAlignmentOptions leftAlign = TextAlignmentOptions.Top;
            if (Settings.SeparateSaberCounts)
            {
                cutCounterRight = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0.2f, -0.2f, 0));
                cutCounterRight.lineSpacing = -26;
                cutCounterRight.text = Settings.SeparateCutValues ? $"{defaultValue}\n{defaultValue}\n{defaultValue}" : $"{defaultValue}";
                cutCounterRight.alignment = TextAlignmentOptions.TopLeft;
                cutCounterRight.color = Color.white;

                leftOffset = new Vector3(-0.2f, -0.2f, 0);
                leftAlign = TextAlignmentOptions.TopRight;
            }

            cutCounterLeft = CanvasUtility.CreateTextFromSettings(Settings, leftOffset);
            cutCounterLeft.lineSpacing = -26;
            cutCounterLeft.text = Settings.SeparateCutValues ? $"{defaultValue}\n{defaultValue}\n{defaultValue}" : $"{defaultValue}";
            cutCounterLeft.alignment = leftAlign;
            cutCounterRight.color = Color.white;
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            if (data.colorType == ColorType.None || !info.allIsOK) return;
            noteCutInfos.Add(info.swingRatingCounter, info);
            info.swingRatingCounter.UnregisterDidFinishReceiver(this);
            info.swingRatingCounter.RegisterDidFinishReceiver(this);
        }

        public void OnNoteMiss(NoteData data) { }

        public void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter v)
        {
            ScoreModel.RawScoreWithoutMultiplier(v, noteCutInfos[v].cutDistanceToCenter, out int beforeCut, out int afterCut, out int cutDistance);

            if (noteCutInfos[v].saberType == SaberType.SaberA)
            {
                totalScoresLeft[0] += beforeCut;
                totalScoresLeft[1] += afterCut;
                totalScoresLeft[2] += cutDistance;
                ++totalCutCountLeft;
            }
            else
            {
                totalScoresRight[0] += beforeCut;
                totalScoresRight[1] += afterCut;
                totalScoresRight[2] += cutDistance;
                ++totalCutCountRight;
            }

            UpdateLabels();

            noteCutInfos.Remove(v);
        }

        private void UpdateLabels()
        {
            int shownDecimals = Settings.AveragePrecision;

            if (Settings.SeparateCutValues && Settings.SeparateSaberCounts) // Before/After/Distance, for both sabers
            {
                string[] leftAverages = new string[3]
                {
                    FormatLabel(totalScoresLeft[0], totalCutCountLeft, shownDecimals),
                    FormatLabel(totalScoresLeft[1], totalCutCountLeft, shownDecimals),
                    FormatLabel(totalScoresLeft[2], totalCutCountLeft, shownDecimals)
                };
                string[] rightAverages = new string[3]
                {
                    FormatLabel(totalScoresRight[0], totalCutCountRight, shownDecimals),
                    FormatLabel(totalScoresRight[1], totalCutCountRight, shownDecimals),
                    FormatLabel(totalScoresRight[2], totalCutCountRight, shownDecimals)
                };
                cutCounterLeft.text = $"{leftAverages[0]}\n{leftAverages[1]}\n{leftAverages[2]}";
                cutCounterRight.text = $"{rightAverages[0]}\n{rightAverages[1]}\n{rightAverages[2]}";
                cutCounterLeft.color = Settings.CustomCutColors ? Settings.GetCutColorFromCut(SafeDivideScore(totalScoresLeft[0] + totalScoresLeft[1] + totalScoresLeft[2], totalCutCountLeft)) : Color.white;
                cutCounterRight.color = Settings.CustomCutColors ? Settings.GetCutColorFromCut(SafeDivideScore(totalScoresRight[0] + totalScoresRight[1] + totalScoresRight[2], totalCutCountRight)) : Color.white;
            }
            else if (Settings.SeparateCutValues) // Before/After/Distance, for combined sabers
            {
                int totalCutCount = totalCutCountLeft + totalCutCountRight;
                string[] cutValueAverages = new string[3]
                {
                    FormatLabel(totalScoresLeft[0] + totalScoresRight[0], totalCutCount, shownDecimals),
                    FormatLabel(totalScoresLeft[1] + totalScoresRight[1], totalCutCount, shownDecimals),
                    FormatLabel(totalScoresLeft[2] + totalScoresRight[2], totalCutCount, shownDecimals)
                };
                cutCounterLeft.text = $"{cutValueAverages[0]}\n{cutValueAverages[1]}\n{cutValueAverages[2]}";
                cutCounterLeft.color = Settings.CustomCutColors ? Settings.GetCutColorFromCut(SafeDivideScore(totalScoresLeft[0] + totalScoresRight[0] + totalScoresLeft[1] + totalScoresRight[1] + totalScoresLeft[2] + totalScoresRight[2], totalCutCount)) : Color.white;
            }
            else if (Settings.SeparateSaberCounts) // Combined cut, for separate sabers
            {
                string[] saberAverages = new string[2]
                {
                    FormatLabel(totalScoresLeft[0] + totalScoresLeft[1] + totalScoresLeft[2], totalCutCountLeft, shownDecimals),
                    FormatLabel(totalScoresRight[0] + totalScoresRight[1] + totalScoresRight[2], totalCutCountRight, shownDecimals)
                };
                cutCounterLeft.text = $"{saberAverages[0]}";
                cutCounterRight.text = $"{saberAverages[1]}";
                cutCounterLeft.color = Settings.CustomCutColors ? Settings.GetCutColorFromCut(SafeDivideScore(totalScoresLeft[0] + totalScoresLeft[1] + totalScoresLeft[2], totalCutCountLeft)) : Color.white;
                cutCounterRight.color = Settings.CustomCutColors ? Settings.GetCutColorFromCut(SafeDivideScore(totalScoresRight[0] + totalScoresRight[1] + totalScoresRight[2], totalCutCountRight)) : Color.white;
            }
            else // Combined cut, for combined sabers
            {
                string averages = FormatLabel(totalScoresLeft[0] + totalScoresLeft[1] + totalScoresLeft[2] + totalScoresRight[0] + totalScoresRight[1] + totalScoresRight[2], totalCutCountLeft + totalCutCountRight, shownDecimals);
                cutCounterLeft.text = $"{averages}";
                cutCounterLeft.color = Settings.CustomCutColors ? Settings.GetCutColorFromCut(SafeDivideScore(totalScoresLeft[0] + totalScoresRight[0] + totalScoresLeft[1] + totalScoresRight[1] + totalScoresLeft[2] + totalScoresRight[2], totalCutCountLeft + totalCutCountRight)) : Color.white;
            }
        }

        private double SafeDivideScore(int total, int count)
        {
            double result = (double)total / count;
            return double.IsNaN(result) ? 0 : result;
        }

        private string FormatLabel(int totalScore, int totalCuts, int decimals)
        {
            return SafeDivideScore(totalScore, totalCuts).ToString($"F{decimals}", CultureInfo.InvariantCulture);
        }
    }
}
