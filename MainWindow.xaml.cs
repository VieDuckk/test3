using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LamLaiQuanLyBanHang.Models;

namespace LamLaiQuanLyBanHang
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<SanPham> dsSanPham = new List<SanPham>();
        QLBanHangContext db = new QLBanHangContext();
        private void HienThi()
        {
            var query = from sp in db.SanPhams
                        orderby sp.DonGia
                        select new
                        {
                            sp.MaSp,
                            sp.TenSp,
                            sp.MaLoai,
                            sp.SoLuong,
                            sp.DonGia,
                            ThanhTien = sp.DonGia * sp.SoLuong
                        };
            dtgsanpham.ItemsSource = query.ToList();
        }
        private void HienThiCBB()
        {
            var query = from lsp in db.LoaiSanPhams
                        select lsp;
            cboloaisp.ItemsSource = query.ToList();
            cboloaisp.SelectedValuePath = "MaLoai";
            cboloaisp.DisplayMemberPath = "TenLoai";
            cboloaisp.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HienThi();
            HienThiCBB();
        }

        private void btnthem_Click(object sender, RoutedEventArgs e)
        {
            var query = db.SanPhams.SingleOrDefault(t => t.MaSp.Equals(txtmasp.Text));
            if (query != null)
            {
                MessageBox.Show("Sản phẩm đã tồn tại", "Thông báo");
            }
            else
            {
                if (iskiemtra())
                {
                    SanPham spmoi = new SanPham();
                    spmoi.MaSp = txtmasp.Text;
                    spmoi.TenSp = txttensp.Text;
                    LoaiSanPham item = (LoaiSanPham)cboloaisp.SelectedItem;
                    spmoi.MaLoai = item.MaLoai;
                    spmoi.DonGia = double.Parse(txtdongia.Text);
                    spmoi.SoLuong = int.Parse(txtsoluongco.Text);
                    db.SanPhams.Add(spmoi);
                    db.SaveChanges();
                    MessageBox.Show("Đã thêm thành công", "Thông báo");
                }
                HienThi();
            }
        }
        private bool iskiemtra()
        {
            if (txtmasp.Text == "")
            {
                MessageBox.Show("Vui lòng nhập mã sản phẩm", "Lỗi nhập dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                txtmasp.Focus();
                return false;
            }
            if (txttensp.Text == "")
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm", "Lỗi nhập dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                txttensp.Focus();
                return false;
            }
            try
            {
                int dongia = int.Parse(txtdongia.Text);
                if (dongia <= 0)
                {
                    MessageBox.Show("Đơn giá phải là số nguyên dương lớn hơn 0", "Lỗi nhập dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtdongia.Focus();
                    txtdongia.SelectAll();
                    return false;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Đơn giá phải là số nguyên", "Lỗi nhập dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                txtdongia.Focus();
                txtdongia.SelectAll();
                return false;
            }            
            return true;
        }

        private void btnsua_Click(object sender, RoutedEventArgs e)
        {
            var spsua = db.SanPhams.SingleOrDefault(t => t.MaSp.Equals(txtmasp.Text));
            if (spsua != null)
            {
                spsua.MaSp = txtmasp.Text;
                spsua.TenSp = txttensp.Text;
                LoaiSanPham item = (LoaiSanPham)cboloaisp.SelectedItem;
                spsua.MaLoai = item.MaLoai;
                spsua.DonGia = double.Parse(txtdongia.Text);
                spsua.SoLuong = int.Parse(txtsoluongco.Text);
                db.SaveChanges();
                MessageBox.Show("Đã sửa thành công", "Thông báo");
            }
            else
            {
                MessageBox.Show("Không tìm thấy sản phẩm cần sửa", "Thông báo");
            }
            HienThi();
        }

        private void btnxoa_Click(object sender, RoutedEventArgs e)
        {
            var query = db.SanPhams.SingleOrDefault(t => t.MaSp.Equals(txtmasp.Text));
            if (query == null)
            {
                MessageBox.Show("Không tìm thấy sản phẩm cần xóa", "Thông báo");
            }
            else
            {
                MessageBoxResult rs = MessageBox.Show("Bạn chắc chắn muốn xóa? ", "Thông báo", MessageBoxButton.YesNo);
                if (rs == MessageBoxResult.Yes)
                {
                    db.SanPhams.Remove(query);
                    db.SaveChanges();
                    MessageBox.Show("Xóa thành công", "Thông báo");
                }
            }
            HienThi();
        }

        private void btntim_Click(object sender, RoutedEventArgs e)
        {
            // Tìm đơn giá cao nhất (bỏ qua giá trị null)
            double maxDonGia = db.SanPhams
                .Where(sp => sp.DonGia.HasValue)
                .Max(sp => sp.DonGia.Value);

            // Lấy danh sách sản phẩm có đơn giá cao nhất
            var sanPhamCaoNhat = db.SanPhams
                .Where(sp => sp.DonGia.HasValue && sp.DonGia.Value == maxDonGia)
                .Select(sp => new
                {
                    sp.MaSp,
                    sp.TenSp,
                    sp.MaLoai,
                    sp.SoLuong,
                    DonGia = sp.DonGia.Value, // Chuyển đổi sang giá trị non-null
                    ThanhTien = sp.DonGia.Value * sp.SoLuong
                })
                .ToList();

            // Mở cửa sổ mới và hiển thị danh sách
            Window2 windowThongTin = new Window2();
            windowThongTin.HienThiSanPham(sanPhamCaoNhat);
            windowThongTin.Show();
        }


        private void btnthongke_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.Show();
        }

        private void dtgsanpham_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dtgsanpham.SelectedItem != null)
            {
                try
                {
                    Type t = dtgsanpham.SelectedItem.GetType();
                    PropertyInfo[] p = t.GetProperties();
                    txtmasp.Text = p[0].GetValue(dtgsanpham.SelectedItem).ToString();
                    txttensp.Text = p[1].GetValue(dtgsanpham.SelectedItem).ToString();
                    txtsoluongco.Text = p[3].GetValue(dtgsanpham.SelectedItem).ToString();
                    txtdongia.Text = p[4].GetValue(dtgsanpham.SelectedItem).ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi khi chọn. \nLỗi: " + ex.Message, "Thông Báo");
                }

            }
        }
    }
}