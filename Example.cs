using System.Linq;
using UnityEngine;

namespace MFG.Bowling
{
	internal class Example : MonoBehaviour
	{
		[SerializeField]
		private bool perfectGame;

		private Player[] players;

		private void Start()
		{
			players = new[] {new Player("Hazza"), new Player("Sabina")};

			Simulate();
		}

		private void Simulate()
		{
			for (var i = 0; i < players.Length; i++)
			{
				var player = players[i];

				// Listen to the event for all the frames to be complete.
				var framesCompleted = false;
				player.OnFramesCompleted += () =>
				{
					framesCompleted = true;
					HandlePlayerFramesCompleted(player);
				};

				// Track the current frames score so we know how many pins remaining can be knocked down.
				var currentFramesScore = 0;

				// Each time a frame is complete, reset the currentFramesScore.
				player.OnFrameComplete += frame => currentFramesScore = 0;

				// Simulate the game
				while (!framesCompleted)
				{
					var max = (currentFramesScore < Constants.maxRollScore ? Constants.maxRollScore - currentFramesScore : Constants.maxRollScore) + 1;
					var roll = perfectGame ? Constants.maxRollScore : Random.Range(0, max);
					currentFramesScore += roll;
					player.Roll(roll);
				}
			}
		}

		private static void HandlePlayerFramesCompleted(Player player)
		{
			foreach (var frame in player.Frames)
			{
				Debug.Log(frame.Roll.Aggregate("Frame:", (current, roll) => current + " / " + "roll: " + roll.Score) + (frame.BonusScore > 0 ? " / bonus: " + frame.BonusScore : "") + " - " +
				          frame.ScoreType + " - Total: " + frame.TotalScore);
			}

			Debug.Log(player.Name + " scored a total of " + player.TotalScore + " points");
		}
	}
}
