using Group17_FruitStoreApp.Class;
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

namespace Group17_FruitStoreApp
{
    public partial class FormTrangChu : Form
    {
        public FormTrangChu()
        {
            InitializeComponent();
            SetupSearchTextBox();
            SetupSearchNCCTextBox();
            this.dgvHoaDon.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHoaDon_CellDoubleClick);
        }


        private void FormTrangChu_Load(object sender, EventArgs e)
        {
            Class.FunctionGeneral.MoKetNoi();
            FunctionGeneral.FillCombo("SELECT KH_ID from KhachHang", cbCus_id, "KH_ID", "KH_Ten");
            cbCus_id.SelectedIndex = -1;

            LoadDataKhachHangGridView();
            LoadDataProGridView();
            LoadDataNhaCCGridView();
            InitializeComboBox();
            LoadHoaDonData();
            InitializeComboBoxes();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormTrangChu_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult rs = MessageBox.Show("Bạn muốn thực sự muốn thoát?", "Cảnh báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (rs == DialogResult.OK)
            {
                Class.FunctionGeneral.DongKetNoi(); //commment
            }
            else
            {
                e.Cancel = true;
            }
        }
   
        DataTable tblCUS; //Chứa dữ liệu bảng Customer
        DataTable tblCTHDB; //Chứa dữ liệu bảng Bill
        //Hiển thị các bản ghi khách hàngf
        private void LoadDataKhachHangGridView()
        {
            string sql;
            sql = "SELECT * FROM KhachHang";//Câu lệnh sql hiển thị danh sách khách hàng từ DB
            tblCUS = Class.FunctionGeneral.GetDataToTable(sql); //Đọc dữ liệu từ bảng
            dgvKH.DataSource = tblCUS; //Nguồn dữ liệu
            SetupDataKhachHangGridView();
            dgvKH.AllowUserToAddRows = false;    //Không cho người dùng thêm dữ liệu trực tiếp
            dgvKH.EditMode = DataGridViewEditMode.EditProgrammatically; //Không cho sửa dữ liệu trực tiếp
            //dgvKH.CurrentCell.Selected = false;
        }

        //Hàm để chỉnh lại tên và độ rộng của mỗi cột
        private void SetupDataKhachHangGridView()
        {
            dgvKH.Columns["KH_ID"].HeaderText = "Mã khách";
            dgvKH.Columns["KH_Ten"].HeaderText = "Tên khách";
            dgvKH.Columns["KH_DiaChi"].HeaderText = "Địa chỉ";
            dgvKH.Columns["KH_SDT"].HeaderText = "Điện thoại";
            dgvKH.Columns["KH_ID"].Width = 255;
            dgvKH.Columns["KH_Ten"].Width = 297;
            dgvKH.Columns["KH_DiaChi"].Width = 300;
            dgvKH.Columns["KH_SDT"].Width = 255;
        }

        //Hàm này để lấy dữ liệu bản ghi lên text
        private void dgvKH_Click(object sender, EventArgs e)
        {
            txt_kh_ma.Enabled = false;
            if (tblCUS.Rows.Count == 0) //Nếu không có dữ liệu
            {
                MessageBox.Show("Không có dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            txt_kh_ma.Text = dgvKH.CurrentRow.Cells["KH_ID"].Value.ToString();
            txt_kh_hoten.Text = dgvKH.CurrentRow.Cells["KH_Ten"].Value.ToString();
            txt_kh_diachi.Text = dgvKH.CurrentRow.Cells["KH_DiaChi"].Value.ToString();
            txt_kh_sdt.Text = dgvKH.CurrentRow.Cells["KH_SDT"].Value.ToString();
        }
        //Thiết lập gợi ý tìm kiếm cho trường txt_kh_timkiem
        private void SetupSearchTextBox()
        {
            txt_kh_timkiem.ForeColor = Color.LightGray;
            txt_kh_timkiem.Text = "Vui lòng nhập tên";
            txt_kh_timkiem.Leave += new System.EventHandler(this.txt_kh_timkiem_Leave);
            txt_kh_timkiem.Enter += new System.EventHandler(this.txt_kh_timkiem_Enter);
        }
        private void txt_kh_timkiem_Leave(object sender, EventArgs e)
        {
            if (txt_kh_timkiem.Text == "")
            {
                txt_kh_timkiem.Text = "Vui lòng nhập tên";
                txt_kh_timkiem.ForeColor = Color.Gray;
            }
        }

        private void txt_kh_timkiem_Enter(object sender, EventArgs e)
        {
            if (txt_kh_timkiem.Text == "Vui lòng nhập tên")
            {
                txt_kh_timkiem.Text = "";
                txt_kh_timkiem.ForeColor = Color.Black;
            }
        }

        //reset lại các text
        private void ResetValues()
        {
            txt_kh_ma.Text = "";
            txt_kh_hoten.Text = "";
            txt_kh_diachi.Text = "";
            txt_kh_sdt.Text = "";
            SetupSearchTextBox();
        }
        //Hàm thêm thông tin khách hàng
        private void btn_kh_them_Click(object sender, EventArgs e)
        {
            string sql;

            if (txt_kh_ma.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập mã khách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_kh_ma.Focus();
                return;
            }
            if (txt_kh_hoten.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập tên khách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_kh_hoten.Focus();
                return;
            }
            if (txt_kh_diachi.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập địa chỉ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_kh_diachi.Focus();
                return;
            }
            if (txt_kh_sdt.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập điện thoại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_kh_sdt.Focus();
                return;
            }
            try
            {
                //Kiểm tra đã tồn tại mã khách chưa
                sql = "SELECT KH_ID FROM KhachHang WHERE KH_ID=N'" + txt_kh_ma.Text.Trim() + "'";
                if (Class.FunctionGeneral.CheckKey(sql))
                {
                    MessageBox.Show("Mã khách này đã tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txt_kh_ma.Focus();
                    return;
                }
                //Gọi câu lệnh sql để thêm dữ liệu
                sql = "INSERT INTO KhachHang VALUES (N'" + txt_kh_ma.Text.Trim() +
                    "',N'" + txt_kh_hoten.Text.Trim() + "',N'" + txt_kh_diachi.Text.Trim() + "','" + txt_kh_sdt.Text.Trim() + "')";
                Class.FunctionGeneral.RunSQL(sql);
                LoadDataKhachHangGridView();
                ResetValues();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thành công: ", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Hàm sửa thông tin khách hàng
        private void btn_kh_sua_Click(object sender, EventArgs e)
        {
            string sql;
            if (tblCUS.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txt_kh_ma.Text == "")
            {
                MessageBox.Show("Bạn phải chọn bản ghi cần sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txt_kh_hoten.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập tên khách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_kh_hoten.Focus();
                return;
            }
            if (txt_kh_diachi.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập địa chỉ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_kh_diachi.Focus();
                return;
            }
            if (txt_kh_sdt.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập điện thoại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_kh_sdt.Focus();
                return;
            }
            try
            {
                sql = "UPDATE KhachHang SET KH_Ten=N'" + txt_kh_hoten.Text.Trim().ToString() + "',KH_DiaChi=N'" +
                txt_kh_diachi.Text.Trim().ToString() + "',KH_SDT='" + txt_kh_sdt.Text.Trim().ToString() +
                "' WHERE KH_ID=N'" + txt_kh_ma.Text + "'";
                Class.FunctionGeneral.RunSQL(sql);
                LoadDataKhachHangGridView();
                ResetValues();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thành công: ", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Hàm xóa thông tin khách hàng
        private void btn_kh_xoa_Click(object sender, EventArgs e)
        {
            string sql;
            if (tblCUS.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txt_kh_ma.Text.Trim() == "")
            {
                MessageBox.Show("Bạn chưa chọn bản ghi hoặc nhập mã cần xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Bạn có muốn xoá bản ghi này không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    sql = "DELETE KhachHang WHERE KH_ID=N'" + txt_kh_ma.Text + "'";
                    Class.FunctionGeneral.RunSQL(sql);
                    LoadDataKhachHangGridView();
                    ResetValues();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thành công: ", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Hàm này dùng để reset kết thúc toàn bộ các trường
        private void btn_kh_boqua_Click(object sender, EventArgs e)
        {
            ResetValues();
            LoadDataKhachHangGridView();
            txt_kh_ma.Enabled = true;
        }

        //Hàm tìm kiếm theo tên
        private void btn_kh_tk_Click(object sender, EventArgs e)
        {

            if (txt_kh_timkiem.Text == "")
            {

                MessageBox.Show("Chưa nhập dữ liệu tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                string query = "select * from KhachHang where KH_Ten like N'%" + txt_kh_timkiem.Text + "%'";
                dgvKH.DataSource = Class.FunctionGeneral.GetDataToTable(query);
            }
        }

        //Hàm này để đóng chương trình
        private void btn_kh_dong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Hàm này dùng 'Enter' để di chuyển sang các text giống như nút 'Tab'
        private void txt_kh_ma_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        //Hàm này dùng 'Enter' để di chuyển sang các text giống như nút 'Tab'
        private void txt_kh_hoten_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        //Hàm này dùng 'Enter' để di chuyển sang các text giống như nút 'Tab'
        private void txt_kh_diachi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }


        //Thanh Tâm start
        DataTable tblPro;


        //Hiển thị dữ liệu từ database lên datagrid view.
        private void LoadDataProGridView()
        {
            string sql;
            sql = "SELECT * FROM HangHoa";

            tblPro = Class.FunctionGeneral.GetDataToTable(sql); //Đọc dữ liệu từ bảng
            dgvHH.DataSource = tblPro; //Nguồn dữ liệu            
            dgvHH.AllowUserToAddRows = false; //Không cho người dùng thêm dữ liệu trực tiếp
            dgvHH.EditMode = DataGridViewEditMode.EditProgrammatically; //Không cho sửa dữ liệu trực tiếp
            SetupDataPRoGridView();
        }

        //Chức năng reset
        private void ResetValuesPro()
        {
            txt_HH_id.Enabled = true;
            txt_HH_id.Text = "";
            txt_HH_SP.Text = "";
            txt_HH_giaBan.Text = "";
            txt_HH_tonKho.Text = "";
            cbo_HH_idNCC.SelectedIndex = -1; // Reset combobox nhà cung cấp
            txt_HH_ngayNhap.Text = "";
            txt_HH_giaNhap.Text = "";
            txt_HH_SLNhap.Text = "";
            txt_HH_dvTinh.Text = "";
            cbo_HH_idNCC.Text = "";
            SetupSearchProTextBox();
        }

        //đóng giao diện
        private void btn_hangHoa_dong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //data đổ vào combobox
        private void cbo_HH_idNCC_Click(object sender, EventArgs e)
        {
            FunctionGeneral.FillCombo("SELECT * FROM NhaCungCap", cbo_HH_idNCC, "NCC_ID", "NCC_Ten");
        }

        //hiển thị hướng dẫn người dùng nhập theo định dạng ngày
        private void txt_HH_ngayNhap_Enter(object sender, EventArgs e)
        {
            if (txt_HH_ngayNhap.Text == "DD/MM/YYYY")
            {
                txt_HH_ngayNhap.Text = "";
                txt_HH_ngayNhap.ForeColor = Color.Black;
            }
        }

        private void txt_HH_ngayNhap_Leave(object sender, EventArgs e)
        {
            if (txt_HH_ngayNhap.Text == "")
            {
                txt_HH_ngayNhap.Text = "DD/MM/YYYY";
                txt_HH_ngayNhap.ForeColor = Color.Gray;
            }
        }

        //Hiển thị hướng dẫn người dùng tìm kiếm
        private void txt_HH_timKiem_Enter(object sender, EventArgs e)
        {
            if (txt_HH_timKiem.Text == "Vui lòng nhập tên")
            {
                txt_HH_timKiem.Text = "";
                txt_HH_timKiem.ForeColor = Color.Black;
            }
        }

        private void txt_HH_timKiem_Leave(object sender, EventArgs e)
        {
            if (txt_HH_timKiem.Text == "")
            {
                txt_HH_timKiem.Text = "Vui lòng nhập tên";
                txt_HH_timKiem.ForeColor = Color.Gray;
            }
        }

        //Thiết lập định dạng giúp người dùng
        private void SetupSearchProTextBox()
        {
            txt_HH_timKiem.ForeColor = Color.LightGray;
            txt_HH_timKiem.Text = "Vui lòng nhập tên";
            txt_HH_timKiem.Leave += new System.EventHandler(this.txt_HH_timKiem_Leave);
            txt_HH_timKiem.Enter += new System.EventHandler(this.txt_HH_timKiem_Enter);

            txt_HH_ngayNhap.ForeColor = Color.LightGray;
            txt_HH_ngayNhap.Text = "DD/MM/YYYY";
            txt_HH_ngayNhap.Leave += new System.EventHandler(this.txt_HH_ngayNhap_Leave);
            txt_HH_ngayNhap.Enter += new System.EventHandler(this.txt_HH_ngayNhap_Enter);
        }

        //chức năng tìm kiếm
        private void btn_hangHoa_timKiem_Click(object sender, EventArgs e)
        {
            if (txt_HH_timKiem.Text == "")
            {

                MessageBox.Show("Chưa nhập dữ liệu tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                string query = "select * from HangHoa where HH_Ten like N'%" + txt_HH_timKiem.Text + "%'";
                dgvHH.DataSource = Class.FunctionGeneral.GetDataToTable(query);
            }
        }

        private void btn_HH_xoa_Click(object sender, EventArgs e)
        {
            string sql;
            if (tblPro.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txt_HH_id.Text.Trim() == "")
            {
                MessageBox.Show("Bạn chưa chọn bản ghi hoặc nhập ID cần xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Bạn có muốn xoá bản ghi này không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    sql = "DELETE FROM HangHoa WHERE HH_ID=N'" + txt_HH_id.Text + "'";
                    Class.FunctionGeneral.RunSQL(sql);
                    LoadDataProGridView();
                    ResetValuesPro();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thành công: " + ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_HH_boQua_Click(object sender, EventArgs e)
        {
            ResetValuesPro();
        }

        private void btn_HH_sua_Click(object sender, EventArgs e)
        {
            // Kiểm tra liệu có dữ liệu trong bảng không
            if (tblPro.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(txt_HH_id.Text))
            {
                MessageBox.Show("Bạn phải chọn bản ghi cần sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrEmpty(txt_HH_SP.Text.Trim()))
            {
                MessageBox.Show("Bạn phải nhập tên sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_SP.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txt_HH_giaBan.Text.Trim()))
            {
                MessageBox.Show("Bạn phải nhập giá bán", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_giaBan.Focus();
                return;
            }
            // Kiểm tra xem giá bán có phải là số hợp lệ
            if (!decimal.TryParse(txt_HH_giaBan.Text, out decimal giaBan) || giaBan <= 0)
            {
                MessageBox.Show("Giá bán phải là một số hợp lệ và lớn hơn 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_HH_giaBan.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txt_HH_tonKho.Text.Trim()))
            {
                MessageBox.Show("Bạn phải nhập số lượng tồn kho", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_tonKho.Focus();
                return;
            }

            if (cbo_HH_idNCC.SelectedIndex == -1) // Kiểm tra xem có một mục nào được chọn trong ComboBox không
            {
                MessageBox.Show("Bạn phải chọn nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cbo_HH_idNCC.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txt_HH_ngayNhap.Text.Trim()))
            {
                MessageBox.Show("Bạn phải nhập ngày tháng năm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_ngayNhap.Focus();
                return;
            }

            // kiểm tra định dạng ngày
            DateTime ngayNhap;
            bool isDateValid = DateTime.TryParse(txt_HH_ngayNhap.Text, out ngayNhap);
            if (!isDateValid)
            {
                MessageBox.Show("Định dạng ngày không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_HH_ngayNhap.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txt_HH_giaNhap.Text.Trim()))
            {
                MessageBox.Show("Bạn phải nhập giá nhập", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_giaNhap.Focus();
                return;
            }
            // Kiểm tra xem giá nhập có phải là số hợp lệ
            if (!decimal.TryParse(txt_HH_giaNhap.Text, out decimal giaNhap) || giaNhap < 0)
            {
                MessageBox.Show("Giá nhập phải là một số hợp lệ và không âm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_HH_giaNhap.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txt_HH_SLNhap.Text.Trim()))
            {
                MessageBox.Show("Bạn phải nhập số lượng nhập", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_SLNhap.Focus();
                return;
            }

            // Kiểm tra xem số lượng nhập có phải là số nguyên hợp lệ và lớn hơn 0
            if (!int.TryParse(txt_HH_SLNhap.Text, out int soLuongNhap) || soLuongNhap <= 0)
            {
                MessageBox.Show("Số lượng nhập phải là một số nguyên hợp lệ và lớn hơn 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_HH_SLNhap.Focus();
                return;
            }


            if (string.IsNullOrEmpty(txt_HH_dvTinh.Text.Trim()))
            {
                MessageBox.Show("Bạn phải nhập đơn vị tính", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_dvTinh.Focus();
                return;
            }

            // Cập nhật thông tin hàng hóa vào cơ sở dữ liệu
            try
            {
                string sql = "UPDATE HangHoa SET HH_TEN=N'" + txt_HH_SP.Text.Trim() + "'," +
                 "HH_GiaBan=" + txt_HH_giaBan.Text + "," +
                 "HH_SLTonKho=" + txt_HH_tonKho.Text + "," +
                 "NCC_ID=N'" + cbo_HH_idNCC.SelectedValue.ToString() + "'," +
                 "HH_NgayNhap='" + FunctionGeneral.ConvertDateTime(txt_HH_ngayNhap.Text.Trim()) + "'," +
                 "HH_GiaNhap=" + txt_HH_giaNhap.Text + "," +
                 "HH_SLNhap=" + txt_HH_SLNhap.Text + "," +
                 "HH_DonViTinh=N'" + txt_HH_dvTinh.Text.Trim() + "'" +
                 "WHERE HH_ID=N'" + txt_HH_id.Text + "'";

                Class.FunctionGeneral.RunSQL(sql);
                LoadDataProGridView(); // Cập nhật lại DataGridView để hiển thị thông tin mới
                ResetValuesPro(); // Xóa các giá trị đã nhập
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cập nhật không thành công: " + ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_HH_them_Click(object sender, EventArgs e)
        {
            string sql;

            // Kiểm tra nhập liệu mã hàng hóa
            if (txt_HH_id.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập id hàng hóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_id.Focus();
                return;
            }

            // Kiểm tra nhập liệu tên hàng hóa
            if (txt_HH_SP.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập tên sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_SP.Focus();
                return;
            }

            // Kiểm tra nhập liệu giá bán và giá nhập
            if (!decimal.TryParse(txt_HH_giaBan.Text.Trim(), out decimal giaBan) || giaBan <= 0)
            {
                MessageBox.Show("Giá bán phải là một số hợp lệ và lớn hơn 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_giaBan.Focus();
                return;
            }

            if (!decimal.TryParse(txt_HH_giaNhap.Text.Trim(), out decimal giaNhap) || giaNhap < 0)
            {
                MessageBox.Show("Giá nhập phải là một số hợp lệ và không âm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_giaNhap.Focus();
                return;
            }

            // Kiểm tra nhập liệu số lượng nhập và tồn kho
            if (!int.TryParse(txt_HH_SLNhap.Text.Trim(), out int soLuongNhap) || soLuongNhap <= 0)
            {
                MessageBox.Show("Số lượng nhập phải là một số nguyên hợp lệ và lớn hơn 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_SLNhap.Focus();
                return;
            }

            if (!int.TryParse(txt_HH_tonKho.Text.Trim(), out int tonKho) || tonKho < 0)
            {
                MessageBox.Show("Số lượng tồn kho phải là một số nguyên hợp lệ và không âm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_tonKho.Focus();
                return;
            }

            // Kiểm tra nhập liệu nhà cung cấp
            if (cbo_HH_idNCC.SelectedIndex == -1)
            {
                MessageBox.Show("Bạn phải chọn nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cbo_HH_idNCC.Focus();
                return;
            }

            // Kiểm tra định dạng ngày nhập
            if (!DateTime.TryParse(txt_HH_ngayNhap.Text.Trim(), out DateTime ngayNhap))
            {
                MessageBox.Show("Định dạng ngày không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_ngayNhap.Focus();
                return;
            }

            // Kiểm tra nhập liệu đơn vị tính
            if (string.IsNullOrEmpty(txt_HH_dvTinh.Text.Trim()))
            {
                MessageBox.Show("Bạn phải nhập đơn vị tính", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_HH_dvTinh.Focus();
                return;
            }

            try
            {
                // Kiểm tra đã tồn tại mã hàng hóa chưa
                sql = "SELECT HH_ID FROM HangHoa WHERE HH_ID=N'" + txt_HH_id.Text.Trim() + "'";
                if (Class.FunctionGeneral.CheckKey(sql))
                {
                    MessageBox.Show("ID hàng hóa này đã tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txt_HH_id.Focus();
                    return;
                }

                // Câu lệnh SQL để thêm dữ liệu hàng hóa mới
                sql = "INSERT INTO HangHoa(HH_ID, HH_Ten, HH_NgayNhap, HH_GiaNhap, HH_SLNhap, HH_SLTonKho, HH_GiaBan, HH_DonViTinh, NCC_ID) " +
                      "VALUES (N'" + txt_HH_id.Text.Trim() + "', N'" + txt_HH_SP.Text.Trim() +
                      "', '" + ngayNhap.ToString("yyyy-MM-dd") + "', " + giaNhap +
                      ", " + soLuongNhap + ", " + tonKho + ", " + giaBan +
                      ", N'" + txt_HH_dvTinh.Text.Trim() + "', N'" + cbo_HH_idNCC.SelectedValue.ToString() + "')";

                Class.FunctionGeneral.RunSQL(sql);
                LoadDataProGridView();
                ResetValuesPro();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thêm được: " + ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvHH_Click(object sender, EventArgs e)
        {
            txt_HH_id.Enabled = false;
            if (dgvHH.SelectedRows.Count > 0) // Kiểm tra xem có hàng nào được chọn
            {
                int i = dgvHH.SelectedRows[0].Index;

                // Cập nhật TextBox cho ID Hàng Hóa
                txt_HH_id.Text = dgvHH.Rows[i].Cells[0].Value.ToString(); // Thay '0' bằng chỉ số cột đúng

                // Cập nhật TextBox cho Tên Sản Phẩm
                txt_HH_SP.Text = dgvHH.Rows[i].Cells[1].Value.ToString(); // Thay '1' bằng chỉ số cột đúng

                // Cập nhật TextBox cho Giá Bán
                txt_HH_giaBan.Text = dgvHH.Rows[i].Cells[6].Value.ToString(); // Thay '2' bằng chỉ số cột đúng

                // Cập nhật TextBox cho Số Lượng Tồn Kho
                txt_HH_tonKho.Text = dgvHH.Rows[i].Cells[5].Value.ToString(); // Thay '3' bằng chỉ số cột đúng

                string NhaCungCap;
                string sql;
                NhaCungCap = dgvHH.Rows[i].Cells[8].Value.ToString();
                sql = "SELECT NCC_Ten, NCC_ID FROM NhaCungCap WHERE NCC_ID=N'" + NhaCungCap + "'";
                cbo_HH_idNCC.Text = FunctionGeneral.GetFieldValues(sql);
                //cbo_HH_idNCC.SelectedValue = dgvHH.Rows[i].Cells[8].Value.ToString(); // Thay '4' bằng chỉ số cột đúng

                // Cập nhật TextBox cho Ngày Nhập
                DateTime ngayNhap = Convert.ToDateTime(dgvHH.Rows[i].Cells[2].Value);
                txt_HH_ngayNhap.Text = ngayNhap.ToShortDateString(); // Lấy phần ngày
                //txt_HH_ngayNhap.Text = dgvHH.Rows[i].Cells[2].Value.ToString(); // Thay '5' bằng chỉ số cột đúng
                txt_HH_ngayNhap.ForeColor = Color.Black; // Đặt màu chữ màu đen khi có dữ liệu

                // Cập nhật TextBox cho Giá Nhập
                txt_HH_giaNhap.Text = dgvHH.Rows[i].Cells[3].Value.ToString(); // Thay '6' bằng chỉ số cột đúng

                // Cập nhật TextBox cho Số Lượng Nhập
                txt_HH_SLNhap.Text = dgvHH.Rows[i].Cells[4].Value.ToString(); // Thay '7' bằng chỉ số cột đúng

                // Cập nhật TextBox cho Đơn Vị Tính
                txt_HH_dvTinh.Text = dgvHH.Rows[i].Cells[7].Value.ToString(); // Thay '8' bằng chỉ số cột đúng
            }
        }

        private void SetupDataPRoGridView()
        {
            dgvHH.Columns["HH_ID"].HeaderText = "ID Hàng Hóa";
            dgvHH.Columns["HH_Ten"].HeaderText = "Tên Hàng Hóa";
            dgvHH.Columns["HH_NgayNhap"].HeaderText = "Ngày Nhập";
            dgvHH.Columns["HH_GiaNhap"].HeaderText = "Giá Nhập";
            dgvHH.Columns["HH_SLNhap"].HeaderText = "Số Lượng Nhập";
            dgvHH.Columns["HH_SLTonKho"].HeaderText = "Số Lượng Tồn Kho";
            dgvHH.Columns["HH_GiaBan"].HeaderText = "Giá Bán";
            dgvHH.Columns["HH_DonViTinh"].HeaderText = "Đơn Vị Tính";
            dgvHH.Columns["NCC_ID"].HeaderText = "ID Nhà Cung Cấp";
            //dgvHH.Columns["KH_ID"].Width = 255;
            //dgvHH.Columns["KH_Ten"].Width = 297;
            //dgvHH.Columns["KH_DiaChi"].Width = 300;
            //dgvHH.Columns["KH_SDT"].Width = 255;
        }

        DataTable tblNCC; //Chứa dữ liệu bảng Nhà cung cấp

        private void LoadDataNhaCCGridView()
        {
            string sql;
            sql = "SELECT * FROM NhaCungCap";//Câu lệnh sql hiển thị danh sách NCC từ DB
            tblNCC = Class.FunctionGeneral.GetDataToTable(sql); //Đọc dữ liệu từ bảng
            dgvNCC.DataSource = tblNCC; //Nguồn dữ liệu
            SetupDataNhaCCGridView();
            dgvNCC.AllowUserToAddRows = false;    //Không cho người dùng thêm dữ liệu trực tiếp
            dgvNCC.EditMode = DataGridViewEditMode.EditProgrammatically; //Không cho sửa dữ liệu trực tiếp
        }

        //Hàm để chỉnh lại tên và độ rộng của mỗi cột
        private void SetupDataNhaCCGridView()
        {
            dgvNCC.Columns["NCC_ID"].HeaderText = "Mã nhà cung cấp";
            dgvNCC.Columns["NCC_Ten"].HeaderText = "Tên nhà cung cấp";
            dgvNCC.Columns["NCC_DiaChi"].HeaderText = "Địa chỉ";
            dgvNCC.Columns["NCC_SDT"].HeaderText = "Điện thoại";
            dgvNCC.Columns["NCC_ID"].Width = 255;
            dgvNCC.Columns["NCC_Ten"].Width = 297;
            dgvNCC.Columns["NCC_DiaChi"].Width = 300;
            dgvNCC.Columns["NCC_SDT"].Width = 255;
        }

        private void dgvNCC_Click(object sender, EventArgs e)
        {
            txt_ncc_ma.Enabled = false;
            if (tblNCC.Rows.Count == 0) //Nếu không có dữ liệu
            {
                MessageBox.Show("Không có dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            txt_ncc_ma.Text = dgvNCC.CurrentRow.Cells["NCC_ID"].Value.ToString();
            txt_ncc_ten.Text = dgvNCC.CurrentRow.Cells["NCC_Ten"].Value.ToString();
            txt_ncc_diachi.Text = dgvNCC.CurrentRow.Cells["NCC_DiaChi"].Value.ToString();
            txt_ncc_sdt.Text = dgvNCC.CurrentRow.Cells["NCC_SDT"].Value.ToString();
        }

        private void SetupSearchNCCTextBox()
        {
            txt_ncc_timkiem.ForeColor = Color.LightGray;
            txt_ncc_timkiem.Text = "Vui lòng nhập tên";
            txt_ncc_timkiem.Leave += new System.EventHandler(this.txt_ncc_timkiem_Leave);
            txt_ncc_timkiem.Enter += new System.EventHandler(this.txt_ncc_timkiem_Enter);
        }

        private void txt_ncc_timkiem_Leave(object sender, EventArgs e)
        {
            if (txt_ncc_timkiem.Text == "")
            {
                txt_ncc_timkiem.Text = "Vui lòng nhập tên";
                txt_ncc_timkiem.ForeColor = Color.Gray;
            }
        }

        private void txt_ncc_timkiem_Enter(object sender, EventArgs e)
        {
            if (txt_ncc_timkiem.Text == "Vui lòng nhập tên")
            {
                txt_ncc_timkiem.Text = "";
                txt_ncc_timkiem.ForeColor = Color.Black;
            }
        }

        private void ResetValuesNCC()
        {
            txt_ncc_ma.Text = "";
            txt_ncc_ten.Text = "";
            txt_ncc_diachi.Text = "";
            txt_ncc_sdt.Text = "";
            SetupSearchNCCTextBox();
        }

        private void btn_ncc_them_Click(object sender, EventArgs e)
        {
            string sql;

            if (txt_ncc_ma.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập mã nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_ncc_ma.Focus();
                return;
            }
            if (txt_ncc_ten.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập tên nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_ncc_ten.Focus();
                return;
            }
            if (txt_ncc_diachi.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập địa chỉ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_ncc_diachi.Focus();
                return;
            }
            if (txt_ncc_sdt.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập điện thoại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_ncc_sdt.Focus();
                return;
            }
            try
            {
                //Kiểm tra đã tồn tại mã nhà cung cấp chưa
                sql = "SELECT NCC_ID FROM NhaCungCap WHERE NCC_ID=N'" + txt_ncc_ma.Text.Trim() + "'";
                if (Class.FunctionGeneral.CheckKey(sql))
                {
                    MessageBox.Show("Mã nhà cung cấp này đã tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txt_ncc_ma.Focus();
                    return;
                }
                //Gọi câu lệnh sql để thêm dữ liệu
                sql = "INSERT INTO NhaCungCap VALUES (N'" + txt_ncc_ma.Text.Trim() +
                    "',N'" + txt_ncc_ten.Text.Trim() + "',N'" + txt_ncc_diachi.Text.Trim() + "','" + txt_ncc_sdt.Text.Trim() + "')";
                Class.FunctionGeneral.RunSQL(sql);
                LoadDataNhaCCGridView();
                ResetValuesNCC();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thành công: ", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_ncc_sua_Click(object sender, EventArgs e)
        {
            string sql;
            if (tblNCC.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txt_ncc_ma.Text == "")
            {
                MessageBox.Show("Bạn phải chọn bản ghi cần sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txt_ncc_ten.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập tên nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_ncc_ten.Focus();
                return;
            }
            if (txt_ncc_diachi.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập địa chỉ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_ncc_diachi.Focus();
                return;
            }
            if (txt_ncc_sdt.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập điện thoại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_ncc_sdt.Focus();
                return;
            }
            try
            {
                sql = "UPDATE NhaCungCap SET NCC_Ten=N'" + txt_ncc_ten.Text.Trim().ToString() + "',NCC_DiaChi=N'" +
                txt_ncc_diachi.Text.Trim().ToString() + "',NCC_SDT='" + txt_ncc_sdt.Text.Trim().ToString() +
                "' WHERE NCC_ID=N'" + txt_ncc_ma.Text + "'";
                Class.FunctionGeneral.RunSQL(sql);
                LoadDataNhaCCGridView();
                ResetValuesNCC();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thành công: ", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_ncc_xoa_Click(object sender, EventArgs e)
        {
            string sql;
            if (tblNCC.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txt_ncc_ma.Text.Trim() == "")
            {
                MessageBox.Show("Bạn chưa chọn bản ghi hoặc nhập mã cần xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Bạn có muốn xoá bản ghi này không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    sql = "DELETE NhaCungCap WHERE NCC_ID=N'" + txt_ncc_ma.Text + "'";
                    Class.FunctionGeneral.RunSQL(sql);
                    LoadDataNhaCCGridView();
                    ResetValuesNCC();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thành công: ", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_ncc_boqua_Click(object sender, EventArgs e)
        {
            ResetValuesNCC();
            LoadDataNhaCCGridView();
            txt_ncc_ma.Enabled = true;
        }

        private void btn_ncc_tk_Click(object sender, EventArgs e)
        {
            if (txt_ncc_timkiem.Text == "")
            {

                MessageBox.Show("Chưa nhập dữ liệu tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                string query = "select * from NhaCungCap where NCC_Ten like N'%" + txt_ncc_timkiem.Text + "%'";
                dgvNCC.DataSource = Class.FunctionGeneral.GetDataToTable(query);
            }
        }

        private void btn_ncc_dong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_ncc_dong_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txt_ncc_ten_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txt_ncc_diachi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }


        private void LoadDataSaleGridView()
        {
            string sql;
            sql = "SELECT HD_ID,HH_Ten,CT_SoLuong, CT_GiaBan,CT_SoTien FROM ChiTietHoaDon WHERE HD_ID = N'" + txtBil_id.Text + "'";
            tblCTHDB = FunctionGeneral.GetDataToTable(sql);
            dgvSale.DataSource = tblCTHDB;
            dgvSale.AllowUserToAddRows = false;
            dgvSale.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        int a = 0; //Khach hang là mới

        private void btnCreate_Sale_Click(object sender, EventArgs e)
        {
             
            a = 0;

            int maxBIL_IDAuto;

            string sql = "select Max(HD_ID) as Max from HoaDon";
            SqlCommand cmd = new SqlCommand(sql, Class.FunctionGeneral.sqlCon);
            SqlDataReader reader = cmd.ExecuteReader();
            //FunctionGeneral.RunSQL(sql);    
            reader.Read();
            maxBIL_IDAuto = Convert.ToInt32(reader.GetValue(0)) + 1;
            txtBil_id.Text = maxBIL_IDAuto.ToString();
            reader.Close();

            LoadDataSaleGridView();




            //Tự động gen ra ngày
            txtBil_date.Text = DateTime.Now.ToString("MM/dd/yyyy");


            FunctionGeneral.FillCombo("SELECT HH_ID FROM HangHoa", cbPro_id, "HH_ID", "HH_ID");
            cbPro_id.SelectedIndex = -1;
            cbCus_id.SelectedIndex = -1;
            txtTongTien.Text = "";
        }

        private void cbPro_id_TextChanged(object sender, EventArgs e)
        {
            string str = "SELECT HH_TEN FROM HangHoa WHERE HH_ID =N'" + cbPro_id.SelectedValue + "'";
            txtPro_Name.Text = FunctionGeneral.GetFieldValues(str);

            string str1 = "SELECT HH_GiaBan FROM HangHoa WHERE HH_ID =N'" + cbPro_id.SelectedValue + "'";
            txtDongia.Text = FunctionGeneral.GetFieldValues(str1);

            string str2 = "SELECT HH_DonViTinh FROM HangHoa WHERE HH_ID =N'" + cbPro_id.SelectedValue + "'";
            txtDonViTinh.Text = FunctionGeneral.GetFieldValues(str2);

            string str3 = "SELECT HH_GiaNhap FROM HangHoa WHERE HH_ID =N'" + cbPro_id.SelectedValue + "'";
            txtGiaNhap.Text = FunctionGeneral.GetFieldValues(str3);

            txtSoluong.Text = "";
        }

        private void cbCus_id_TextChanged(object sender, EventArgs e)
        {
            txtCus_id.Text = cbCus_id.Text;
            string str = "SELECT KH_Ten FROM KhachHang WHERE KH_ID=N'" + cbCus_id.SelectedValue + "'";
            txtCus_Name.Text = FunctionGeneral.GetFieldValues(str);

            string str1 = "SELECT KH_DiaChi FROM KhachHang WHERE KH_ID=N'" + cbCus_id.SelectedValue + "'";
            txtCus_Address.Text = FunctionGeneral.GetFieldValues(str1);

            string str2 = "SELECT KH_SDT FROM KhachHang WHERE KH_ID=N'" + cbCus_id.SelectedValue + "'";
            txtCus_Phone.Text = FunctionGeneral.GetFieldValues(str2);

            txtSoluong.Text = "";
        }

        private void txtSoluong_TextChanged(object sender, EventArgs e)
        {
            if (txtSoluong.Text.Trim() == "-")
            {
                MessageBox.Show("Không đc thêm số lượng -");
                txtSoluong.Text = "";
            }
            //Khi thay đổi số lượng thì thực hiện tính lại thành tiền
            double tt, sl, dg, tl, dgn;
            if (txtSoluong.Text == "")
                sl = 0;
            else
                sl = Convert.ToDouble(txtSoluong.Text);
            if (txtDongia.Text == "")
                dg = 0;
            else
                dg = Convert.ToDouble(txtDongia.Text);
            dgn = Convert.ToDouble(txtGiaNhap.Text);
            tt = sl * dg;
            tl = tt - (sl * dgn);
            txtThanhtien.Text = tt.ToString();
            txt_TienLai.Text = tl.ToString();
        }

         int countToTongTien = 0;
        private void btnUpdate_Sale_Click(object sender, EventArgs e)
        {
            string sql;
            Boolean isOk;
            double sl, SLcon, tong, Tongmoi, tongLai, tongLaiMoi;

            //Kiểm tra Mã hóa đơn đã có chưa, nếu chưa có thì bắt đầu nhập
            sql = "SELECT HD_ID FROM HoaDon WHERE HD_ID=N'" + txtBil_id.Text + "'";
            if (!FunctionGeneral.CheckKey(sql))
            {
                if (txtBil_date.Text.Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập ngày bán", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtBil_date.Focus();
                    return;
                }
                if (cbPro_id.Text.Length == 0)
                {
                    MessageBox.Show("Bạn phải nhập khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cbPro_id.Focus();
                    return;
                }

                //Kiểm tra khách hàng  cũ hay mới
                //Nếu là KH mới thì phải cập nhật thông tin KH này vào bảng Customer
                sql = "select KH_ID from KhachHang where KH_ID = N'" + txtCus_id.Text + "'";
                if (FunctionGeneral.CheckKey(sql))
                {
                }
                else
                {
                    //Cập nhật vào bảng Customer
                    sql = "INSERT INTO KhachHang VALUES(N'" +
                    txtCus_id.Text + "',N'" + txtCus_Name.Text + "',N'" + txtCus_Address.Text + "',N'" + txtCus_Phone.Text + "')";
                    isOk = Class.FunctionGeneral.RunBillSQL(sql); //Thực hiện câu lệnh sql
                    LoadDataKhachHangGridView();
                }


            }
            //Kiểm tra các ô textbox đã nhập chưa
            if (cbPro_id.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập mã hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if ((txtSoluong.Text.Trim().Length == 0) || (txtSoluong.Text == "0"))
            {
                MessageBox.Show("Bạn phải nhập số lượng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSoluong.Text = "";
                txtSoluong.Focus();
                return;
            }

            //Kiểm tra mã hàng đã có chưa
            sql = "SELECT HH_Ten FROM ChiTietHoaDon WHERE HH_Ten =N'" + txtPro_Name.Text + "' AND HD_ID = N'" + txtBil_id.Text.Trim() + "'"; if (FunctionGeneral.CheckKey(sql))
            {
                MessageBox.Show("Mã hàng này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cbPro_id.Focus();
                return;
            }
            // Kiểm tra xem số lượng hàng trong kho còn đủ để cung cấp không?
            sl = Convert.ToDouble(FunctionGeneral.GetFieldValues("SELECT HH_SLTonKho FROM HangHoa WHERE HH_ID = N'" + cbPro_id.SelectedValue + "'"));
            if (Convert.ToDouble(txtSoluong.Text) > sl)
            {

                MessageBox.Show("Số lượng mặt hàng này chỉ còn " + sl, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSoluong.Text = "";
                txtSoluong.Focus();
                return;
            }
            //Nếu số lượng đủ, các dl khác ok thì tiến hàng thêm vào hóa đơn
            else
            {
                //Kiem tra xem ma Hàng da co trong hoa don đang tạo chưa?
                sql = "SELECT HH_Ten FROM ChiTietHoaDon WHERE HH_Ten =N'" + txtPro_Name.Text + "' AND HD_ID = N'" + txtBil_id.Text.Trim() + "'";
                if (FunctionGeneral.CheckKey(sql))
                {
                    MessageBox.Show("Mã hàng này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cbPro_id.Focus();
                    return;
                }

                //Nếu chưa có thì tiến hành chèn mặt hàng đó vào hóa đơn
                sql = "SELECT HD_ID FROM HoaDon WHERE HD_ID=N'" + txtBil_id.Text + "'";
                if (FunctionGeneral.CheckKey(sql))
                {

                }
                else
                {
                    sql = "INSERT INTO HoaDon VALUES ('" + txtBil_date.Text.Trim() + "','" +
                          txtThanhtien.Text + "','"
                         + txtCus_id.Text + "','" + txtTongLai.Text + "')";
                }
                isOk = Class.FunctionGeneral.RunBillSQL(sql);
                LoadDataSaleGridView();

                sql = "INSERT INTO ChiTietHoaDon VALUES(N'" + txtBil_id.Text.Trim() + "',N'" + txtDonViTinh.Text + "',N'" + txtSoluong.Text + "',N'" + txtDongia.Text + "',N'" + txtThanhtien.Text + "',N'" + txtPro_Name.Text + "','" + txt_TienLai.Text + "')";
                isOk = Class.FunctionGeneral.RunBillSQL(sql);
                LoadDataSaleGridView();
            }


            // Cập nhật lại số lượng của mặt hàng vào bảng tblHang
            SLcon = sl - Convert.ToDouble(txtSoluong.Text);
            sql = "UPDATE HangHoa SET HH_SLTonKho =" + SLcon + " WHERE HH_ID= N'" + cbPro_id.SelectedValue + "'";
            isOk = Class.FunctionGeneral.RunBillSQL(sql);
            // Cập nhật lại tổng tiền cho hóa đơn bán
            tong = Convert.ToDouble(FunctionGeneral.GetFieldValues("SELECT HD_TongTien FROM HoaDon WHERE HD_ID = N'" + txtBil_id.Text.Trim() + "'"));
            if(countToTongTien == 0)
            {
                Tongmoi = Convert.ToDouble(txtThanhtien.Text); 
            }
            else
            {

                Tongmoi = tong + Convert.ToDouble(txtThanhtien.Text);
            }
            sql = "UPDATE HoaDon SET HD_TongTien =" + Tongmoi + " WHERE HD_ID = N'" + txtBil_id.Text.Trim() + "'";
            isOk = FunctionGeneral.RunBillSQL(sql);
            txtTongTien.Text = Tongmoi.ToString();
            // tong lai
            tongLai = Convert.ToDouble(FunctionGeneral.GetFieldValues("SELECT HD_TongLai FROM HoaDon WHERE HD_ID = N'" + txtBil_id.Text.Trim() + "'"));
            tongLaiMoi = tongLai + Convert.ToDouble(txt_TienLai.Text);
            sql = "UPDATE HoaDon SET HD_TongLai =" + tongLaiMoi + " WHERE HD_ID = N'" + txtBil_id.Text.Trim() + "'";
            isOk = FunctionGeneral.RunBillSQL(sql);
            txtTongLai.Text = tongLaiMoi.ToString();
            //MessageBox.Show(isOk.ToString());
            if (isOk)
            {
                MessageBox.Show("thành công", "Thông báo ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                countToTongTien++;
                LoadHoaDonData();

            }
            else
            {
                MessageBox.Show("Không thành công", "Thông báo ", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnDelete_Sale_Click(object sender, EventArgs e)
        {

        }

        private void cbCus_id_Click(object sender, EventArgs e)
        {
            if (a == 0)
            {
                DialogResult rs = MessageBox.Show("Khách hàng mới?", "Question?", MessageBoxButtons.YesNo);
                if (rs == DialogResult.Yes)
                {
                    cbCus_id.Hide();
                    txtCus_id.Show();
                }
                else
                {
                    a = 1;
                    cbCus_id.Show();
                    txtCus_id.Hide();
                }
            }
            else if (a == 1)
            {

                cbCus_id.Show();
                txtCus_id.Hide();
            }
        }

        //phần chi tiết hoá đơn!

        //Tạo Phương Thức Hiển Thị Dữ Liệu
        private void LoadHoaDonData()
        {
            string sql = "SELECT * FROM HoaDon";
            DataTable dt = Class.FunctionGeneral.GetDataToTable(sql);
            dgvHoaDon.DataSource = dt;
            SetupDataGridViewHeaders();
        }

        // Phương thức để thiết lập tiêu đề cho các cột
        private void SetupDataGridViewHeaders()
        {
            dgvHoaDon.Columns["HD_ID"].HeaderText = "Mã Hóa Đơn";
            dgvHoaDon.Columns["KH_ID"].HeaderText = "Mã Khách Hàng";
            dgvHoaDon.Columns["HD_TGTao"].HeaderText = "Thời gian tạo";
            dgvHoaDon.Columns["HD_TongTien"].HeaderText = "Tổng tiền";
            dgvHoaDon.Columns["HD_TongLai"].HeaderText = "Tổng lãi";
        }

        //Tạo Phương Thức Tìm Kiếm
        private void SearchHoaDon(string maHoaDon, string maKhachHang, string thang, string nam)
        {
            string sql = "SELECT * FROM HoaDon WHERE HD_ID LIKE '%" + maHoaDon + "%' AND KH_ID LIKE '%" + maKhachHang + "%'";

            if (!string.IsNullOrEmpty(thang))
            {
                sql += " AND MONTH(HD_TGTao) = " + thang;
            }

            if (!string.IsNullOrEmpty(nam))
            {
                sql += " AND YEAR(HD_TGTao) = " + nam;
            }

            DataTable dt = Class.FunctionGeneral.GetDataToTable(sql);
            dgvHoaDon.DataSource = dt;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string maHoaDon = txtSearchMaHD.Text;
            string maKhachHang = txtSearchMaKH.Text;
            string thang = cbSearchThang.SelectedIndex > 0 ? cbSearchThang.SelectedItem.ToString() : "";
            string nam = cbSearchNam.SelectedIndex > 0 ? cbSearchNam.SelectedItem.ToString() : "";

            SearchHoaDon(maHoaDon, maKhachHang, thang, nam);
        }

        private void InitializeComboBoxes()
        {
            // Khởi tạo ComboBox Tháng
            cbSearchThang.Items.Clear();
            cbSearchThang.Items.Add("Tất cả");
            for (int i = 1; i <= 12; i++)
            {
                cbSearchThang.Items.Add(i.ToString());
            }
            cbSearchThang.SelectedIndex = 0;

            // Khởi tạo ComboBox Năm
            cbSearchNam.Items.Clear();
            cbSearchNam.Items.Add("Tất cả");
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear - 10; i <= currentYear; i++)
            {
                cbSearchNam.Items.Add(i.ToString());
            }
            cbSearchNam.SelectedIndex = 0;
        }

        //sự kiện khi ấn 2 lần vào bảng
        private void dgvHoaDon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string sql;
            sql = "SELECT HD_ID,HH_Ten,CT_SoLuong, CT_GiaBan,CT_SoTien FROM ChiTietHoaDon WHERE HD_ID = N'" + txtBil_id.Text + "'";
            tblCTHDB = FunctionGeneral.GetDataToTable(sql);
            dgvSale.DataSource = tblCTHDB;
            dgvSale.AllowUserToAddRows = false;
            dgvSale.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void dgvHoaDon_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string maHoaDon = dgvHoaDon.Rows[e.RowIndex].Cells["HD_ID"].Value.ToString();
                FormChiTietHD chiTietForm = new FormChiTietHD(maHoaDon);
                chiTietForm.ShowDialog(); // Hiển thị form chi tiết dưới dạng modal
            }
        }

        //Phần thống kê!

        //phương thức tính tổng doanh thu trong tháng
        private double TinhTongDoanhThuTrongThang(int thang, int nam)
        {
            try
            {
                string sql = $"SELECT SUM(HD_TongTien) FROM HoaDon WHERE MONTH(HD_TGTao) = {thang} AND YEAR(HD_TGTao) = {nam}";
                object result = Class.FunctionGeneral.GetFieldValues(sql);

                if (result != null && result != DBNull.Value && double.TryParse(result.ToString(), out double total))
                {
                    return total;
                }
                return 0.0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tính tổng doanh thu: " + ex.Message);
                return 0.0;
            }
        }

        //Số hoá đơn trong tháng
        private int ThongKeSoHoaDonTrongThang(int thang, int nam)
        {
            try
            {
                string sql = $"SELECT COUNT(*) FROM HoaDon WHERE MONTH(HD_TGTao) = {thang} AND YEAR(HD_TGTao) = {nam}";
                return Convert.ToInt32(Class.FunctionGeneral.GetFieldValues(sql));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thống kê số hóa đơn: " + ex.Message);
                return 0;
            }
        }

        //Mặt hàng bán chạy nhất 
        private string MatHangBanChayNhatTrongThang(int thang, int nam)
        {
            string sql = $"SELECT TOP 1 HH_Ten FROM ChiTietHoaDon JOIN HoaDon ON ChiTietHoaDon.HD_ID = HoaDon.HD_ID WHERE MONTH(HD_TGTao) = {thang} AND YEAR(HD_TGTao) = {nam} GROUP BY HH_Ten ORDER BY SUM(CT_SoLuong) DESC";
            return Class.FunctionGeneral.GetFieldValues(sql);
        }

        //Mặt hàng bán ít nhất tháng
        private string MatHangBanItNhatTrongThang(int thang, int nam)
        {
            string sql = $"SELECT TOP 1 HH_Ten FROM ChiTietHoaDon JOIN HoaDon ON ChiTietHoaDon.HD_ID = HoaDon.HD_ID WHERE MONTH(HD_TGTao) = {thang} AND YEAR(HD_TGTao) = {nam} GROUP BY HH_Ten ORDER BY SUM(CT_SoLuong)";
            return Class.FunctionGeneral.GetFieldValues(sql);
        }

        //Tổng lợi nhuận trong tháng
        private double TinhTongLoiNhuanTrongThang(int thang, int nam)
        {
            try
            {
                string sql = $"SELECT SUM(CT_TienLai) FROM ChiTietHoaDon JOIN HoaDon ON ChiTietHoaDon.HD_ID = HoaDon.HD_ID WHERE MONTH(HD_TGTao) = {thang} AND YEAR(HD_TGTao) = {nam}";
                object result = Class.FunctionGeneral.GetFieldValues(sql);

                // Sử dụng double.TryParse để chuyển đổi kết quả
                double totalProfit;
                if (result != DBNull.Value && double.TryParse(result.ToString(), out totalProfit))
                {
                    return totalProfit;
                }
                return 0.0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tính tổng lợi nhuận: " + ex.Message);
                return 0.0;
            }
        }

        //Khách hàng mua nhiều nhất trong tháng
        private string KhachHangMuaNhieuNhatTrongThang(int thang, int nam)
        {
            string sql = $"SELECT TOP 1 KH_Ten FROM HoaDon JOIN KhachHang ON HoaDon.KH_ID = KhachHang.KH_ID WHERE MONTH(HD_TGTao) = {thang} AND YEAR(HD_TGTao) = {nam} GROUP BY KH_Ten ORDER BY SUM(HD_TongTien) DESC";
            return Class.FunctionGeneral.GetFieldValues(sql);
        }

        //Doanh thu trong năm
        private double TinhTongDoanhThuTrongNam(int nam)
        {
            try
            {
                string sql = $"SELECT SUM(HD_TongTien) FROM HoaDon WHERE YEAR(HD_TGTao) = {nam}";
                object result = Class.FunctionGeneral.GetFieldValues(sql);
                double total;
                if (result != DBNull.Value && double.TryParse(result.ToString(), out total))
                {
                    return total;
                }
                return 0.0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tính tổng doanh thu trong năm: " + ex.Message);
                return 0.0;
            }
        }
        //Lợi nhuận trong năm
        private double TinhTongLoiNhuanTrongNam(int nam)
        {
            try
            {
                string sql = $"SELECT SUM(CT_TienLai) FROM ChiTietHoaDon JOIN HoaDon ON ChiTietHoaDon.HD_ID = HoaDon.HD_ID WHERE YEAR(HD_TGTao) = {nam}";
                object result = Class.FunctionGeneral.GetFieldValues(sql);
                double totalProfit;
                if (result != DBNull.Value && double.TryParse(result.ToString(), out totalProfit))
                {
                    return totalProfit;
                }
                return 0.0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tính tổng lợi nhuận trong năm: " + ex.Message);
                return 0.0;
            }
        }

        //mặt hàng bán chạy nhất năm
        private string MatHangBanChayNhatTrongNam(int nam)
        {
            string sql = $"SELECT TOP 1 HH_Ten FROM ChiTietHoaDon JOIN HoaDon ON ChiTietHoaDon.HD_ID = HoaDon.HD_ID WHERE YEAR(HD_TGTao) = {nam} GROUP BY HH_Ten ORDER BY SUM(CT_SoLuong) DESC";
            return Class.FunctionGeneral.GetFieldValues(sql);
        }

        //mặt hàng bán ít nhất năm
        private string MatHangBanItNhatTrongNam(int nam)
        {
            string sql = $"SELECT TOP 1 HH_Ten FROM ChiTietHoaDon JOIN HoaDon ON ChiTietHoaDon.HD_ID = HoaDon.HD_ID WHERE YEAR(HD_TGTao) = {nam} GROUP BY HH_Ten ORDER BY SUM(CT_SoLuong)";
            return Class.FunctionGeneral.GetFieldValues(sql);
        }

        //khách mua nhiều nhất năm
        private string KhachHangMuaNhieuNhatTrongNam(int nam)
        {
            string sql = $"SELECT TOP 1 KH_Ten FROM HoaDon JOIN KhachHang ON HoaDon.KH_ID = KhachHang.KH_ID WHERE YEAR(HD_TGTao) = {nam} GROUP BY KH_Ten ORDER BY SUM(HD_TongTien) DESC";
            return Class.FunctionGeneral.GetFieldValues(sql);
        }

        // Phương thức thống kê số lượng mặt hàng bán được trong năm
        private void ThongKeSoLuongMatHangTrongNam(string matHang, int nam)
        {
            string sql = $"SELECT SUM(CT_SoLuong) FROM ChiTietHoaDon JOIN HoaDon ON ChiTietHoaDon.HD_ID = HoaDon.HD_ID WHERE HH_Ten = N'{matHang}' AND YEAR(HD_TGTao) = {nam}";
            object result = Class.FunctionGeneral.GetFieldValues(sql);
            int soLuongBanTrongNam = 0;
            if (result != null && result != DBNull.Value)
            {
                int.TryParse(result.ToString(), out soLuongBanTrongNam);
            }

            // Tạo DataTable mới
            DataTable dt = new DataTable();
            dt.Columns.Add("Mặt hàng", typeof(string));
            dt.Columns.Add("Số lượng", typeof(string));

            // Thêm dữ liệu vào DataTable
            dt.Rows.Add(matHang, soLuongBanTrongNam.ToString());

            // Đặt DataTable làm nguồn dữ liệu cho DataGridView
            dgvThongKeNgay.DataSource = dt;
        }

        // Sự kiện khi thay đổi lựa chọn trong ComboBox Tháng hoặc Mặt Hàng
        private void cbThangOrMatHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Xóa dữ liệu hiện tại trong DataGridView
            dgvThongKeNgay.DataSource = null;

            if (cbThang.SelectedItem.ToString() == "Tất cả")
            {
                cbNgay.Enabled = false;
                cbNgay.SelectedIndex = -1;

                if (cbMatHang.SelectedIndex > 0)
                {
                    string matHangDuocChon = cbMatHang.SelectedItem.ToString();
                    int namDuocChon = Convert.ToInt32(cbNam.SelectedItem);
                    ThongKeSoLuongMatHangTrongNam(matHangDuocChon, namDuocChon);
                }
            }
            else
            {
                cbNgay.Enabled = true;

                if (cbNgay.SelectedIndex != -1 && cbMatHang.SelectedIndex != -1)
                {
                    ThucHienThongKeNgayVaMatHang();
                }
            }
        }

        //Hoá đơn trong năm
        private int ThongKeSoHoaDonTrongNam(int nam)
        {
            string sql = $"SELECT COUNT(*) FROM HoaDon WHERE YEAR(HD_TGTao) = {nam}";
            return Convert.ToInt32(Class.FunctionGeneral.GetFieldValues(sql));
        }

        private void InitializeComboBox()
        {
            // Khởi tạo ComboBox cho ngày
            cbNgay.Items.Clear();

            cbNgay.SelectedIndex = -1;

            // Khởi tạo ComboBox cho tháng
            cbThang.Items.Clear();
            cbThang.Items.Add("Tất cả");
            for (int i = 1; i <= 12; i++)
            {
                cbThang.Items.Add(i.ToString());
            }
            cbThang.SelectedIndex = DateTime.Now.Month - 1;

            // Khởi tạo ComboBox cho năm
            cbNam.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear - 10; i <= currentYear; i++)
            {
                cbNam.Items.Add(i);
            }
            cbNam.SelectedItem = currentYear;

            // Khởi tạo ComboBox cho mặt hàng
            cbMatHang.Items.Clear();
            cbMatHang.Items.Add("Chọn...");
            cbMatHang.SelectedIndex = -1;
            string sqlMatHang = "SELECT HH_Ten FROM HangHoa";
            DataTable dtMatHang = FunctionGeneral.GetDataToTable(sqlMatHang);
            foreach (DataRow row in dtMatHang.Rows)
            {
                cbMatHang.Items.Add(row["HH_Ten"].ToString());
            }
            if (cbMatHang.Items.Count > 0) cbMatHang.SelectedIndex = 0;

            // Cập nhật ComboBox ngày dựa trên tháng và năm hiện tại
            UpdateDaysComboBox();
            // Đăng ký sự kiện EnabledChanged
            cbNgay.EnabledChanged += new EventHandler(cbNgay_EnabledChanged);
        }

        private void cbThang_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn trong ComboBox không
            if (cbThang.SelectedIndex != -1 && cbNam.SelectedIndex != -1)
            {
                int selectedMonth;
                int currentYear = Convert.ToInt32(cbNam.SelectedItem.ToString());

                if (cbThang.SelectedItem.ToString() == "Tất cả")
                {
                    // tắt cbx ngày
                    cbNgay.Enabled = false;
                    cbNgay.SelectedIndex = -1;

                    DataTable dt = CreateStatisticsDataTableForAllMonths(currentYear);
                    dgvThongKe.DataSource = dt;
                }
                else if (int.TryParse(cbThang.SelectedItem.ToString(), out selectedMonth))
                {
                    // Thống kê dữ liệu cho tháng cụ thể
                    DataTable dt = CreateStatisticsDataTable();
                    FillStatisticsDataTable(dt, selectedMonth, currentYear);
                    dgvThongKe.DataSource = dt;

                    // Kích hoạt lại và cập nhật ComboBox Ngày
                    cbNgay.Enabled = true;
                    UpdateDaysComboBox();
                }
                // Cập nhật ComboBox ngày
                UpdateDaysComboBox();

            }
            else
            {
                cbThang.SelectedIndex = 0;
            }
        }

        //cập nhật ngày theo tháng
        private void UpdateDaysComboBox()
        {
            cbNgay.Items.Clear();
            cbNgay.Items.Add("Chọn một ngày...");

            int selectedYear = Convert.ToInt32(cbNam.SelectedItem);
            int selectedMonth = cbThang.SelectedIndex;
            // Kiểm tra xem người dùng có chọn "Tất cả" hay không
            if (selectedMonth > 0)
            {
                int daysInMonth = DateTime.DaysInMonth(selectedYear, selectedMonth);
                for (int day = 1; day <= daysInMonth; day++)
                {
                    cbNgay.Items.Add(day);
                }
            }

            cbNgay.SelectedIndex = -1; // Không có lựa chọn nào được chọn mặc định
        }

        //kiểm tra xem cb năm có mục nào được chọn không
        private void cbNam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(cbNam.SelectedItem.ToString(), out int currentYear))
            {
                if (cbThang.SelectedItem.ToString() == "Tất cả")
                {
                    DataTable dt = CreateStatisticsDataTableForAllMonths(currentYear);
                    dgvThongKe.DataSource = dt;
                }
                else if (int.TryParse(cbThang.SelectedItem.ToString(), out int selectedMonth))
                {
                    DataTable dt = CreateStatisticsDataTable();
                    FillStatisticsDataTable(dt, selectedMonth, currentYear);
                    dgvThongKe.DataSource = dt;
                }
            }
        }

        //trong trường hợp chọn tất cả các tháng
        private DataTable CreateStatisticsDataTableForAllMonths(int year)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mô tả", typeof(string));
            dt.Columns.Add("Giá trị", typeof(string));

            // Số Hóa Đơn
            dt.Rows.Add("Số Hóa Đơn", ThongKeSoHoaDonTrongNam(year).ToString());

            // Tổng Doanh Thu
            dt.Rows.Add("Tổng Doanh Thu", TinhTongDoanhThuTrongNam(year).ToString("C"));

            // Tổng Lợi Nhuận
            dt.Rows.Add("Tổng Lợi Nhuận", TinhTongLoiNhuanTrongNam(year).ToString("C"));

            // Mặt hàng bán chạy nhất
            dt.Rows.Add("Mặt hàng bán chạy nhất năm", MatHangBanChayNhatTrongNam(year));

            // Mặt hàng bán ít nhất
            dt.Rows.Add("Mặt hàng bán ít nhất năm", MatHangBanItNhatTrongNam(year));

            // Khách hàng mua nhiều nhất nă
            dt.Rows.Add("Khách hàng mua nhiều nhất", KhachHangMuaNhieuNhatTrongNam(year));

            return dt;
        }

        private DataTable CreateStatisticsDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mô tả", typeof(string));
            dt.Columns.Add("Giá trị", typeof(string));
            return dt;
        }

        private void FillStatisticsDataTable(DataTable dataTable, int month, int year)
        {
            dataTable.Rows.Clear();

            // Số Hóa Đơn
            dataTable.Rows.Add("Số Hóa Đơn", ThongKeSoHoaDonTrongThang(month, year).ToString());

            // Tổng Doanh Thu
            dataTable.Rows.Add("Tổng Doanh Thu", TinhTongDoanhThuTrongThang(month, year).ToString("C"));

            // Mặt Hàng Bán Chạy Nhất
            dataTable.Rows.Add("Mặt Hàng Bán Chạy Nhất", MatHangBanChayNhatTrongThang(month, year));

            // Mặt Hàng Bán Ít Nhất
            dataTable.Rows.Add("Mặt Hàng Bán Ít Nhất", MatHangBanItNhatTrongThang(month, year));

            // Tổng Lợi Nhuận
            dataTable.Rows.Add("Tổng Lợi Nhuận", TinhTongLoiNhuanTrongThang(month, year).ToString("C"));

            // Khách Hàng Mua Nhiều Nhất
            dataTable.Rows.Add("Khách Hàng Mua Nhiều Nhất", KhachHangMuaNhieuNhatTrongThang(month, year));
        }

        private double TinhLoiNhuanTrongNgay(DateTime ngayDuocChon)
        {
            try
            {
                string sql = $"SELECT SUM(CT_TienLai) FROM ChiTietHoaDon JOIN HoaDon ON ChiTietHoaDon.HD_ID = HoaDon.HD_ID WHERE CONVERT(DATE, HD_TGTao) = '{ngayDuocChon.ToString("yyyy-MM-dd")}'";
                object result = Class.FunctionGeneral.GetFieldValues(sql);

                if (result != null && result != DBNull.Value)
                {
                    if (double.TryParse(result.ToString(), out double loiNhuan))
                    {
                        return loiNhuan;
                    }
                }
                return 0.0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tính lợi nhuận trong ngày: " + ex.Message);
                return 0.0;
            }
        }

        private int DemSoHoaDonTrongNgay(DateTime ngayDuocChon)
        {
            try
            {
                string sql = $"SELECT COUNT(*) FROM HoaDon WHERE CONVERT(DATE, HD_TGTao) = '{ngayDuocChon.ToString("yyyy-MM-dd")}'";
                object result = Class.FunctionGeneral.GetFieldValues(sql);

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result
        );
                }
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đếm số hóa đơn trong ngày: " + ex.Message);
                return 0;
            }
        }

        private int DemSoLuongBanTrongNgay(DateTime ngayDuocChon, string maMatHang)
        {
            try
            {
                string sql = $"SELECT SUM(CT_SoLuong) FROM ChiTietHoaDon JOIN HoaDon ON ChiTietHoaDon.HD_ID = HoaDon.HD_ID WHERE HH_Ten = N'{maMatHang}' AND CONVERT(DATE, HD_TGTao) = '{ngayDuocChon.ToString("yyyy-MM-dd")}'";
                object result = Class.FunctionGeneral.GetFieldValues(sql);

                if (result != null && result != DBNull.Value)
                {
                    if (int.TryParse(result.ToString(), out int soLuongBan))
                    {
                        return soLuongBan;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đếm số lượng bán trong ngày: " + ex.Message);
                return 0;
            }
        }

        private DateTime LayNgayDuocChon()
        {
            try
            {
                int ngay = Convert.ToInt32(cbNgay.SelectedItem);
                int thang = Convert.ToInt32(cbThang.SelectedItem);
                int nam = Convert.ToInt32(cbNam.SelectedItem);

                return new DateTime(nam, thang, ngay);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private void ThucHienThongKeNgayVaMatHang()
        {
            if (cbNgay.SelectedIndex != -1 && cbMatHang.SelectedIndex != -1)
            {
                DateTime ngayDuocChon = LayNgayDuocChon();
                string maMatHang = cbMatHang.SelectedItem.ToString();

                double loiNhuanNgay = TinhLoiNhuanTrongNgay(ngayDuocChon);
                int soHoaDonNgay = DemSoHoaDonTrongNgay(ngayDuocChon);
                int soLuongBanNgay = DemSoLuongBanTrongNgay(ngayDuocChon, maMatHang);

                DataTable dt = new DataTable();
                dt.Columns.Add("Mô Tả", typeof(string));
                dt.Columns.Add("Giá Trị", typeof(string));

                dt.Rows.Add("Lợi nhuận trong ngày", loiNhuanNgay.ToString("C"));
                dt.Rows.Add("Số hóa đơn trong ngày", soHoaDonNgay.ToString());
                dt.Rows.Add("Số lượng mặt hàng bán", soLuongBanNgay.ToString());

                dgvThongKeNgay.DataSource = dt;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ngày và mặt hàng trước khi thực hiện thống kê.");
            }
        }

        private void cbNgay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMatHang.SelectedIndex > 0)
            {
                if (cbNgay.SelectedIndex > 0 && cbNgay.Enabled)
                {
                    ThucHienThongKeNgayVaMatHang();
                }
                else if (cbThang.SelectedItem.ToString() == "Tất cả")
                {
                    ThongKeSoLuongMatHangTrongNam(cbMatHang.SelectedItem.ToString(), Convert.ToInt32(cbNam.SelectedItem));
                }
            }
        }

        private void cbMatHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMatHang.SelectedIndex > 0)
            {
                if (cbNgay.SelectedIndex > 0 && cbNgay.Enabled)
                {
                    ThucHienThongKeNgayVaMatHang();
                }
                else if (cbThang.SelectedItem.ToString() == "Tất cả")
                {
                    ThongKeSoLuongMatHangTrongNam(cbMatHang.SelectedItem.ToString(), Convert.ToInt32(cbNam.SelectedItem));
                }
            }
        }

        //xử lý khi combo box ngày đc chọn
        private void cbNgay_EnabledChanged(object sender, EventArgs e)
        {
            if (cbNgay.Enabled)
            {
                // Nếu cbNgay được kích hoạt và cả hai ComboBox đều có mục được chọn
                if (cbNgay.SelectedIndex != -1 && cbMatHang.SelectedIndex != -1)
                {
                    ThucHienThongKeNgayVaMatHang();
                }
            }
            else
            {
                // Nếu cbNgay không được kích hoạt và cbThang được chọn là "Tất cả"
                if (cbThang.SelectedItem.ToString() == "Tất cả" && cbMatHang.SelectedIndex != -1)
                {
                    int namDuocChon = Convert.ToInt32(cbNam.SelectedItem);
                    string matHangDuocChon = cbMatHang.SelectedItem.ToString();
                    ThongKeSoLuongMatHangTrongNam(matHangDuocChon, namDuocChon);
                }
            }
        }
    }
}
