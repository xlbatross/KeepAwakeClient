using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using OpenCvSharp;

namespace OpenCVForm
{
    internal class Encode
    {

        public enum EncodeType : int
        {
            Login = 0,
            DrivingImage = 1
        }

        public List<List<byte>> DataBytesList { get; set; }

        public List<byte> HeaderBytes { get; set; }

        public List<byte> DataBytes { get; set; }

        public Encode()
        {
            DataBytesList = new();
            HeaderBytes = new();
            DataBytes = new();
        }

        public byte[] totalSizeByte()
        {
            return BitConverter.GetBytes(HeaderBytes.Count + DataBytes.Count);
        }
    }

    internal class EncodeTCP : Encode
    {
        public EncodeTCP()
        {
            
        }

        public void packaging(int typeValue)
        {
            List<int> headerList = new List<int>();

            // 헤더
            // 헤더의 길이(4바이트 정수형, 이 길이값은 이 뒤에 오는 데이터의 길이를 의미한다.)
            // + 요청 타입(4바이트 정수형) + 데이터 하나의 바이트 길이(4바이트 정수형) * ((헤더의 길이 / 4바이트) - 1)
            headerList.Add(typeValue);
            foreach (List<byte> db in DataBytesList)
            {
                headerList.Add(db.Count); // 데이터 하나의 바이트 길이
                DataBytes.AddRange(db); // 데이터 하나
            }

            // 헤더의 길이
            HeaderBytes.AddRange(BitConverter.GetBytes(headerList.Count * 4));
            // 인코딩 타입 + 데이터 하나의 바이트 길이
            foreach (int i in headerList)
            {
                HeaderBytes.AddRange(BitConverter.GetBytes(i));
            }
        }
    }

    internal class EcdLogin : EncodeTCP
    {

        public EcdLogin(Mat img)
        {
            DataBytesList.Add(BitConverter.GetBytes(img.Rows).ToList());
            DataBytesList.Add(BitConverter.GetBytes(img.Cols).ToList());
            img = img.Reshape(1);
            byte[] imaBuff;
            img.GetArray(out imaBuff);
            DataBytesList.Add(imaBuff.ToList());
            //가히
            var nowTime = DateTime.Now.ToString("hh:mm:ss");
            DataBytesList.Add(Encoding.UTF8.GetBytes(nowTime).ToList());
            packaging((int)EncodeType.Login);
        }
    }

    internal class EcdDrivingImage : EncodeTCP
    {
        public EcdDrivingImage(Mat img) 
        {
            DataBytesList.Add(BitConverter.GetBytes(img.Rows).ToList());
            DataBytesList.Add(BitConverter.GetBytes(img.Cols).ToList());
            img = img.Reshape(1);
            byte[] imaBuff;
            img.GetArray(out imaBuff);
            DataBytesList.Add(imaBuff.ToList());
            //가히
            var nowTime = DateTime.Now.ToString("hh:mm:ss");
            DataBytesList.Add(Encoding.UTF8.GetBytes(nowTime).ToList());
            packaging((int)EncodeType.DrivingImage);
        }
    }

    
}
