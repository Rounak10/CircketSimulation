﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CircketSimulation;

namespace CricketSimulation
{
    public class Match
    {
        private const int BallsPerOver = 6;

        private const int overslimit = 4;
        public ScoreCard ScoreCard { get; set; }
        public List<Over> Overs { get; set; }
        public Commentary scoreDisplay { get; set; }
        public Team team { get; set; }
        public Match()
        {
            List<Team> teams = new List<Team>();
            team = new Team("Lengaburu");
            teams.Add(team);
            ScoreCard = new ScoreCard(overslimit, 40, 3, 0, team.Players);
            ScoreCard = new ScoreCard(overslimit, 40, 3, 0, teams);
            Overs = new List<Over>();
            scoreDisplay = new Commentary();
        }

        public bool ConductMatch()
        {
            Player firstPlayer = ScoreCard.Teams[0].Players[0];
            Player secondPlayer = ScoreCard.Teams[0].Players[1];
            firstPlayer.IsBattingNow = true;
            firstPlayer.IsplayingCurrently = true;
            secondPlayer.IsplayingCurrently = true;
            Result result = Result.Continue;
            for (int i = 0; i < overslimit; i++)
            {
                result = ConductOver(ref firstPlayer, ref secondPlayer);
                ScoreCard.updateOver();
                scoreDisplay.CommentarybyOver(ScoreCard, Overs);
                if (result == Result.Won || result == Result.Allout)
                {
                    if(result == Result.Won)
                    {
                        scoreDisplay.CommentaryBymatchifWon(ScoreCard, Overs);
                    }
                    else if(result == Result.Allout)
                    {
                        scoreDisplay.CommentaryBymatchifLost(ScoreCard, Overs);
                    }else if(result == Result.MatchTied)
                    {

                    }
                    break;
                }
               
                ChangeStrikeAfterOver(ref firstPlayer, ref secondPlayer,Overs[Overs.Count-1]);
            }
            if(ScoreCard.OversLeft == 0 && ScoreCard.RunsScored < ScoreCard.Target && result != Result.Allout)
            {
                scoreDisplay.CommentaryBymatchifLost(ScoreCard, Overs);
            }
            return true;
        }

        private void ChangeStrikeAfterOver(ref Player firstPlayer, ref Player secondPlayer, Over over)
        {
            Ball lastBallofOver = over.Balls[over.Balls.Count - 1];
            //if (lastBallofOver.RunsScored == 0 || lastBallofOver.RunsScored == 2 || lastBallofOver.RunsScored == 4 || lastBallofOver.RunsScored == 6)
                SwapPlayers(ref firstPlayer, ref secondPlayer);
        }

        private Result ConductOver(ref Player striker,ref Player runner)
        {
            Over over = new Over();
            over.overNumber = Overs.Count ;
            Overs.Add(over);
            for (int i = 0; i < BallsPerOver; i++)
            {
                System.Threading.Thread.Sleep(1000);
                Ball newBall = new Ball(striker.Name);
                over.Bowler.Bowl(ref striker, newBall);
                striker.updateScore(newBall);
                over.updateScore(newBall);
                
                if (!newBall.IsWicket)
                {
                    ScoreCard.updateScore(newBall);
                }else
                { 
                    ScoreCard.updateScore(newBall);
                    
                }
                if (over.ControlPlayerBetweenWickets(newBall))
                {
                    SwapPlayers(ref striker, ref runner);
                }
                scoreDisplay.commentarybyBall(newBall, over);
                if (striker.isOutorNot)
                {
                    scoreDisplay.CommentaryAfterWicket(striker);
                    striker = GetTheNextPlayer();
                    if(striker != null)
                        striker.IsplayingCurrently = true;
                }
                
                if (ScoreCard.RunsScored > ScoreCard.Target)
                    return Result.Won;
                if (ScoreCard.RunsScored == ScoreCard.Target && over.overNumber == overslimit && i == BallsPerOver)
                    return Result.MatchTied;
                if (ScoreCard.WicketsLeft == 0)
                    return Result.Allout;

            }
            return Result.Continue;
        }

        private Player GetTheNextPlayer()
        {
            return ScoreCard.Teams[0].Players.Where(x => x.IsplayingCurrently == false).FirstOrDefault();
        }

        private void SwapPlayers(ref Player striker, ref Player runner)
        {
            Player temp = striker;
            striker = runner;
            runner = temp;
        }

        public static void Main()
        {
            Match match = new Match();
            bool result = match.ConductMatch();
            Console.ReadLine();
        }
    }
}
