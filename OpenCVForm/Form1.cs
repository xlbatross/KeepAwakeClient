using Microsoft.VisualBasic.Devices;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using OpenCvSharp;

namespace OpenCVForm
{
    public partial class Form1 : Form
    {
        VideoCapture cap = new VideoCapture(0);
        Client client = new Client();
        Mat img = new Mat();
        bool isDriving = false;

        NAudio.Wave.WaveInEvent waveIn = new NAudio.Wave.WaveInEvent
        {
            DeviceNumber = 0, // indicates whick microphone to use
            WaveFormat = new NAudio.Wave.WaveFormat(rate: 44100, bits: 16, channels: 1),
            BufferMilliseconds = 20
        };
        NAudio.Wave.WaveOutEvent waveOut = new NAudio.Wave.WaveOutEvent();
        int decibel_count = 0;
        //가히
        //int append_count = 0;
        //int drowsy_count = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            decibel_count = 0;
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.StartRecording();

            var reader = new AudioFileReader("./Y_Stan_remix.mp3");
            waveOut.Init(reader);
            waveOut.Play();

            DateTime a = DateTime.Now + TimeSpan.Parse("00:10:00");
            MessageBox.Show((DateTime.Now > a) ? "True" : "False");

            // 클라이언트의 이벤트 핸들러 연결
            client.Connected += client_Connected;
            client.DataResponsed += client_DataResponsed;

            // 클라이언트와 서버의 연결 시작
            client.Connect();
            timer1.Start();
        }

        private void WaveIn_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            // copy buffer into an array of integers
            Int16[] values = new Int16[e.Buffer.Length / 2];
            Buffer.BlockCopy(e.Buffer, 0, values, 0, e.Buffer.Length);

            // determine the highest value as a fraction of the maximum possible value
            float fraction = (float)values.Max() / 32768;

            int decibel = (int)(fraction * 100);

            // print a level meter using the console
            progressBar1.Value = decibel;
            if (decibel == 99)
                decibel_count += 1;
            label1.Text = decibel_count.ToString();

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
                MessageBox.Show("서버와 연결하지 못하였습니다.");
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
                            foreach (string time in dcdLoginResult.DrowsyAvg)
                            {
                                textBox1.AppendText(time + "\r\n");
                            }
                        }    
                    } break;
                case (int)Decode.DecodeType.DrivingResult:
                    {
                        DcdDrivingResult dcdDrivingResult = new DcdDrivingResult((DecodeTCP)dcd);
                        pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dcdDrivingResult.img);
                        //가히
                        
                        if (dcdDrivingResult.text != "")
                        {
                            textBox1.AppendText(dcdDrivingResult.text + "\r\n");
                            //append_count += 1;
                        }
                        
                        /*if (append_count >= 5)
                        {
                            drowsy_count += 1; 
                            client.SendDrowsyCount(drowsy_count);
                        }*/
                        
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
                textBox1.AppendText("운전을 정지했습니다." + "\r\n");
                isDriving = false;
                btn_start.Text = "Start";
            }
        }

        
    }
}