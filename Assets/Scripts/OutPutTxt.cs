using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class OutPutTxt
{
    public Thread t;
    int i = 0;

    public bool LOOP;
    string outstr = "";


    public OutPutTxt()
    {

    }

    public void StartThread()
    {
        t = new Thread(new ThreadStart(OutPutTxtLOG));
        LOOP = true;
        t.Start();
    }

    


    public void OutPutTxtLOG()
    {

        Thread.Sleep(1000);

        while (LOOP)
        {

        
            if (ValueSheet.output.Count > 0)
            {
                i++;
                if (i < 600)
                {
                    outstr += ValueSheet.output.Dequeue() + "\n";
                }
                else
                {
                    //"F:\\TestTxt.txt"
                    i = 0;
                    //string path = "D:\\"+ System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt";

                    string path = Application.streamingAssetsPath+"/" + System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt";

                    FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);//创建写入文件 
                    StreamWriter sw = new StreamWriter(fs1);
                    sw.WriteLine(outstr);//开始写入值
                    sw.Close();
                    fs1.Close();
                    outstr = "";
                }
            }
        }
        OutLastStr();

        Thread.Sleep(1000);

        t.Abort();
    }

    public void OutLastStr()
    {
        if (outstr.Length > 0)
        {

            //string path = "D:\\" + System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt";

            string path = Application.streamingAssetsPath+"/" + System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt";
            FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);//创建写入文件 
            StreamWriter sw = new StreamWriter(fs1);
            sw.WriteLine(outstr);//开始写入值
            sw.Close();
            fs1.Close();
            outstr = "";
        }
    }


//     if (!File.Exists("F:\\TestTxt.txt"))
//            {
//                FileStream fs1 = new FileStream("F:\\TestTxt.txt", FileMode.Create, FileAccess.Write);//创建写入文件 
//    StreamWriter sw = new StreamWriter(fs1);
//    sw.WriteLine(ValueSheet.output.Dequeue()+"/n");//开始写入值
//                sw.Close();
//                fs1.Close();
//            }
//            else
//{
//    FileStream fs = new FileStream("F:\\TestTxt.txt", FileMode.Open, FileAccess.Write);
//    StreamWriter sr = new StreamWriter(fs);
//    sr.WriteLine(ValueSheet.output.Dequeue() + "/n");//开始写入值
//    sr.Close();
//    fs.Close();
//}
}
