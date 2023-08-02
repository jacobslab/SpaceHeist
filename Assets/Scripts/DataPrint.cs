using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.IO.Ports;


public class DataPrint : MonoBehaviour
{
    private string myFilePath;
    public  int participant_num;
    public int version;
    public  int block_num;
    private static int v; //version static
    private static int b; //block static
    private static int pnum; //participant num static
   
   
   //SERIAL PORT
    public SerialPort sp;
    float next_time;

    void Start()
    {
        serialportEnabling();
        v = version;
        pnum = participant_num;
        b = block_num;

    }

    public void sendttlPulse(int pulseCount, int time){
        string the_com = "COM3";
        sp = new SerialPort("\\\\.\\" + the_com, 9600);
        if(!sp.IsOpen)   sp.Open();
        if(sp.IsOpen)  sp.Write(time+"+"+pulseCount);
        sp.Close();
    }


    public void serialportEnabling(){
        string the_com="";
        next_time = Time.time;
        
        foreach (string mysps in SerialPort.GetPortNames())
        {
            print(mysps);
            if (mysps != "COM1")
            {
                the_com = mysps;
                break;
            }
        }

        sp = new SerialPort("\\\\.\\" + the_com, 9600);
        if (!sp.IsOpen)
        {
            print("Opening " + the_com + ", baud 9600");
            sp.Open();
            sp.ReadTimeout = 100;
            sp.Handshake = Handshake.None;
            if (sp.IsOpen) { print("Open"); }
        }


    }

}
