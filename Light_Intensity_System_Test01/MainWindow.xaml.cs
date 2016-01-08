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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.IO.Ports;

using System.Threading;
using System.Windows.Threading;

using System.Net.Sockets;
using System.Net;

using System.Diagnostics;

namespace Light_Intensity_System_Test01
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        public SerialPort serialPort1 = new SerialPort();

        const int origin_x = 431;
        const int origin_y = 501;

        public string nowlux = "0";

        public MainWindow()
        {
            InitializeComponent();

            BT_Start.Click += BT_Start_Click;
            BT_Stop.Click += BT_Stop_Click;

            TB_Light1_x.TextChanged += TB_Light1_x_TextChanged;
            TB_Light1_y.TextChanged += TB_Light1_y_TextChanged;
            TB_Light2_x.TextChanged += TB_Light2_x_TextChanged;
            TB_Light2_y.TextChanged += TB_Light2_y_TextChanged;
            TB_Light3_x.TextChanged += TB_Light3_x_TextChanged;
            TB_Light3_y.TextChanged += TB_Light3_y_TextChanged;

            TB_Light1_x.Text = "84.68";
            TB_Light1_y.Text = "247.84";
            TB_Light2_x.Text = "263.49";
            TB_Light2_y.Text = "27.54";
            TB_Light3_x.Text = "28.6";
            TB_Light3_y.Text = "74.51";

            Thread thread = new Thread(new ThreadStart(ThreadMethod_TCP));
            thread.Start();

        }

        void BT_Stop_Click(object sender, RoutedEventArgs e)
        {
        }

        #region 光源座標変換

        void TB_Light3_y_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                El_Light3.SetValue(Canvas.TopProperty, (double)(origin_y - double.Parse(TB_Light3_y.Text)));
            }
            catch
            {
                El_Light3.SetValue(Canvas.TopProperty, (double)(origin_y));
            }

        }

        void TB_Light3_x_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                El_Light3.SetValue(Canvas.LeftProperty, (double)(origin_x + double.Parse(TB_Light3_x.Text)));
            }
            catch
            {
                El_Light3.SetValue(Canvas.LeftProperty, (double)(origin_x));
            }
        }

        void TB_Light2_y_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                El_Light2.SetValue(Canvas.TopProperty, (double)(origin_y - double.Parse(TB_Light2_y.Text)));
            }
            catch
            {
                El_Light2.SetValue(Canvas.TopProperty, (double)(origin_y));
            }
        }

        void TB_Light2_x_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                El_Light2.SetValue(Canvas.LeftProperty, (double)(origin_x + double.Parse(TB_Light2_x.Text)));
            }
            catch
            {
                El_Light2.SetValue(Canvas.LeftProperty, (double)(origin_x));
            }
        }

        void TB_Light1_y_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                El_Light1.SetValue(Canvas.TopProperty, (double)(origin_y - double.Parse(TB_Light1_y.Text)));
            }
            catch
            {
                El_Light1.SetValue(Canvas.TopProperty, (double)(origin_y));
            }
        }

        void TB_Light1_x_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                El_Light1.SetValue(Canvas.LeftProperty, (double)(origin_x + double.Parse(TB_Light1_x.Text)));
            }
            catch
            {
                El_Light1.SetValue(Canvas.LeftProperty, (double)(origin_x));
            }
        }

        #endregion

        void BT_Start_Click(object sender, RoutedEventArgs e)
        {

            //int lux1 = 100;
            //int lux2 = 200;
            //int lux3 = 300;

            //TB_Light1_Lux.Text = lux1.ToString();
            //TB_Light2_Lux.Text = lux2.ToString();
            //TB_Light3_Lux.Text = lux3.ToString();

            //Ellipse myEllipse = new Ellipse();
            //myEllipse.StrokeThickness = 2;
            //myEllipse.Stroke = Brushes.Black;
            //myEllipse.Width = lux1;
            //myEllipse.Height = lux1;

            //Cv_Room.Children.Add(myEllipse);

            //myEllipse.SetValue(Canvas.TopProperty, (double)origin_y - double.Parse(TB_Light1_y.Text) - (double)myEllipse.Height / 2.0 + (double)El_Light1.Height / 2.0);
            //myEllipse.SetValue(Canvas.LeftProperty, (double)origin_x + double.Parse(TB_Light1_x.Text) - (double)myEllipse.Width / 2.0 + (double)El_Light1.Width / 2.0);

            #region 照度取得

            double lux_1 = 0;
            double lux_2 = 0;
            double lux_3 = 0;

            double beforelux = double.Parse(nowlux);
            TB_Top.Text = SendRemocon("192_168_10_81","is;101");
            System.Threading.Thread.Sleep(2000);
            lux_1 = double.Parse(nowlux) - beforelux;
            TB_Light1_Lux.Text = lux_1.ToString();

            beforelux = double.Parse(nowlux);
            TB_Top.Text = SendRemocon("192_168_10_80", "is;103");
            System.Threading.Thread.Sleep(2000);
            lux_2 = double.Parse(nowlux) - beforelux;
            TB_Light2_Lux.Text = lux_2.ToString();

            beforelux = double.Parse(nowlux);
            TB_Top.Text = SendRemocon("192_168_10_69", "is;104");
            System.Threading.Thread.Sleep(2000);
            lux_3 = double.Parse(nowlux) - beforelux;
            TB_Light3_Lux.Text = lux_3.ToString();

            #endregion

            #region 交点が出来ないパターンを考慮

            double m1 = Math.Pow((lux_1 / 143.79), (1.0 / -2.082));
            double m2 = Math.Pow((lux_2 / 143.79), (1.0 / -2.082));
            double m3 = Math.Pow((lux_3 / 143.79), (1.0 / -2.082));

            double lux1_x = double.Parse(TB_Light1_x.Text);
            double lux1_y = double.Parse(TB_Light1_y.Text);
            double lux2_x = double.Parse(TB_Light2_x.Text);
            double lux2_y = double.Parse(TB_Light2_y.Text);
            double lux3_x = double.Parse(TB_Light3_x.Text);
            double lux3_y = double.Parse(TB_Light3_y.Text);

            double distance12 = Math.Sqrt(Math.Pow(lux1_x - lux2_x, 2.0) + Math.Pow(lux1_y - lux2_y, 2.0)) * 0.0123466;
            double distance13 = Math.Sqrt(Math.Pow(lux1_x - lux3_x, 2.0) + Math.Pow(lux1_y - lux3_y, 2.0)) * 0.0123466;
            double distance23 = Math.Sqrt(Math.Pow(lux2_x - lux3_x, 2.0) + Math.Pow(lux2_y - lux3_y, 2.0)) * 0.0123466;

            while (distance12 > m1 + m2) {
                m1 = m1 * 1.01;
                m2 = m2 * 1.01;
            }
            while (distance12 < m1 - m2) {
                m1 = m1 * 0.99;
                m2 = m2 * 1.01;
            }
            while (distance12 < m2 - m1) {
                m1 = m1 * 1.01;
                m2 = m2 * 0.99;
            }

            while (distance13 > m1 + m3) {
                m1 = m1 * 1.01;
                m3 = m3 * 1.01;
            }
            while (distance13 < m1 - m3) {
                m1 = m1 * 0.99;
                m3 = m3 * 1.01;
            }
            while (distance13 < m3 - m1) {
                m1 = m1 * 1.01;
                m3 = m3 * 0.99;
            }

            while (distance23 > m2 + m3) {
                m2 = m2 * 1.01;
                m3 = m3 * 1.01;
            }
            while (distance23 < m2 - m3) {
                m2 = m3 * 0.99;
                m3 = m3 * 1.01;
            }
            while (distance23 < m3 - m2) {
                m2 = m2 * 1.01;
                m3 = m3 * 0.99;
            }

            #endregion

            #region 交点を求める

            double r1 = m1;
            double r2 = m2;
            double r3 = m3;

            double x1 = lux1_x * 0.0123466;
            double y1 = lux1_y * 0.0123466;
            double z1 = 2.15;

            double x2 = lux2_x * 0.0123466;
            double y2 = lux2_y * 0.0123466;
            double z2 = 2.15;

            double x3 = lux3_x * 0.0123466;
            double y3 = lux3_y * 0.0123466;
            double z3 = 2.15;

            double x21 = x2 - x1;
            double x31 = x3 - x1;
            double y21 = y2 - y1;
            double y31 = y3 - y1;
            double z21 = z2 - z1;
            double z31 = z3 - z1;

            double A1 = Math.Pow(r1, 2.0) - Math.Pow(x1, 2.0) - Math.Pow(y1, 2.0) - Math.Pow(z1, 2.0);
            double A2 = Math.Pow(r2, 2.0) - Math.Pow(x2, 2.0) - Math.Pow(y2, 2.0) - Math.Pow(z2, 2.0);
            double A3 = Math.Pow(r3, 2.0) - Math.Pow(x3, 2.0) - Math.Pow(y3, 2.0) - Math.Pow(z3, 2.0);
            double A21 = -(A2 - A1) / 2.0;
            double A31 = -(A3 - A1) / 2.0;

            double D = x21 * y31 - y21 * x31;
            double B0 = (A21 * y31 - A31 * y21) / D;
            double B1 = (y21 * z31 - y31 * z21) / D;
            double C0 = (A31 * x21 - A21 * x31) / D;
            double C1 = (x31 * z21 - x21 * z31) / D;

            double E = Math.Pow(B1, 2.0) + Math.Pow(C1, 2.0) + 1;
            double F = B1 * (B0 - x1) + C1 * (C0 - y1) - z1;
            double G = Math.Pow(B0 - x1, 2.0) + Math.Pow(C0 - y1, 2.0) + Math.Pow(z1, 2.0) - Math.Pow(r1, 2.0);

            double Dz = Math.Pow(F, 2.0) - E * G;

            Debug.WriteLine("Dz:" + Dz.ToString());

            double z = (-F - Math.Sqrt(Math.Pow(F,2.0) - E*G)) / E;
            double x = B0 + B1 * z;
            double y = C0 + C1 * z;

            Debug.WriteLine("実座標: (" + x.ToString() + "," + y.ToString() + "," + z.ToString() + ")");

            double xx = x * 80.994152;
            double yy = y * 80.994152;

            Debug.WriteLine("アプリ座標: (" + xx.ToString()+ "," + yy.ToString() + ")" );

            #endregion

            Im_Star.SetValue(Canvas.LeftProperty, (double)(origin_x + xx - (double)Im_Star.Width / 2.0 ));
            Im_Star.SetValue(Canvas.TopProperty, (double)(origin_y - yy - (double)Im_Star.Height / 2.0));
            Im_Star.Visibility = Visibility.Visible;

            SendRemocon("192_168_10_81", "is;101");
            SendRemocon("192_168_10_80", "is;103");
            SendRemocon("192_168_10_69", "is;104");

            string postData = "\"" + (4.24-x).ToString() + "\",\"" + y.ToString() + "\",\"" + z.ToString() + "\"";
            SendCats(postData);
            
        }

        void SerialPortReceivedData(object sender, SerialDataReceivedEventArgs e)
        {
            //! シリアルポートのバッファーの読み込む
            string data = serialPort1.ReadExisting();
            Console.Write(data);
        }

        string SendRemocon(string url, string comand) {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://192.168.10.67/iRemoconManage/api/run/"+url+"/"+comand);
            req.Method = "GET";
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Stream s = res.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string content = sr.ReadToEnd();

            return content;
        }

        public void ThreadMethod_TCP()
        {
            //ListenするIPアドレスを決める
            string host = "localhost";
            System.Net.IPAddress ipAdd =
                System.Net.Dns.GetHostEntry(host).AddressList[0];

            //Listenするポート番号
            int port = 1111;

            //TcpListenerオブジェクトを作成する
            System.Net.Sockets.TcpListener listener =
                new System.Net.Sockets.TcpListener(IPAddress.Any, port);

            //Listenを開始する
            listener.Start();
            TB_Top.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                TB_Top.Text = "Listenを開始しました(" + ((System.Net.IPEndPoint)listener.LocalEndpoint).Address + ":" + ((System.Net.IPEndPoint)listener.LocalEndpoint).Port + ")。";
            }));

            //接続要求があったら受け入れる
            System.Net.Sockets.TcpClient client = listener.AcceptTcpClient();
            TB_Top.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                TB_Top.Text = "クライアント(" + ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address + ":" + ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Port + ")と接続しました。";
            }));

            TB_Top.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {

            }));

            //NetworkStreamを取得
            System.Net.Sockets.NetworkStream ns = client.GetStream();

            //クライアントから送られたデータを受信する
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            bool disconnected = false;
            string resMsg;
            while (true) {
                disconnected = false;
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                byte[] resBytes = new byte[256];
                do {
                    //データの一部を受信する
                    int resSize = ns.Read(resBytes, 0, resBytes.Length);
                    //Readが0を返した時はクライアントが切断したと判断
                    if (resSize == 0) {
                        //disconnected = true;
                        TB_Top.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                            TB_Top.Text = "クライアントが切断しました。";
                        }));
                        break;
                    }
                    //受信したデータを蓄積する
                    ms.Write(resBytes, 0, resSize);
                } while (ns.DataAvailable);
                //受信したデータを文字列に変換
                resMsg = enc.GetString(ms.ToArray());

                string sendMsg = resMsg.ToString() + "(res)";
                byte[] sendBytes = enc.GetBytes(sendMsg);
                ns.Write(sendBytes, 0, sendBytes.Length);

                ms.Close();
                nowlux = resMsg.ToString();
                TB_Top.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                    TB_Top.Text = resMsg;
                }));
                if (resMsg.Contains("end")) {
                    break;
                }
                if (disconnected) {
                    break;
                }
            }

            //閉じる
            ns.Close();
            client.Close();
            TB_Top.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                TB_Top.Text = "クライアントとの接続を閉じました。";
            }));

            //リスナを閉じる
            listener.Stop();
            TB_Top.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                TB_Top.Text = "Listenerを閉じました。";
            }));
            
        }

        public void SendCats(string postData) {
            // POSTする対象のURL
            string url = "http://192.168.10.67/Cats/api/dataadd/marimo/module01";
            // POSTメソッドで渡すパラメータ
            string param = "[{ \"dt\": null, \"pw\": null, \"dat\": [" + postData + "] }]";
            //string param = "[{ \"dt\": null, \"pw\": null, \"dat\": [\"111\",\"111\",\"111\"] }]";

            // paramをUTF-8文字列にエンコードする
            byte[] data = Encoding.UTF8.GetBytes(param);

            // リクエスト作成
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/JSON";
            request.ContentLength = data.Length;

            // ポストデータをリクエストに書き込む
            using (Stream reqStream = request.GetRequestStream())
                reqStream.Write(data, 0, data.Length);

            // レスポンスの取得
            WebResponse response = request.GetResponse();

            // 結果の読み込み
            string htmlString = "";
            using (Stream resStream = response.GetResponseStream())
            using (var reader = new StreamReader(resStream, Encoding.GetEncoding("utf-8")))
                htmlString = reader.ReadToEnd();
        }

    }
}
