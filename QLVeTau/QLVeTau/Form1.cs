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
        DataSet getQueryPassenger()
        {
            DataSet data = new DataSet();
            if (cbQueryPassenger.Text.ToLower().StartsWith("số lượng hành khách có giới tính là 'nam'")) 
            {               
                string query = "SELECT COUNT(*) AS SOLUONGHANHKHACHNAM FROM HANHKHACH WHERE GIOITINH = 'Nam'";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(data);

                    connection.Close();
                }                
            }
            if (cbQueryPassenger.Text.ToLower().StartsWith("danh sách các hành khách chưa đặt vé cho chuyến tàu từ ga x đến ga y"))
            {
                string query = "SELECT * FROM HANHKHACH LEFT JOIN VE ON HANHKHACH.ID_HK = VE.ID_HK AND VE.ID_CHUYEN = (SELECT ID_CHUYEN FROM LICHTRINH WHERE ID_GA_XP = 1 AND ID_GA_KT = 2) WHERE VE.ID_VE IS NULL";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(data);

                    connection.Close();
                }
            }
            if (cbQueryPassenger.Text.StartsWith("Cập nhật thông tin của khách hàng có ID_HK là 'KH001'"))
            {
                string query = "UPDATE HANHKHACH SET TEN = 'Nguyen Van A', DIACHI = '123 Nguyen Trai' WHERE ID_HK = 'KH001'";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(data);

                    connection.Close();
                }
            }
            if (cbQueryPassenger.Text.StartsWith("Xóa thông tin của khách hàng có ID_HK là 'KH002'"))
            {
                string query = "DELETE FROM HANHKHACH WHERE ID_HK = 'KH002'";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(data);

                    connection.Close();
                }
            }
            if (cbQueryPassenger.Text.StartsWith("Danh sách các hành khách đã mua vé và thống kê số lần mua vé của mỗi hành khách"))
            {
                string query = "SELECT HK.HO, HK.TEN, HK.CMND, COUNT(V.ID_VE) AS SO_LAN_MUA_VE FROM HANHKHACH HK INNER JOIN VE V ON HK.ID_HK = V.ID_HK GROUP BY HK.HO, HK.TEN, HK.CMND";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(data);

                    connection.Close();
                }
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
        void editPassenger()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("UPDATE HANHKHACH SET HO = @HO, TEN = @TEN, GIOITINH = @GIOITINH, NGAYSINH = @NGAYSINH, CMND = @CMND, DIACHI = @DIACHI, DIENTHOAI = @DIENTHOAI, EMAIL = @EMAIL WHERE ID_HK = @ID_HK", connection))
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
                }
            }
        }
        void delPassenger()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("XOA_HANHKHACH", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_HK", "HK000");
                connection.Open();
                command.ExecuteNonQuery();
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
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                addPassenger();
                dataInfoPassenger.DataSource = getDataGridPassenger().Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                delPassenger();
                dataInfoPassenger.DataSource = getDataGridPassenger().Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {

                editPassenger();
                dataInfoPassenger.DataSource = getDataGridPassenger().Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void btnQueryPassenger_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbQueryPassenger.Text.StartsWith("Xóa thông tin của khách hàng có ID_HK là 'KH002'") || cbQueryPassenger.Text.StartsWith("Cập nhật thông tin của khách hàng có ID_HK là 'KH001'"))
                {
                    dataInfoPassenger.DataSource = getDataGridPassenger().Tables[0];
                }
                else
                    dataQueryPassenger.DataSource = getQueryPassenger().Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        private void showInfoStaff(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = dataInfoStaff.CurrentRow.Index;
            txtIDStaff.Text = dataInfoStaff.Rows[i].Cells[0].Value.ToString();
            txtHoStaff.Text = dataInfoStaff.Rows[i].Cells[1].Value.ToString();
            txtNameStaff.Text = dataInfoStaff.Rows[i].Cells[2].Value.ToString();
            cbSexStaff.Text = dataInfoStaff.Rows[i].Cells[3].Value.ToString();
            dateTimeBirthStaff.Text = dataInfoStaff.Rows[i].Cells[4].Value.ToString();
            txtCMNDStaff.Text = dataInfoStaff.Rows[i].Cells[5].Value.ToString();
            txtAdressStaff.Text = dataInfoStaff.Rows[i].Cells[6].Value.ToString();
            txtPhoneStaff.Text = dataInfoStaff.Rows[i].Cells[7].Value.ToString();
            txtEmailStaff.Text = dataInfoStaff.Rows[i].Cells[8].Value.ToString();
            txtCVStaff.Text = dataInfoStaff.Rows[i].Cells[9].Value.ToString();
            dateTimeNgayThue.Text = dataInfoStaff.Rows[i].Cells[10].Value.ToString();
            txtSalaryStaff.Text = dataInfoStaff.Rows[i].Cells[11].Value.ToString();
        }
        void addStaff()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO NHANVIEN(ID_NV, HO, TEN, GIOITINH, NGAYSINH, CMND, DIACHI, DIENTHOAI, EMAIL, CHUCVU, NGAYTHUE, LUONG) VALUES (@ID_NV, @HO, @TEN, @GIOITINH, @NGAYSINH, @CMND, @DIACHI, @DIENTHOAI, @EMAIL, @CHUCVU, @NGAYTHUE, @LUONG)", connection))
                {
                    command.Parameters.Add(new SqlParameter("@ID_NV", SqlDbType.VarChar, 5)).Value = txtIDStaff.Text;
                    command.Parameters.Add(new SqlParameter("@HO", SqlDbType.NVarChar, 20)).Value = txtHoStaff.Text;
                    command.Parameters.Add(new SqlParameter("@TEN", SqlDbType.NVarChar, 20)).Value = txtNameStaff.Text;
                    command.Parameters.Add(new SqlParameter("@GIOITINH", SqlDbType.NVarChar, 5)).Value = cbSexStaff.Text;
                    command.Parameters.Add(new SqlParameter("@NGAYSINH", SqlDbType.Date)).Value = dateTimeBirthStaff.Text;
                    command.Parameters.Add(new SqlParameter("@CMND", SqlDbType.VarChar, 10)).Value = txtCMNDStaff.Text;
                    command.Parameters.Add(new SqlParameter("@DIACHI", SqlDbType.NVarChar, 100)).Value = txtAdressStaff.Text;
                    command.Parameters.Add(new SqlParameter("@DIENTHOAI", SqlDbType.VarChar, 20)).Value = txtPhoneStaff.Text;
                    command.Parameters.Add(new SqlParameter("@EMAIL", SqlDbType.NVarChar, 100)).Value = txtEmailStaff.Text;
                    command.Parameters.Add(new SqlParameter("@CHUCVU", SqlDbType.NVarChar, 50)).Value = txtCVStaff.Text;
                    command.Parameters.Add(new SqlParameter("@NGAYTHUE", SqlDbType.Date)).Value = dateTimeNgayThue.Text;
                    command.Parameters.Add(new SqlParameter("@LUONG", SqlDbType.Decimal)).Value = txtSalaryStaff.Text;

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    // Check rowsAffected to see if the insert was successful.
                }
            }
        }
        void editStaff()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("UPDATE NHANVIEN SET HO = @HO, TEN = @TEN, GIOITINH = @GIOITINH, NGAYSINH = @NGAYSINH, CMND = @CMND, DIACHI = @DIACHI, DIENTHOAI = @DIENTHOAI, EMAIL = @EMAIL, CHUCVU = @CHUCVU, NGAYTHUE = @NGAYTHUE, LUONG = @LUONG WHERE ID_NV = @ID_NV", connection))
                {
                    command.Parameters.Add(new SqlParameter("@ID_NV", SqlDbType.VarChar, 5)).Value = txtIDStaff.Text;
                    command.Parameters.Add(new SqlParameter("@HO", SqlDbType.NVarChar, 20)).Value = txtHoStaff.Text;
                    command.Parameters.Add(new SqlParameter("@TEN", SqlDbType.NVarChar, 20)).Value = txtNameStaff.Text;
                    command.Parameters.Add(new SqlParameter("@GIOITINH", SqlDbType.NVarChar, 5)).Value = cbSexStaff.Text;
                    command.Parameters.Add(new SqlParameter("@NGAYSINH", SqlDbType.Date)).Value = dateTimeBirthStaff.Text;
                    command.Parameters.Add(new SqlParameter("@CMND", SqlDbType.VarChar, 10)).Value = txtCMNDStaff.Text;
                    command.Parameters.Add(new SqlParameter("@DIACHI", SqlDbType.NVarChar, 100)).Value = txtAdressStaff.Text;
                    command.Parameters.Add(new SqlParameter("@DIENTHOAI", SqlDbType.VarChar, 20)).Value = txtPhoneStaff.Text;
                    command.Parameters.Add(new SqlParameter("@EMAIL", SqlDbType.NVarChar, 100)).Value = txtEmailStaff.Text;
                    command.Parameters.Add(new SqlParameter("@CHUCVU", SqlDbType.NVarChar, 50)).Value = txtCVStaff.Text;
                    command.Parameters.Add(new SqlParameter("@NGAYTHUE", SqlDbType.Date)).Value = dateTimeNgayThue.Text;
                    command.Parameters.Add(new SqlParameter("@LUONG", SqlDbType.Decimal)).Value = txtSalaryStaff.Text;

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    // Check rowsAffected to see if the insert was successful.
                }
            }
        }
        private void btnAddStaff_Click(object sender, EventArgs e)
        {
            try
            {
                addStaff();
                dataInfoStaff.DataSource = getDataGridStaff().Tables[0];
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnEditStaff_Click(object sender, EventArgs e)
        {
            try
            {
                editStaff();
                dataInfoStaff.DataSource = getDataGridStaff().Tables[0];
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
