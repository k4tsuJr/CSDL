using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace QLVeTau
{
    public partial class Form1 : Form
    {
        public string connectionString = @"Data Source=MSI;Initial Catalog=QLVETAU;Integrated Security=True";
        public Form1()
        {
            InitializeComponent();
            loadData();
        }
        void loadData()
        {
            dataInfoPassenger.DataSource = getDataGridPassenger().Tables[0];
            dataInfoStaff.DataSource = getDataGridStaff().Tables[0];
        }
        #region[Hành khách]
        DataSet getQuery1()
        {
            DataSet data = new DataSet();

            string query = "SELECT COUNT(*) AS SOLUONGHANHKHACHNAM FROM HANHKHACH WHERE GIOITINH = 'Nam'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(data);

                connection.Close();
            }

            return data;
        }
        DataSet getQuery2()
        {
            DataSet data = new DataSet();

            string query = "SELECT * FROM HANHKHACH LEFT JOIN VE ON HANHKHACH.ID_HK = VE.ID_HK AND VE.ID_CHUYEN = (SELECT ID_CHUYEN FROM LICHTRINH WHERE ID_GA_XP = 1 AND ID_GA_KT = 2) WHERE VE.ID_VE IS NULL";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(data);
                
                connection.Close();
            }

            return data;
        }
        DataSet getDataGridPassenger()
        {
            DataSet data = new DataSet();

            string query = "SELECT * FROM HANHKHACH";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(data);

                connection.Close();
            }
            return data;
        }
        DataSet getDataGridStaff()
        {
            DataSet data = new DataSet();

            string query = "SELECT * FROM NHANVIEN";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(data);

                connection.Close();
            }
            return data;
        }
        void addPassenger()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO HANHKHACH (ID_HK, HO, TEN, GIOITINH, NGAYSINH, CMND, DIACHI, DIENTHOAI, EMAIL) VALUES (@ID_HK, @HO, @TEN, @GIOITINH, @NGAYSINH, @CMND, @DIACHI, @DIENTHOAI, @EMAIL)", connection))
                {
                    command.Parameters.Add(new SqlParameter("@ID_HK", SqlDbType.VarChar, 8)).Value = textMaKH.Text;
                    command.Parameters.Add(new SqlParameter("@HO", SqlDbType.NVarChar, 20)).Value = textHo.Text;
                    command.Parameters.Add(new SqlParameter("@TEN", SqlDbType.NVarChar, 20)).Value = textName.Text;
                    command.Parameters.Add(new SqlParameter("@GIOITINH", SqlDbType.NVarChar, 5)).Value = cbSex.Text;
                    command.Parameters.Add(new SqlParameter("@NGAYSINH", SqlDbType.Date)).Value = timePassenger.Text;
                    command.Parameters.Add(new SqlParameter("@CMND", SqlDbType.VarChar, 10)).Value = textCCCD.Text;
                    command.Parameters.Add(new SqlParameter("@DIACHI", SqlDbType.NVarChar, 100)).Value = textAdress.Text;
                    command.Parameters.Add(new SqlParameter("@DIENTHOAI", SqlDbType.VarChar, 20)).Value = textPhone.Text;
                    command.Parameters.Add(new SqlParameter("@EMAIL", SqlDbType.NVarChar, 100)).Value = textEmail.Text;

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    // Check rowsAffected to see if the insert was successful.

                    // Execute the trigger.
                    using (SqlCommand triggerCommand = new SqlCommand("ENABLE TRIGGER trg_HANHKHACH_Insert ON HANHKHACH", connection))
                    {
                        connection.Open();
                        triggerCommand.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                addPassenger();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void showInfo(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = dataInfoPassenger.CurrentRow.Index;
            textMaKH.Text = dataInfoPassenger.Rows[i].Cells[0].Value.ToString();
            textHo.Text = dataInfoPassenger.Rows[i].Cells[1].Value.ToString();
            textName.Text = dataInfoPassenger.Rows[i].Cells[2].Value.ToString();
            cbSex.Text = dataInfoPassenger.Rows[i].Cells[3].Value.ToString();
            timePassenger.Text = dataInfoPassenger.Rows[i].Cells[4].Value.ToString();
            textCCCD.Text = dataInfoPassenger.Rows[i].Cells[5].Value.ToString();
            textAdress.Text = dataInfoPassenger.Rows[i].Cells[6].Value.ToString();
            textPhone.Text = dataInfoPassenger.Rows[i].Cells[7].Value.ToString();
            textEmail.Text = dataInfoPassenger.Rows[i].Cells[8].Value.ToString();
        }
        private void btnQuery1_Click(object sender, EventArgs e)
        {
            dataQueryPassenger.DataSource = getQuery1().Tables[0];
        }
        private void btnQuery2_Click(object sender, EventArgs e)
        {
            dataQueryPassenger.DataSource = getQuery2().Tables[0];
        }
        #endregion


    }
}
