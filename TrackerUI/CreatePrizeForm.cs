﻿using System;
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
    public partial class CreatePrizeForm : Form
    {
        IPrizeRequester callingForm;
        public CreatePrizeForm(IPrizeRequester caller)
        {
            InitializeComponent();
            callingForm = caller;
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if (TryValidateData())
            {
                PrizeModel model = new PrizeModel(
                    placeNumberValue.Text,
                    placeNameValue.Text,
                    prizeAmountValue.Text,
                    prizePercentageValue.Text);


                GlobalConfig.Connection.CreatePrize(model);

                callingForm.PrizeComplete(model);

                this.Close();

                //placeNumberValue.Text = "";
                //placeNameValue.Text = "";
                //prizeAmountValue.Text = "0";
                //prizePercentageValue.Text = "0";
            }
            else
            {
                MessageBox.Show("This form has invalid information. Please, check it and try again");
            }
        }

        private bool TryValidateData()
        {
            bool output = true;

            bool isPlaceNumberValid = int.TryParse(placeNumberValue.Text, out int placeNumber);
            if (!isPlaceNumberValid)
            {
                output = false;
            }
            if (placeNumber < 1)
            {
                output = false;
            }

            if (placeNameValue.Text.Length == 0)
            {
                output = false;
            }

            bool isPrizeAmountValid = decimal.TryParse(prizeAmountValue.Text, out decimal prizeAmount);
            bool isPrizePercentageValid = double.TryParse(prizePercentageValue.Text, out double prizePercentage);

            if (!isPrizeAmountValid || !isPrizePercentageValid)
            {
                output = false;
            }

            if (prizeAmount < 1 && prizePercentage < 1)
            {
                output = false;
            }

            if (prizePercentage < 0 && prizePercentage > 100)
            {
                output = false;
            }

            return output;
        }
    }
}
