using System;
using System.Linq;

namespace MFG.Bowling
{
	public class Player
	{
		public string Name { get; private set; }
		public Frame[] Frames { get; private set; }
		public int TotalScore { get { return Frames.Sum(frame => frame.TotalScore); } }

		private int currentFrame;
		private int remainingRolls;
		private bool bonusRollAvailable;
		private readonly ScoreTracker scoreTracker;

		public event Action<Frame> OnFrameComplete;
		public event Action OnFramesCompleted;

		public Player(string name)
		{
			Name = name;

			Frames = new Frame[Constants.framesPerMatch];
			for (var i = 0; i < Frames.Length; i++)
			{
				Frames[i] = new Frame(i);
			}

			scoreTracker = new ScoreTracker(Frames);

			remainingRolls = 2;
		}

		public void Roll(int score)
		{
			if (score > Constants.maxRollScore)
			{
				throw new ArgumentOutOfRangeException(string.Format("Score cannot be more than {0}.", Constants.maxRollScore));
			}

			if (currentFrame >= Constants.framesPerMatch)
			{
				throw new Exception("No more frames to roll for.");
			}

			// Get the current frame
			var frame = Frames[currentFrame];

			var endOfFrame = false;
			var trackRolls = 0;

			// Add the score
			var frameType = GetScoreType(score, frame.Roll.FirstOrDefault());
			frame.Roll.Add(new Roll(frameType, score));

			// Set the frame type based on the score
			frame.ScoreType = score > 0 || frame.TotalScore > 0 ? FrameScoreType.Standard : FrameScoreType.Miss;

			// Check if it was a strike
			if (IsStrike(score))
			{
				frame.ScoreType = FrameScoreType.Strike;

				// Grant a bonus roll if applicable
				if (IsLastFrame() && bonusRollAvailable)
				{
					remainingRolls += 1;
					bonusRollAvailable = false;
				}
				// else register the strike and move on to the next frame
				else if (!IsLastFrame())
				{
					endOfFrame = true;
					trackRolls = 2;
				}
			}

			// Reduce the remaining rolls
			remainingRolls -= 1;

			// Check if there are no more rolls remaining, if true then move to the next frame
			if (remainingRolls <= 0)
			{
				if (!IsLastFrame() && frame.TotalScore == Constants.maxRollScore)
				{
					frame.ScoreType = FrameScoreType.Spare;
					trackRolls = 1;
				}

				endOfFrame = true;
			}

			// Update the score tracker with this rolls score.
			scoreTracker.HandleRoll(score);

			// If this roll causes the frame to complete, set next frame.
			if (endOfFrame)
			{
				NextFrame();
			}

			// If this roll is a strike, register the strike
			if (trackRolls > 0)
			{
				scoreTracker.TrackRolls(frame, trackRolls);
			}
		}

		private static FrameScoreType GetScoreType(int score, Roll previousRoll = null)
		{
			switch (score)
			{
				case 0: return FrameScoreType.Miss;
				case Constants.maxRollScore: return FrameScoreType.Strike;
			}

			return previousRoll != null && score + previousRoll.Score == Constants.maxRollScore
				? FrameScoreType.Spare
				: FrameScoreType.Standard;
		}

		private void NextFrame()
		{
			if (OnFrameComplete != null)
			{
				OnFrameComplete.Invoke(Frames[currentFrame]);
			}

			currentFrame++;

			if (currentFrame >= Constants.framesPerMatch)
			{
				if (OnFramesCompleted != null)
				{
					OnFramesCompleted.Invoke();
				}

				return;
			}

			remainingRolls = 2;
			bonusRollAvailable = IsLastFrame();
		}

		private static bool IsStrike(int score)
		{
			return score == Constants.maxRollScore;
		}

		private bool IsLastFrame()
		{
			return currentFrame == Constants.framesPerMatch - 1;
		}
	}
}
