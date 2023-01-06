using OpenCvSharp;

namespace OpenCVForm
{
    public partial class Form1 : Form
    {
        VideoCapture cap = new VideoCapture(0);
        Client client = new Client();
        Mat img = new Mat();
        bool isDriving = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Ŭ���̾�Ʈ�� �̺�Ʈ �ڵ鷯 ����
            client.Connected += client_Connected;
            client.DataResponsed += client_DataResponsed;

            // Ŭ���̾�Ʈ�� ������ ���� ����
            client.Connect();
            timer1.Start();
        }

        private void client_Connected(object sender, EventArgs e) 
        {
            bool isConnected = (bool)sender;
            if (isConnected)
            {
                panel1.Hide();
            }
            else
            {
                MessageBox.Show("������ �������� ���Ͽ����ϴ�.");
                Application.Exit();
            }
        }

        private void client_DataResponsed(object sender, EventArgs e)
        {
            Decode dcd = (Decode)sender;

            switch (dcd.Type)
            {
                case (int)Decode.DecodeType.LoginResult:
                    {
                        DcdLoginResult dcdLoginResult = new DcdLoginResult((DecodeTCP)dcd);
                        textBox1.AppendText(dcdLoginResult.Ment + "\r\n");
                        if (dcdLoginResult.Type > 0)
                        {
                            isDriving = true;
                            btn_start.Text = "End";
                        }    
                    } break;
                case (int)Decode.DecodeType.DrivingResult:
                    {
                        DcdDrivingResult dcdDrivingResult = new DcdDrivingResult((DecodeTCP)dcd);
                        pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dcdDrivingResult.img);
                    } break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(cap.Read(img))
            {
                Cv2.Flip(img, img, FlipMode.Y);
                if (isDriving)
                {
                    client.SendDrivingImage(img);
                }
                else
                {
                    pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img);
                }
            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (!isDriving)
            {
                client.SendLogin(img);
            }
            else
            {
                textBox1.AppendText("������ �����߽��ϴ�." + "\r\n");
                isDriving = false;
                btn_start.Text = "Start";
            }
        }
    }
}