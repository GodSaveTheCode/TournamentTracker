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
        public CreateTeamForm()
        {
            InitializeComponent();
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

                GlobalConfig.Connection.CreatePerson(person);

                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellphoneValue.Text = "";
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
    }
}
