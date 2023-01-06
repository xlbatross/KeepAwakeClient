using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace OpenCVForm
{
    internal class Decode
    {
        public enum DecodeType : int
        {
            Image = 0
        }

        public int Type { get; set; }

        public List<List<byte>> DataBytesList { get; set; }

        public Decode()
        {
            DataBytesList= new List<List<byte>>();
            Type = -1;
        }
    }

    internal class DecodeTCP : Decode
    {
        public DecodeTCP(byte[] rawData)
        {
            List<int> dataLengthList = new List<int>();
            int pointer = 0;
            int headerSize = BitConverter.ToInt32(rawData, pointer);
            pointer += 4;

            Type = BitConverter.ToInt32(rawData, pointer);
            pointer += 4;

            for (int i = 0; i < headerSize - 4; i += 4)
            {
                dataLengthList.Add(BitConverter.ToInt32(rawData, pointer));
                pointer+= 4;
            }

            foreach (int length in dataLengthList) 
            {
                DataBytesList.Add(rawData[pointer..(pointer + length)].ToList());
                pointer += length;
            }
        }
    }

    internal class DcdImage
    {
        public int Rows { get; set; }

        public int Cols { get; set; }

        public Mat img { get; set; }

        public DcdImage(DecodeTCP dcdtcp)
        {
            Rows = BitConverter.ToInt32(dcdtcp.DataBytesList[0].ToArray(), 0);
            Cols = BitConverter.ToInt32(dcdtcp.DataBytesList[1].ToArray(), 0);
            img = new Mat(Rows, Cols, MatType.CV_8UC3, dcdtcp.DataBytesList[2].ToArray());
        }
    }
}
