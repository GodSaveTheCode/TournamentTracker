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
    public partial class CreateTeamForm : Form
    {
        List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
        List<PersonModel> selectedTeamMembers = new List<PersonModel>();

        ITeamRequester callingForm;

        public CreateTeamForm(ITeamRequester caller)
        {
            InitializeComponent();
            WireUpLists();
            callingForm = caller;
        }

        private void WireUpLists()
        {
            selectTeamMemberDropDown.DataSource = null;
            teamMembersListBox.DataSource = null;

            selectTeamMemberDropDown.DataSource = availableTeamMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName";
            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (TryValidateData())
            {
                PersonModel person = new PersonModel()
                {
                    FirstName = firstNameValue.Text,
                    LastName = lastNameValue.Text,
                    EmailAddress = emailValue.Text,
                    CellphoneNumber = cellphoneValue.Text
                };

                person = GlobalConfig.Connection.CreatePerson(person);

                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellphoneValue.Text = "";

                selectedTeamMembers.Add(person);

                WireUpLists();
            }
            else
            {
                MessageBox.Show("You need to fill in all of the fields.");
            }
        }


        private bool TryValidateData()
        {
            if (firstNameValue.Text.Length < 1)
            {
                return false;
            }
            if (lastNameValue.Text.Length < 1)
            {
                return false;
            }
            if (emailValue.Text.Length < 1)
            {
                return false;
            }
            if (cellphoneValue.Text.Length < 1)
            {
                return false;
            }

            return true;
        }

        private void addMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel selectedPerson = (PersonModel)selectTeamMemberDropDown.SelectedItem;

            if (selectedPerson != null)
            {
                availableTeamMembers.Remove(selectedPerson);
                selectedTeamMembers.Add(selectedPerson);

                WireUpLists(); 
            }
        }

        private void removeSelectedMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel selectedPerson = (PersonModel)teamMembersListBox.SelectedItem;
            if (selectedPerson != null)
            {
                selectedTeamMembers.Remove(selectedPerson);
                availableTeamMembers.Add(selectedPerson);

                WireUpLists();
            }

        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel model = new TeamModel();
            model.TeamName = teamNameValue.Text; // TODO - Validate team name
            model.TeamMembers = selectedTeamMembers;
            GlobalConfig.Connection.CreateTeam(model);

            callingForm.TeamComplete(model);
            this.Close();
        }
    }
}
