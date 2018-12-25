using Cosockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ManageStreaming : MonoBehaviour
{
    internal Boolean socketReady = false;
    TcpClient mySocket;
    NetworkStream theStream;
    BinaryWriter theWriter;
    BinaryReader theReader;

    String Host = "192.168.43.141";
    Int32 Port = 4148;
    byte[] imageBuff;
    Texture2D texture;
    // Use this for initialization
    void Start()
    {
        Application.runInBackground = true;
        texture = new Texture2D(640, 480, TextureFormat.RGB24, false);
        imageBuff = new byte[640 * 480 * 3];
        setupSocket();

        writeSocket("HELLLO");
        StartCoroutine(_corout());
    }

    IEnumerator _corout()
    {

        while (true)
        {
            yield return readSocket();
            texture.LoadRawTextureData(imageBuff);
            texture.Apply();
            this.gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;

        }
    }


    private void _displayImage(byte[] image)
    {
        texture.LoadRawTextureData(imageBuff);
        texture.Apply();
        this.gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setupSocket()
    {

        try
        {
            mySocket = new TcpClient(Host, Port);
            theStream = mySocket.GetStream();
            theWriter = new BinaryWriter(theStream);
            theReader = new BinaryReader(theStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
        }
    }
    public void writeSocket(string theLine)
    {
        if (!socketReady)
            return;
        String foo = theLine + "\r\n";
        theWriter.Write(foo);
        theWriter.Flush();
    }
    public IEnumerator readSocket()
    {
        //if (!socketReady)
        //    return;

        //if (theStream.DataAvailable)
        //    return theReader.ReadLine();
        int count = 0;
        int ret = 0;
        while (count < (640 * 480 * 3))
        {
            ret = theReader.Read(imageBuff, count, Mathf.Min((640 * 480 * 3) - count, 500000));
            if (ret == 0)
            {
                //return null;
            }
            yield return null;
            count += ret;
        }
    }
    public void closeSocket()
    {
        if (!socketReady)
            return;
        theWriter.Close();
        theReader.Close();
        mySocket.Close();
        socketReady = false;
    }
}
