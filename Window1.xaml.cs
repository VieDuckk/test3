using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LamLaiQuanLyBanHang.Models;

namespace LamLaiQuanLyBanHang
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            QLBanHangContext db = new QLBanHangContext();
            var query = from sp in db.SanPhams
                        join ls in db.LoaiSanPhams
                        on sp.MaLoai equals ls.MaLoai
                        select new
                        {
                            sp.MaSp,
                            sp.TenSp,
                            ls.TenLoai,
                            sp.SoLuong,
                            sp.DonGia,
                            ThanhTien = sp.DonGia * sp.SoLuong
                        };
            dtgsanpham.ItemsSource = query.ToList();
        }
    }
}
