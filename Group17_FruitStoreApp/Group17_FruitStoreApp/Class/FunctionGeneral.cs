using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
namespace Group17_FruitStoreApp.Class
{
    class FunctionGeneral
    {
        public static SqlConnection sqlCon;  //Khai báo đối tượng kết nối        

        public static void MoKetNoi()
        {
            sqlCon = new SqlConnection();   //Khởi tạo đối tượng
            //Gắn chuỗi kết nối
            sqlCon.ConnectionString = @"Data Source=ACERASPIRE-KHAI\SQLEXPRESS;Initial Catalog=Fruit_Storee;Integrated Security=True";
            //Mở chuỗi kết nối
            sqlCon.Open();
            //Kiểm tra kết nối
            if (sqlCon.State == ConnectionState.Open)
            {
                MessageBox.Show("Đã kết nối với dữ liệu");
            }
            else MessageBox.Show("Không thể kết nối với dữ liệu");
        }
        public static void DongKetNoi()
        {
            if (sqlCon.State == ConnectionState.Open)
            {
                sqlCon.Close();   	//Đóng kết nối
                sqlCon.Dispose(); 	//Giải phóng tài nguyên
                sqlCon = null;      // Ngắt chuỗi kết nối
            }
        }

        //Lấy dữ liệu vào bảng
        public static DataTable GetDataToTable(string sql)
        {
            SqlDataAdapter dap = new SqlDataAdapter(sql, sqlCon); //Định nghĩa đối tượng thuộc lớp SqlDataAdapter
            //Khai báo đối tượng table thuộc lớp DataTable
            DataTable table = new DataTable();
            dap.Fill(table); //Đổ kết quả từ câu lệnh sql vào table
            return table;
        }

        //Hàm thực hiện câu lệnh SQL
        public static void RunSQL(string sql)
        {
            SqlCommand cmd; //Đối tượng thuộc lớp SqlCommand
            cmd = new SqlCommand(); //Khởi tạo đối tượng
            cmd.Connection = sqlCon; //Gán kết nối
            cmd.CommandText = sql; //Gán lệnh SQL
            try
            {
                int kq = cmd.ExecuteNonQuery(); //Thực hiện câu lệnh SQL
                if (kq > 0) //Vì executeNonQuery trả về kiểu int, cho biết số dòng được áp dụng.
                {
                    MessageBox.Show("Thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            //Bắt lỗi
            catch (Exception ex)
            {
                MessageBox.Show("Không thành công", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(sql.ToString());
                MessageBox.Show(ex.ToString());
            }
            cmd.Dispose();//Giải phóng bộ nhớ
            //cmd = null;
        }

        //Khải - runsql tạo bill
        public static Boolean RunBillSQL(string sql)
        {
            SqlCommand cmd; //Đối tượng thuộc lớp SqlCommand
            cmd = new SqlCommand(); //Khởi tạo đối tượng
            cmd.Connection = sqlCon; //Gán kết nối
            cmd.CommandText = sql; //Gán lệnh SQL
            Boolean isSuccess = true;
            try
            {
                int kq = cmd.ExecuteNonQuery(); //Thực hiện câu lệnh SQL

            }
            //Bắt lỗi
            catch (Exception ex)
            {
                isSuccess = false;
                MessageBox.Show("Không thành công", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(sql.ToString());
                MessageBox.Show(ex.ToString());
            }
            cmd.Dispose();//Giải phóng bộ nhớ
            //cmd = null;
            //MessageBox.Show(isSuccess.ToString());

            return isSuccess;
        }

        //Hàm thực thi câu lệnh Xóa của SQL
        public static void RunSqlDel(string sql)
        {
            //Khởi tạo đối tượng
            SqlCommand cmd = new SqlCommand();
            //Nối chuỗi kết nối
            cmd.Connection = FunctionGeneral.sqlCon;
            cmd.CommandText = sql;
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Xóa thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dữ liệu đang được dùng, không thể xoá...", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            cmd.Dispose();
            cmd = null;
        }

        //Hàm kiểm tra DL nhập vào có dạng Date hay ko.
        public static bool IsDate(string date)
        {
            //Tách các phần tử bởi dấu /
            string[] elements = date.Split('/');
            //Kiểm tra ngày, tháng năm nhập vào với mức giới hạn 31 ngày 12 tháng và 2000
            if ((Convert.ToInt32(elements[0]) >= 1) && (Convert.ToInt32(elements[0]) <= 31) && (Convert.ToInt32(elements[1]) >= 1) && (Convert.ToInt32(elements[1]) <= 12) && (Convert.ToInt32(elements[2]) >= 2000))
                return true;
            else return false;
        }

        //Hàm chuyển đổi format Date nhập vào để phù hợp với thiết kế csdl
        public static string ConvertDateTime(string date)
        {
            string[] elements = date.Split('/');
            string dt = string.Format("{0}/{1}/{2}", elements[1], elements[0], elements[2]);
            return dt;
        }

        //Hàm fill combobox (với dữ liệu lấy từ SQL)
        public static void FillCombo(string sql, ComboBox cbo, string ma, string ten)
        {
            //Khởi tạo đối tượng
            SqlDataAdapter dap = new SqlDataAdapter(sql, sqlCon);
            DataTable table = new DataTable();
            dap.Fill(table);
            cbo.DataSource = table;
            cbo.ValueMember = ma; //Trường giá trị
            cbo.DisplayMember = ten; //Trường hiển thị
        }

        //Get fieldValue
        public static string GetFieldValues(string sql, List<SqlParameter> parameters = null)
        {
            string ma = "";
            SqlCommand cmd = new SqlCommand(sql, sqlCon);

            // Thêm tham số vào SqlCommand nếu có
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters.ToArray());
            }

            try
            {
                // Mở kết nối nếu nó chưa mở
                if (sqlCon.State != ConnectionState.Open)
                {
                    sqlCon.Open();
                }

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ma = reader.GetValue(0).ToString();
                }
                reader.Close();
            }
            finally
            {
                // Đóng kết nối sau khi sử dụng
                if (sqlCon.State == ConnectionState.Open)
                {
                    //sqlCon.Close();
                }
            }

            return ma;
        }



        //Hàm kiểm tra khoá trùng
        public static bool CheckKey(string sql)
        {
            SqlDataAdapter dap = new SqlDataAdapter(sql, sqlCon);
            DataTable table = new DataTable();
            dap.Fill(table);
            if (table.Rows.Count > 0)
                return true;
            else return false;
        }
    }
}
