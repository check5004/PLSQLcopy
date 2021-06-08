using System;
using System.Threading;
using System.Windows.Forms;

namespace PLSQLcopy {
    public partial class Form1 : Form {
        private MyClipboardViewer viewer;
        private string loginCode = "";
        private string clipbordStr = "";
        private string sqlPlusAppPath = "";
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
            sqlPlusAppPath = $@"C:\app\{System.Net.Dns.GetHostName()}\product\12.2.0\dbhome_1\bin\sqlplus.exe";
            if (sqlPlusAppPath == Properties.Settings.Default.sqlplusapppath || Properties.Settings.Default.sqlplusapppath == "") {
                resetPath();
            } else {
                textBoxSqlPlusPath.Text = Properties.Settings.Default.sqlplusapppath;
                sqlPlusAppPath = Properties.Settings.Default.sqlplusapppath;
            }
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
                    Microsoft.VisualBasic.Interaction.AppActivate(sqlPlusAppPath);

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
                    try {
                        Microsoft.VisualBasic.Interaction.AppActivate(sqlPlusAppPath);
                        this.Activate();
                        //メッセージボックスを表示する
                        DialogResult result = MessageBox.Show("SQL Plusは既に起動しています。\n起動しますか？",
                            "質問",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button2);

                        if (result == DialogResult.Cancel) {
                            return;
                        }
                    } catch (Exception) { }
                    // open
                    System.Diagnostics.Process.Start(sqlPlusAppPath);
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
                            Microsoft.VisualBasic.Interaction.AppActivate(sqlPlusAppPath);
                            Thread.Sleep(300);

                            break;
                        }
                    }
                    SendKeys.Send(loginCode + "{ENTER}");
                    SendKeys.Send("SET SERVEROUTPUT ON{ENTER}");
                } catch (Exception) {
                    MessageBox.Show("SQL Plusが開けません\nSQL Plusのパスが間違っていないか確認してください。");
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

        /// <summary>
        /// SQL Plus ファイルパス：デフォルトに戻すボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void デフォルトに戻すToolStripMenuItem_Click(object sender, EventArgs e) {
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("パスをデフォルトに戻してもよろしいですか？",
                "質問",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.OK) {
                //「OK」が選択された時
                resetPath();
            }
        }

        /// <summary>
        /// SQL Plus のパスをデフォルトに戻す
        /// </summary>
        private void resetPath() {
            sqlPlusAppPath = $@"C:\app\{System.Net.Dns.GetHostName()}\product\12.2.0\dbhome_1\bin\sqlplus.exe";
            textBoxSqlPlusPath.Text = sqlPlusAppPath;
            Properties.Settings.Default.sqlplusapppath = sqlPlusAppPath;
            Properties.Settings.Default.Save();
        }


        /// <summary>
        /// SQL Plus ファイルパス：変更ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 確定ToolStripMenuItem_Click(object sender, EventArgs e) {
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("変更したパスを適用しますか？",
                "質問",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.OK) {
                //「OK」が選択された時
                Properties.Settings.Default.sqlplusapppath = this.textBoxSqlPlusPath.Text;
                sqlPlusAppPath = this.textBoxSqlPlusPath.Text;
                Properties.Settings.Default.Save();
            }
        }
    }
}
