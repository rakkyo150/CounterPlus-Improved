﻿using CountersPlus.Counters.Interfaces;
using Zenject;

namespace CountersPlus.Counters.Event_Broadcasters
{
    /// <summary>
    /// A <see cref="EventBroadcaster{T}"/> that broadcasts score-related events.
    /// </summary>
    internal class ScoreEventBroadcaster : EventBroadcaster<IScoreEventHandler>
    {
        [Inject] private ScoreController scoreController;

        public override void Initialize()
        {
            scoreController.scoreDidChangeEvent += ScoreDidChangeEvent;
        }

        private void ScoreDidChangeEvent(int rawScore, int modifiedScore)
        {
            foreach (IScoreEventHandler scoreEventHandler in EventHandlers)
            {
                scoreEventHandler?.ScoreUpdated(modifiedScore);
                scoreEventHandler?.MaxScoreUpdated(modifiedScore);
            }
        }

        public override void Dispose()
        {
            scoreController.scoreDidChangeEvent -= ScoreDidChangeEvent;
        }
    }
}
