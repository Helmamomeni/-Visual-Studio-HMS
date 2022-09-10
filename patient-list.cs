using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HMS
{
    public partial class patient_list : Form
    {
        string mdfFile = @"database.mdb";

        public patient_list()
        {
            InitializeComponent();
        }

        private void load_patient_grid_view()
        {

            using (OleDbConnection connection = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", mdfFile)))
            {
                using (OleDbCommand selectCommand = new OleDbCommand("SELECT * FROM patient", connection))
                {
                    connection.Open();

                    DataTable table = new DataTable();
                    OleDbDataAdapter adapter = new OleDbDataAdapter();
                    adapter.SelectCommand = selectCommand;
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                    dataGridView1.Columns[0].HeaderCell.Value = "کد";
                    dataGridView1.Columns[1].HeaderCell.Value = "نام ";
                    dataGridView1.Columns[2].HeaderCell.Value = " نام خانوادگی";
                    dataGridView1.Columns[3].HeaderCell.Value = "نام پدر";
                    dataGridView1.Columns[4].HeaderCell.Value = "کد ملی";
                    dataGridView1.Columns[5].HeaderCell.Value = "سن";


                }
            }
        }


        private void patient_list_Load(object sender, EventArgs e)
        {
            load_patient_grid_view();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("شما میتوانید در این قسمت اقدام به حذف یا ویرایش بیمار کنید", "راهنما", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK) this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            using (OleDbConnection connection = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", mdfFile)))
            {
                int id = 0;
                string nationalcode = "";
                try
                {
                    id = Int16.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                    nationalcode = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                }
                catch
                {
                    MessageBox.Show("کد یا کد ملی قابل خواندن نیست");
                }
                using (OleDbCommand selectCommand = new OleDbCommand("SELECT * FROM patient where [nationalcode]='" + nationalcode + "' and [id] not in (" + id + ")", connection))
                {
                    connection.Open();
                    OleDbDataReader reader = selectCommand.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        using (OleDbCommand updateCommand = new OleDbCommand("UPDATE patient SET [username] = ?, [userfamily] = ? ,[fathername] = ?, [nationalcode] = ?,[userage] = ? WHERE [ID] = ?", connection))
                        {
                            string name = "";
                            string family = "";
                            string father = "";
                            int age = 0;

                            try
                            {
                                name = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                                family = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                                father = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                                age = Int16.Parse(dataGridView1.CurrentRow.Cells[5].Value.ToString());
                            }
                            catch
                            {
                                MessageBox.Show("مقادیر ستون های تغییر یافته صحیح نیست");
                            }
                            updateCommand.Parameters.AddWithValue("@username", name);
                            updateCommand.Parameters.AddWithValue("@userfamily", family);
                            updateCommand.Parameters.AddWithValue("@fathername", father);
                            updateCommand.Parameters.AddWithValue("@nationalcode", nationalcode);
                            updateCommand.Parameters.AddWithValue("@userage", age);
                            updateCommand.Parameters.AddWithValue("@ID", id);
                            try
                            {
                                updateCommand.ExecuteNonQuery();
                            }
                            catch
                            {
                                MessageBox.Show("عملیات بروزرسانی با خطا مواجه شد");
                            }
                            connection.Close();
                            load_patient_grid_view();
                            dataGridView1.Refresh();


                        }
                    }
                    else
                    {
                        MessageBox.Show("این کد ملی تکراری است");
                    }


                    connection.Close();
                    load_patient_grid_view();
                    dataGridView1.Refresh();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int id = 0;
            try
            {
                id = Int16.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString());
            }
            catch
            {
                MessageBox.Show("کد انتخاب شده معتبر نمی باشد.");
            }
            using (OleDbConnection connection = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", mdfFile)))
            {
                using (OleDbCommand deleteCommand = new OleDbCommand("DELETE FROM patient WHERE [ID] = ?", connection))
                {

                    connection.Open();

                    deleteCommand.Parameters.AddWithValue("@ID", id);
                    try
                    {
                        deleteCommand.ExecuteNonQuery();
                    }
                    catch
                    {
                        MessageBox.Show("عملیات حذف با خطا مواجه شد");
                    }
                    connection.Close();

                    load_patient_grid_view();

                    dataGridView1.Refresh();

                }
            }



        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("داده های وارد شده معتبر نمی باشد ");
        }
    }
}
