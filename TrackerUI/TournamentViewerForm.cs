using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class TournamentViewerForm : Form
    {
        TournamentModel tournament;
        List<int> rounds;
        List<MatchupModel> matchups;
        public TournamentViewerForm(TournamentModel t)
        {
            InitializeComponent();
            tournament = t;
            tournamentNameLabel.Text = t.TournamentName;
            LoadRounds();
        }

        private void WireUpMatchups()
        {
            matchupListBox.DataSource = null;
            matchupListBox.DataSource = matchups;
            matchupListBox.DisplayMember = "DisplayName";

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
            matchups = new List<MatchupModel>();
            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                if (round[0].MatchupRound == currRound)
                {
                    matchups = round;
                    break;
                }
            }
            WireUpMatchups();
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
    }
}
