using OpenCvSharp;

namespace OpenCVForm
{
    public partial class Form1 : Form
    {
        VideoCapture cap = new VideoCapture(0);
        Client client = new Client();
        Notify notify = new Notify();
        bool isDriving = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // �󺧰� ������ ���ε�
            BindingSource labelBinding = new BindingSource();
            labelBinding.DataSource = notify;
            label1.DataBindings.Add(new Binding("Text", labelBinding, "Text", true, DataSourceUpdateMode.OnPropertyChanged));
            
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
            else if (!isConnected) 
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
                case (int)Decode.DecodeType.:
                    {
                        DcdImage dcdImage = new DcdImage((DecodeTCP)dcd);
                        pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dcdImage.img);
                    }
                    break;
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Mat img = new Mat();
            if(cap.Read(img))
            {
                pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img);
                if (isDriving)
                {
                    client.SendDrivingImage(img);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}