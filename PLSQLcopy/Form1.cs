using System;
using System.Threading;
using System.Windows.Forms;

namespace PLSQLcopy {
    public partial class Form1 : Form {
        private MyClipboardViewer viewer;
        private string loginCode = "";
        private string clipbordStr = "";
        private string[] Reservation = {
            "END;",
            "end;",
            "/"
        };

        public Form1() {
            // イベントハンドラを登録
            viewer = new MyClipboardViewer(this);
            viewer.ClipboardHandler += this.OnClipBoardChanged;
            InitializeComponent();

            textBox1.Text = Properties.Settings.Default.username;
            textBox2.Text = Properties.Settings.Default.password;
            textBox3.Text = Properties.Settings.Default.tablename;
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
                    Microsoft.VisualBasic.Interaction.AppActivate($@"C:\app\{System.Net.Dns.GetHostName()}\product\12.2.0\dbhome_1\bin\sqlplus.exe");

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
                    System.Diagnostics.Process.Start($@"C:\app\{System.Net.Dns.GetHostName()}\product\12.2.0\dbhome_1\bin\sqlplus.exe");
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
                            Microsoft.VisualBasic.Interaction.AppActivate($@"C:\app\{System.Net.Dns.GetHostName()}\product\12.2.0\dbhome_1\bin\sqlplus.exe");
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

        /// <summary>
        /// テキスト変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e) {
            Properties.Settings.Default.username = this.textBox1.Text;
            Properties.Settings.Default.password = this.textBox2.Text;
            Properties.Settings.Default.tablename = this.textBox3.Text;
            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e) {
        }
    }
}
