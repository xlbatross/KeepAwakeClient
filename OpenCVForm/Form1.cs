using OpenCvSharp;

namespace OpenCVForm
{
    public partial class Form1 : Form
    {
        VideoCapture cap = new VideoCapture(0);
        Mat img = new Mat();
        Client client = new Client();
        Notify notify = new Notify();
        bool isDriving = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindingSource labelBinding = new BindingSource();
            labelBinding.DataSource = notify;
            label1.DataBindings.Add(new Binding("Text", labelBinding, "Text", true, DataSourceUpdateMode.OnPropertyChanged));
            
            client.Connected += client_Connected;
            client.DataResponsed += client_DataResponsed;
            client.Connect();
            MessageBox.Show("������ ���� ���Դϴ�.");
        }

        private void client_Connected(object sender, EventArgs e) 
        {
            bool isConnected = (bool)sender;
            if (isConnected) 
            {
                timer1.Start();
            }
            else
            {
                MessageBox.Show("������ ������ �����߽��ϴ�");
                Application.Exit();
            }
        }
        private void client_DataResponsed(object sender, EventArgs e)
        {
            Decode dcd = (Decode)sender;

            switch (dcd.Type)
            {
                case (int)Decode.DecodeType.Image:
                    {
                        DcdImage dcdImage = new DcdImage((DecodeTCP)dcd);
                        pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dcdImage.img);
                    }
                    break;
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            cap.Read(img);
            pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img);
            // client.SendImage(img);
            notify.Text = "3";
        }

        

        
    }
}