using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace raFile1
{
    public partial class Form1 : Form
    {
        private FileStream raFile = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                raFile = new FileStream("Clients.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                if(raFile.Length != 4400)
                {
                    initializeFile();
                }
                readFile();

            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Error Creating Stream");
            }
            txtBalance.KeyPress += txtBalance_KeyPress;
            txtLName.KeyPress += txtLName_KeyPress;
            txtFName.KeyPress += txtFName_KeyPress;
            txtAcctNum.KeyPress += txtAcctNum_KeyPress;
            // kill right click 
            txtBalance.ContextMenuStrip = new ContextMenuStrip();
            txtAcctNum.ContextMenuStrip = new ContextMenuStrip();
            txtFName.ContextMenuStrip = new ContextMenuStrip();
            txtLName.ContextMenuStrip = new ContextMenuStrip();
        }

       
        private void initializeFile()
        {
            MessageBox.Show("InitializeFile() called");
            AccountRecordRA ra = new AccountRecordRA();
            try
            {
                // position file pointer
                raFile.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < 100; i++)
                {
                    ra.write(raFile);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Error initilizing File");
            }
        }
        private void readFile()
        {
            listBox1.Items.Clear();
            AccountRecordRA ra = new AccountRecordRA();
            try
            {
                raFile.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < 100; i++)
                {
                    ra.read(raFile);
                    if (ra.Account > 0)
                    {
                        listBox1.Items.Add(ra.Account + ";" + ra.FirstName + ";" + ra.LastName + ";" + ra.Balance.ToString("c"));
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Error reading file");
            }
        }
        private void cmdInsert_Click(object sender, EventArgs e)
        {
            if (dataGood())
            {
                int acct = Convert.ToInt32(txtAcctNum.Text);
                if (isValidAccount(acct))
                {
                    string fn = txtFName.Text;
                    string ln = txtFName.Text;
                    double bal = Convert.ToDouble(txtBalance.Text);
                    AccountRecordRA ra = new AccountRecordRA(acct, fn, ln, bal);
                    try
                    {
                        raFile.Seek ((acct -1)*44,SeekOrigin.Begin);
                        ra.write(raFile);
                        readFile();
                        clearText();
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(ex.Message, "error inserting record");
                    }
                }
            }
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            if (dataGood())
            {
                int acct = Convert.ToInt32(txtAcctNum.Text);
                string fn = txtFName.Text;
                string ln = txtLName.Text;
                string sBal = txtBalance.Text;
                if (sBal[0] == '$')
                {
                    sBal = sBal.Remove(0, 1);
                }
                double bal = Convert.ToDouble(sBal);
                AccountRecordRA ra = new AccountRecordRA(acct, fn, ln, bal);
                try
                {
                    raFile.Seek((acct - 1) * 44, SeekOrigin.Begin);
                    ra.write(raFile);
                    readFile();
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message, "error Updating");
                }
                setControlState("i");
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this record ?", "Confirm record delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                // delete record
                int acct = Convert.ToInt32(txtAcctNum.Text);
                AccountRecordRA ra = new AccountRecordRA();
                try
                {
                    //potition file pointer
                    raFile.Seek((acct - 1) * 44, SeekOrigin.Begin);
                    ra.write(raFile);
                    readFile();

                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message, "error deleting record");
                }
            }
        }
        private void clearText()
        {
            txtAcctNum.Text = "";
            txtBalance.Text = "";
            txtFName.Text = "";
            txtLName.Text = "";
            listBox1.ClearSelected();
        }
        private bool dataGood()
        {
            return true;
        }
        private bool isValidAccount( int acct)
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                int len = listBox1.Items[i].ToString().IndexOf(";");
                string sAcct = listBox1.Items[i].ToString().Substring(0, len);
                if (acct == Convert.ToInt32(sAcct))
                {
                    MessageBox.Show("this account number exists, enter new account number", "Primary key violation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAcctNum.Focus();
                    return false;
                }
            }
                return true;
        }
        private void setControlState(string state)
        {
            if (state.Equals("i"))
            {
                txtAcctNum.Enabled = true;
                txtFName.Enabled = true;
                txtLName.Enabled = true;
                cmdInsert.Enabled = true;
                cmdUpdate.Enabled = false;
                cmdDelete.Enabled = false;
            }
            else if (state.Equals("u/d"))
            {
                txtAcctNum.Enabled = false;
                txtFName.Enabled = false;
                txtLName.Enabled = false;
                cmdInsert.Enabled = false;
                cmdUpdate.Enabled = true;
                cmdDelete.Enabled = true;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                string record = listBox1.Items[listBox1.SelectedIndex].ToString();
                string[] tokens = record.Split(new char[]{';'});
                txtAcctNum.Text = tokens[0];
                txtFName.Text = tokens[1];
                txtLName.Text = tokens[2];
                txtBalance.Text = tokens[3];
                setControlState("u/d");
                     
            } 
        }
        void txtAcctNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            int len = ((TextBox)sender).Text.Length;
            ((TextBox)sender).SelectionStart = len;
            if (c != 8)
            {
                if (len == 0 && (c < 49 || c > 57))
                {
                    e.Handled = true;
                }
                else if (len > 0 && (c < 48 || c > 57))
                {
                    e.Handled = true;
                }
            }
        }

        void txtFName_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            int len = ((TextBox)sender).Text.Length;
            ((TextBox)sender).SelectionStart = len;
            if (c != 8)
            {
                if ((c < 65 || c > 90) && (c < 97 || c > 122))
                {
                    e.Handled = true;
                }
                else if (len == 0 && (c > 96 && c < 123))
                {
                    e.KeyChar = (char)(c - 32);
                }
                //else if (len > 0 && ())
            }
        }

        void txtLName_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        void txtBalance_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            int len = ((TextBox)sender).Text.Length;
            ((TextBox)sender).SelectionStart = len;
            if (c != 8)
            {
                if (len == 0)
                {
                    if (c < 48 || c > 57)
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    if (c < 48 || c > 57)
                    {
                        if (c == 46)
                        {
                            if(((TextBox)sender).Text.IndexOf(".")>-1){
                                e.Handled = true; 
                            }
                        }
                        else
                        {
                            e.Handled = true;
                        }
                    }
                    else if (((TextBox)sender).Text.IndexOf(".") > -1)
                    {
                        if (len == ((TextBox)sender).Text.IndexOf(".") + 3)
                        {
                            e.Handled = true;
                        }
                    }
                    
                }
            }
        }
    }
}
