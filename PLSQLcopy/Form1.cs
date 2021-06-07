using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace PLSQLcopy {
    public partial class Form1 : Form {
        private MyClipboardViewer viewer;
        private string loginCode = "";
        private string clipbordStr = "";
        private string[] Reservation = {
            //"DECLARE",
            //"BEGIN",
            "END;",
            "/"
        };

        public Form1() {
            // イベントハンドラを登録
            viewer = new MyClipboardViewer(this);
            viewer.ClipboardHandler += this.OnClipBoardChanged;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            SqlPlusPastePlay();
        }

        /// <summary>
        /// クリップボードにテキストがコピーされると呼び出される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnClipBoardChanged(object sender, ClipboardEventArgs args) {
            clipbordStr = args.Text;

            // SQL? check!
            int fc = 0;
            foreach (var item in Reservation) {
                bool sqlCheckResult = clipbordStr.Contains(item);

                if (sqlCheckResult) {
                    fc++;
                }
            }
            if (fc >= 2) {
                SqlPlusPastePlay();
            }
        }

        /// <summary>
        /// SQL Plusをアクティブにしてペースト実行
        /// </summary>
        private void SqlPlusPastePlay() {
            if (textBox1.Text.Length >= 5) {
                try {
                    // window select
                    Microsoft.VisualBasic.Interaction.AppActivate(@"C:\app\CRCL082\product\12.2.0\dbhome_1\bin\sqlplus.exe");

                    // paste -> enter
                    SendKeys.Send("^v{ENTER}");
                } catch (Exception) {
                    //StartSqlPlus();
                    MessageBox.Show("SQL Plus を起動してください");
                }
            }
        }

        /// <summary>
        /// SQL Plus 起動
        /// </summary>
        private void SqlPlusStart() {
            if (textBox1.Text.Length >= 5) {
                try {
                    // open
                    System.Diagnostics.Process.Start(@"C:\app\CRCL082\product\12.2.0\dbhome_1\bin\sqlplus.exe");
                    Thread.Sleep(500);
                    // login
                    while (true) {
                        if (Control.IsKeyLocked(Keys.CapsLock)) {
                            using (Form f = new Form()) {
                                f.TopMost = true;
                                MessageBox.Show(f, "CapsLockがONになってます！\nOFFにしてからOKを押してください");
                                f.TopMost = false;
                            }
                        } else {
                            Microsoft.VisualBasic.Interaction.AppActivate(@"C:\app\CRCL082\product\12.2.0\dbhome_1\bin\sqlplus.exe");
                            Thread.Sleep(300);

                            break;
                        }
                    }
                    SendKeys.Send(loginCode + "{ENTER}");
                    SendKeys.Send("SET SERVEROUTPUT ON{ENTER}");
                } catch (Exception) {
                    MessageBox.Show("OpenError!!");
                }
            }
        }

        /// <summary>
        /// 起動ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartSqlPlus_Click(object sender, EventArgs e) {
            loginCode = $"{textBox1.Text}/{textBox2.Text}@{textBox3.Text}";
            SqlPlusStart();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {

        }
    }
}
