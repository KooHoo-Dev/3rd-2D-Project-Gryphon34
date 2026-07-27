using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Study.CsharpAdv.FileIO
{
    public class StreamReaderAndWriter : MonoBehaviour
    {
        public string FilePath;

        private void Start()
        {
            //StreamWriter writer = new StreamWriter(FilePath);
            //writer.WriteLine("첫번째 줄");
            //.WriteLine("두번째 줄");
            //writer.Close();
            
            StreamReader streamReader = new StreamReader(FilePath);
            
            List<string> lines = new List<string>();
            while (streamReader.ReadLine() != null)
            {
                string line = streamReader.ReadLine();
                if (line == null) break;
                lines.Add(line);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                Debug.Log(lines[i]);
            }
        }
    }
}