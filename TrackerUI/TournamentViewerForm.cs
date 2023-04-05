using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class TournamentViewerForm : Form
    {
        TournamentModel tournament;
        List<int> rounds;
        List<MatchupModel> allMatchups;
        public TournamentViewerForm(TournamentModel t)
        {
            InitializeComponent();
            tournament = t;
            tournamentNameLabel.Text = t.TournamentName;
            LoadRounds();
        }

        private void WireUpMatchups(List<MatchupModel> matchups)
        {
            
            matchupListBox.DataSource = null;
            matchupListBox.DataSource = matchups;
            matchupListBox.DisplayMember = "DisplayName";
            bool isVisible = matchups.Count > 0;
            teamOneNameLabel.Visible = isVisible;
            teamOneScoreLabel.Visible = isVisible;
            teamOneScoreValue.Visible = isVisible;
            teamTwoNameLabel.Visible = isVisible;
            teamTwoScoreLabel.Visible = isVisible;
            teamTwoScoreValue.Visible = isVisible;
            versusLabel.Visible = isVisible;
            scoreButton.Visible = isVisible;
        }

        private void WireUpRounds()
        {
            roundDropDown.DataSource = null;
            roundDropDown.DataSource = rounds;
        }

        private void LoadRounds()
        {
            rounds = new List<int>();
            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                rounds.Add(round[0].MatchupRound);
            }
            WireUpRounds();
        }

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups();
        }

        private void LoadMatchups()
        {
            int currRound = (int)roundDropDown.SelectedItem;
            allMatchups = new List<MatchupModel>();
            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                if (round[0].MatchupRound == currRound)
                {
                    allMatchups = round;
                    break;
                }
            }

            if (unplayedOnlyCheckBox.Checked)
            {
                List<MatchupModel> unplayedMatchups = new List<MatchupModel>();
                foreach (MatchupModel match in allMatchups)
                {
                    if (match.Winner == null)
                    {
                        unplayedMatchups.Add(match);
                    }
                }

                WireUpMatchups(unplayedMatchups);
                return;
            }

            WireUpMatchups(allMatchups);
        }

        private void LoadMatchup()
        {
            MatchupModel currMatchup = (MatchupModel)matchupListBox.SelectedItem;

            if (currMatchup == null)
            {
                return;
            }

            for (int i = 0; i < currMatchup.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (currMatchup.Entries[0].TeamCompeting != null)
                    {
                        teamOneNameLabel.Text = currMatchup.Entries[0].TeamCompeting.TeamName;
                        teamOneScoreValue.Text = currMatchup.Entries[0].Score.ToString();
                        teamTwoNameLabel.Text = "<bye>";
                        teamTwoScoreValue.Text = "0";
                    }
                    else
                    {
                        teamOneNameLabel.Text = "Not Yet Set";
                        teamOneScoreValue.Text = "";
                    }

                }

                if (i == 1)
                {
                    if (currMatchup.Entries[1].TeamCompeting != null)
                    {
                        teamTwoNameLabel.Text = currMatchup.Entries[1].TeamCompeting.TeamName;
                        teamTwoScoreValue.Text = currMatchup.Entries[1].Score.ToString();
                    }
                    else
                    {
                        teamTwoNameLabel.Text = "Not Yet Set";
                        teamTwoScoreValue.Text = "0";
                    }
                }
            }
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchup();
        }

        private void unplayedOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups();
        }

        private string ValidateScore()
        {
            string output = "";

            bool isValidOne = int.TryParse(teamOneScoreValue.Text, out int teamOneScore);
            bool isValidTwo = int.TryParse(teamTwoScoreValue.Text, out int teamTwoScore);

            if (!isValidOne)
            {
                output = "The Score One value is not a valid number";
            }
            else if (!isValidTwo)
            {
                output = "The Score Two value is not a valid number";
            }
            else if (teamOneScore == 0 & teamTwoScore == 0)
            {
                output = "You did not enter a score for either team.";
            }
            else if(teamOneScore == teamTwoScore)
            {
                output = "We do not allow ties in this application";
            }

            return output;
        }

        private void scoreButton_Click(object sender, EventArgs e)
        {
            string errorMessage = ValidateScore();
            if (errorMessage.Length > 0)
            {
                MessageBox.Show("Input Error: " + errorMessage);
                return;
            }

            MatchupModel currMatchup = (MatchupModel)matchupListBox.SelectedItem;

            double firstTeamScore = 0;
            double secondTeamScore = 0;

            for (int i = 0; i < currMatchup.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (currMatchup.Entries[0].TeamCompeting != null)
                    {
                        bool scoreValid = double.TryParse(teamOneScoreValue.Text, out firstTeamScore);
                        if (scoreValid)
                        {
                            currMatchup.Entries[0].Score = firstTeamScore;
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 1");
                            return;
                        }
                    }

                }

                if (i == 1)
                {
                    if (currMatchup.Entries[1].TeamCompeting != null)
                    {
                        bool scoreValid = double.TryParse(teamTwoScoreValue.Text, out secondTeamScore);
                        if (scoreValid)
                        {
                            currMatchup.Entries[1].Score = secondTeamScore;
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 2");
                            return;
                        }
                    }
                }
            }

            try
            {
                TournamentLogic.UpdateTournamentResults(tournament);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The application had the following error: {ex.Message}");
                return;
            }

            LoadMatchups();
        }
    }
}
