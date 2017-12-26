using System.Windows;
using CoreTweet;
using iTunesLib;
using TM = System.Timers;
using System;

namespace NowPlayingonWindows
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //変数の指定
        OAuth.OAuthSession session;
        public Tokens tokens;
        private iTunesApp iTunes;

        public MainWindow()
        {
            InitializeComponent();
        }

        //PINCODEを取得
        private void button_Click(object sender, RoutedEventArgs e)
        {
            session = OAuth.Authorize("5hCvlgzl4pT7LmdKwNwTpSanq", "vli2vuIYm1jLSjV7A2HhLGEhYaoIeCBJKiPz01jGe3X4M5dkuO");
            System.Diagnostics.Process.Start(session.AuthorizeUri.AbsoluteUri);
        }

        //トークンを取得
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (session == null)
            {
                System.Windows.Forms.MessageBox.Show("認証をしてください");
                return;
            }


            string pincode = textbox.Text;
            tokens = OAuth.GetTokens(session, pincode);
            System.Windows.Forms.MessageBox.Show("認証に成功しました。");

        }

        //6分ごとに処理
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            TM.Timer timer = new TM.Timer();
            timer.Elapsed += new TM.ElapsedEventHandler(TimeDisp);
            timer.Interval = 360000;
            timer.AutoReset = true;
            timer.Enabled = true;

        }

        //ツイート
        private void TimeDisp(object sender, EventArgs e)
        {
            //iTunes COMのセットアップ
            iTunesApp iTunes = new iTunesApp();
            IITTrack track = iTunes.CurrentTrack;

            if (iTunes == null)
            {
                System.Windows.Forms.MessageBox.Show("iTunesで曲が再生されていません");
                return;
            }


            //変数の指定
            var AlbumName = track.Album;
            var TrackInfo = track.TrackNumber + " / " + track.TrackCount;
            var Artist = track.Artist;
            var TrackName = track.Name;
            var Ruledline = "│";
            var nowplaying = "#nowplaying\n";
            IITArtworkCollection artwork = track.Artwork;
            DateTime dt = DateTime.Now;

            //ツイート
            var text = nowplaying + AlbumName + Ruledline + TrackName + Ruledline + Artist + Ruledline + dt;
            tokens.Statuses.Update(status => text);

            //お知らせ
            System.Windows.Forms.MessageBox.Show("実行します。");
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
