using System.Collections.Generic;
using System.Linq;

namespace MFG.Bowling
{
	internal class ScoreTracker
	{
		private Dictionary<Frame, int> strikeTracker;

		public ScoreTracker(IEnumerable<Frame> frames)
		{
			strikeTracker = new Dictionary<Frame, int>();
			foreach (var frame in frames)
			{
				strikeTracker.Add(frame, 0);
			}
		}

		public void TrackRolls(Frame frame, int rollsToTrack)
		{
			strikeTracker[frame] = rollsToTrack;
		}

		public void HandleRoll(int score)
		{
			for (var i = 0; i < strikeTracker.Count; i++)
			{
				var kvp = strikeTracker.ElementAt(i);
				if (kvp.Value <= 0) continue;
				kvp.Key.BonusScore += score;
				strikeTracker[kvp.Key] = kvp.Value - 1;
			}
		}
	}
}
