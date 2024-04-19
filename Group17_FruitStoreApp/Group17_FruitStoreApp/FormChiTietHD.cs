using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Group17_FruitStoreApp
{
    public partial class FormChiTietHD : Form
    {
        private string maHoaDon;
        public FormChiTietHD(string maHoaDon)
        {
            InitializeComponent();
            this.txtMaHoaDon.Text = maHoaDon;
            this.txtMaHoaDon.ReadOnly = true;

            this.maHoaDon = maHoaDon;
            LoadChiTietHoaDon();
        }
        private void LoadChiTietHoaDon()
        {
            string sql = $"SELECT * FROM ChiTietHoaDon WHERE HD_ID = '{maHoaDon}'";
            DataTable dt = Class.FunctionGeneral.GetDataToTable(sql);
            dgvChiTietHoaDon.DataSource = dt;
            SetupDataGridViewHeaders();
        }

        // Phương thức để thiết lập tiêu đề cho các cột
        private void SetupDataGridViewHeaders()
        {
            dgvChiTietHoaDon.Columns["HD_ID"].HeaderText = "Mã Hóa Đơn";
            dgvChiTietHoaDon.Columns["CT_DonViTinh"].HeaderText = "Đơn vị tính";
            dgvChiTietHoaDon.Columns["CT_SoLuong"].HeaderText = "Số lượng";
            dgvChiTietHoaDon.Columns["CT_GiaBan"].HeaderText = "Giá bán";
            dgvChiTietHoaDon.Columns["CT_SoTien"].HeaderText = "Số tiền";
            dgvChiTietHoaDon.Columns["CT_TienLai"].HeaderText = "Tiền lãi";
            dgvChiTietHoaDon.Columns["HH_Ten"].HeaderText = "Tên hàng";

        }
        private void dgvChiTietHoaDon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtMaHoaDon_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
