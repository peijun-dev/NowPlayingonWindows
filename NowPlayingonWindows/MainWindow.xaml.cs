using System.Windows;
using CoreTweet;
using iTunesLib;
using System;
using TH = System.Threading;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;

namespace NowPlayingonWindows
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>

    public class Person : INotifyPropertyChanged
    {
        private string _name;
        //iTunes COMのセットアップ
        private iTunesApp iTunes1;
        
        public string Name
        {
            get { return _name; }
            set
            {
                IITTrack track1 = iTunes1.CurrentTrack;
                if (Equals(_name, value)) return;

                _name = value;
                OnPropertyChanged(track1.Name);
            }
        }

        #region INotifyPropertyChanged メンバ

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged == null) return;

            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

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
            try
            {
                /*
                if (key.Text == "")
                {
                    MessageBox.Show("keyを入力してください");
                    return;
                }
                else if (secret.Text == "")
                {
                    MessageBox.Show("secretを入力してください");
                    return;
                }
                session = OAuth.Authorize(key.Text, secret.Text);
                System.Diagnostics.Process.Start(session.AuthorizeUri.AbsoluteUri);*/

                session = OAuth.Authorize("5hCvlgzl4pT7LmdKwNwTpSanq", "vli2vuIYm1jLSjV7A2HhLGEhYaoIeCBJKiPz01jGe3X4M5dkuO");
                System.Diagnostics.Process.Start(session.AuthorizeUri.AbsoluteUri);
            }
            catch (CoreTweet.TwitterException twe)
            {
                MessageBox.Show(twe.Message, twe.Source);
            }
            
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


        //ツイート
        private void button2_Click(object sender, RoutedEventArgs e)
        {


            //iTunes COMのセットアップ
            iTunesApp iTunes = new iTunesApp();
            IITTrack track = iTunes.CurrentTrack;

            //再生されていない時の処理
            if (track == null)
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

            Task.Run(() => {

                var p = new Person();
                
                while (true)
                {
                    // プロパティに変更があった場合に呼び出されるイベントを登録
                    p.PropertyChanged += NameChanged;
                    p.Name = track.Name;
                }
            });


        }

        private static void NameChanged(object sender, PropertyChangedEventArgs e)
        {
            //iTunes COMのセットアップ
            iTunesApp iTunes2 = new iTunesApp();
            IITTrack track2 = iTunes2.CurrentTrack;

            // 文字列でプロパティ名を判別
            if (e.PropertyName != track2.Name) return;

            // そしてキャスト
            var p = (Person)sender;

            // 各々の処理
            //iTunes COMの再セットアップ
            //iTunesApp iTunes1 = new iTunesApp();
            //IITTrack track1 = iTunes1.CurrentTrack;

            //再ツイート
            //string text1 = track1.Name;
            //tokens.Statuses.Update(status => text1);
            System.Windows.MessageBox.Show("成功だよ");
        }


        /* 内容が変更された場合の処理
        private void TxtGreeting_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //iTunes COMの再セットアップ
            iTunesApp iTunes1 = new iTunesApp();
            IITTrack track1 = iTunes1.CurrentTrack;

            //再ツイート
            string text1 = track1.Name;
            tokens.Statuses.Update(status => text1);
        }
        */


        private void button3_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
