using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using SimpleTCP;

namespace TIP
{

    public partial class Form1 : Form
    { 
        SerialPort serialPort = new SerialPort();
        byte[] datapackage = new byte[40];
        TcpClient client;
        bool clientCheck;
        int port;
        IPAddress iPAddress;
        public Form1()
        {  
            InitializeComponent();         
        }
        void getAvailablePorts() {
            String[] ports = SerialPort.GetPortNames();
            portNameComboBox.Items.AddRange(ports);
        }
        void intitialPorts()
        {
            
            baudRateComboBox.SelectedIndex = 6;
            dataBitsComboBox.SelectedIndex = 0;
            parityComboBox.SelectedIndex = 0;
            stopBitsComboBox.SelectedIndex = 1;
            instructionCombo.SelectedIndex = 0;
            patternDisplayCombo.SelectedIndex = 0;
        }
        void setLTL() {
            switch (statusCombobox.Text)
            {
                case "Off":
                    datapackage[2] = 0x00;
                    break;
                case "Red":
                    datapackage[2] = 0x01;
                    break;
                case "Green":
                    datapackage[2] = 0x02;
                    break;
                case "Ignore":
                    datapackage[2] = 0x03;
                    break;  
            }
      
        }
        byte bufferDisplayPattern;
        byte setdisplaypattern(ComboBox comboName)
        {
           
            //get class data from classTextbox
            switch (comboName.Text)
            {
                case "ว่าง":
                    bufferDisplayPattern = 0x00;
                    break;
                case "ยินดีต้อนรับ":
                    bufferDisplayPattern = 0x01;
                    break;
                case "ไม่พบบัตร/ชำรุด":
                    bufferDisplayPattern = 0x02;
                    break;
                case "ขอบคุณ":
                    bufferDisplayPattern = 0x03;
                    break;
                case "พร้อมทำงาน":
                    bufferDisplayPattern = 0x04;
                    break;
                case "รถ-ขบวน":
                    bufferDisplayPattern = 0x05;
                    break;
                case "ค้างชำระ":
                    bufferDisplayPattern = 0x06;
                    break;
                case "บัตรหมดอายุ":
                    bufferDisplayPattern = 0x07;
                    break;
                case "รถ-ยกเว้น":
                    bufferDisplayPattern = 0x08;
                    break;
                case "รถ-ฝ่าด่าน":
                    bufferDisplayPattern = 0x09;
                    break;
                case "เติมเงิน(Top Up)":
                    bufferDisplayPattern = 0x0A;
                    break;
                case "กลับรถ":
                    bufferDisplayPattern = 0x0B;
                    break;
                case "บัตรพิเศษ":
                    bufferDisplayPattern = 0x0C;
                    break;
                case "ปิดช่องทาง":
                    bufferDisplayPattern = 0x0D;
                    break;
                case "รถ-ฉุกเฉิน":
                    bufferDisplayPattern = 0x0E;
                    break;
                case "แก้ไขประเภทรถ":
                    bufferDisplayPattern = 0x0F;
                    break;
                case "รถ-ลากจูง":
                    bufferDisplayPattern = 0x10;
                    break;
                case "โปรดรอสักครู่":
                    bufferDisplayPattern = 0x11;
                    break;
                case "ไม่มีบัตร ทางด่วน":
                    bufferDisplayPattern = 0x12;
                    break;
                case "บัตรอ่านไม่ได้":
                    bufferDisplayPattern = 0x13;
                    break;
                case "บัตรผิดปกติ":
                    bufferDisplayPattern = 0x14;
                    break;
                case "บัตรถูกอายัด":
                    bufferDisplayPattern = 0x15;
                    break;
                case "โปรดเติมเงิน":
                    bufferDisplayPattern = 0x16;
                    break;
                case "แบตเตอรี่อ่อน":
                    bufferDisplayPattern = 0x17;
                    break;
                case "ยอดเงินไม่พอ":
                    bufferDisplayPattern = 0x18;
                    break;
                case "เติมเงินทางด่วน!":
                    bufferDisplayPattern = 0x19;
                    break;
                case "ผิดเส้นทาง":
                    bufferDisplayPattern = 0x1A;
                    break;
                case "ซ่อมบำรุง":
                    bufferDisplayPattern = 0x1B;
                    break;
                case "ติดต่อจนท.":
                    bufferDisplayPattern = 0x1C;
                    break;
                case "หลอด LED ติดทุกดวง":
                    bufferDisplayPattern = 0x1D;
                    break;        
            }
            return bufferDisplayPattern;
        }
        void intitialWindows()
        {
            instructionCombo.Enabled = false;        
            updateBtn.Enabled = false;
            clearBtn.Enabled = false;
            
        }
        void setClassDisplay()
        {
            byte[] buffer = new byte[15];
            //get class data from classTextbox
            buffer = Encoding.ASCII.GetBytes(classTextbox.Text);
            for (int i = 0; i < buffer.Length; i++)
            {
                datapackage[i + 4] = buffer[i];
            }
        }
        void setPrice()
        {
            //get price data from priceTextbox
            byte[] buffer = new byte[15];
            for (int j = 6 - priceTextbox.Text.Count(); j > 0; j--)
            {
                priceTextbox.Text = " " + priceTextbox.Text;
            }
            buffer = Encoding.ASCII.GetBytes(priceTextbox.Text);
            for (int i = 0; i < buffer.Length; i++)
            {
                datapackage[i + 5] = buffer[i];
            }
        }
        byte[] setDisplayLine1()
        {
            byte[] bufferLine1 = new byte[12];
            //get Balance from firstTextbox
            for (int j = 12 - firstTextbox.Text.Count(); j > 0; j--)
            {
                firstTextbox.Text = " " + firstTextbox.Text;
            }
            bufferLine1 = Encoding.ASCII.GetBytes(firstTextbox.Text);
            
            return bufferLine1;
        }
        byte[] setDisplayLine2()
        {
            byte[] bufferLine2 = new byte[12];
            for (int j = 12 - secondTextbox.Text.Count(); j > 0; j--)
            {
                secondTextbox.Text = " " + secondTextbox.Text;
            }
            bufferLine2 = Encoding.ASCII.GetBytes(secondTextbox.Text);
            return bufferLine2;  
        }
        void setProtocol()
        {
            datapackage[0] = 0x02;
            switch (instructionCombo.Text)
            {
                case "Reset Hardware Command":
                    datapackage[1] = 0x00;
                    datapackage[2] = checkSum(datapackage, 2);
                    datapackage[3] = 0x03;
                    break;
                case "Clear Display Command":
                    datapackage[1] = 0x02;
                    datapackage[2] = checkSum(datapackage, 2);
                    datapackage[3] = 0x03;
                    break;
                case "Alive Check Command":
                    datapackage[1] = 0x03;
                    datapackage[2] = checkSum(datapackage, 2);
                    datapackage[3] = 0x03;
                    break;
                case "LED Check Command":
                    datapackage[1] = 0x04;
                    datapackage[2] = checkSum(datapackage, 2);
                    datapackage[3] = 0x03;
                    break;
                case "Version Check Command":
                    datapackage[1] = 0x09;
                    datapackage[2] = checkSum(datapackage, 2);
                    datapackage[3] = 0x03;
                    break;
                case "Brightness Adjust Command":
                    datapackage[1] = 0x0B;
                    switch (brightnessCombo.Text)
                    {
                        case "High":
                            datapackage[2] = 0x48;
                            break;
                        case "Medium":
                            datapackage[2] = 0x4D;
                            break;
                        case "Low":
                            datapackage[2] = 0x4C;
                            break;
                    }
                    break;
                case "Display Numeric Command":
                    displayGroupBox.Enabled = true;
                    datapackage[1] = 0x01;
                    setLTL();
                    setClassDisplay();
                    setPrice();
                    byte[] line1 = new byte[12];
                    line1 = setDisplayLine1();
                    for (int i = 0; i <line1.Length; i++)
                    {
                        datapackage[i + 11] = line1[i];
                    }
                    byte[] line2= new byte[12];
                    line2 = setDisplayLine2();
                    for (int i = 0; i < line2.Length; i++)
                    {
                        datapackage[i + 23] = line2[i];
                    }
                    datapackage[35] = checkSum(datapackage, 35);
                    datapackage[36] = 0x03;
                    break;
                case "Display Pattern Command":
                    datapackage[1] = 0x07;
                    setLTL();
                    datapackage[3] = 0x00;
                    setClassDisplay();
                    setPrice();
                    datapackage[11] = setdisplaypattern(patternDisplayCombo);
                    datapackage[12] = checkSum(datapackage, 12);
                    datapackage[13] = 0x03;
                    break;
                case "Display Custom Command":           
                    datapackage[1] = 0x81;
                    setLTL();
                    datapackage[3] = 0x00;
                    setClassDisplay();
                    setPrice();
                    if (radioSingleLine.Checked)
                    {
                        datapackage[11] = 0x07;
                        datapackage[12] = setdisplaypattern(patternDisplayCombo);
                        for (int i = 13; i < 35; i++)
                        {
                            datapackage[i] = 0x20;
                        }

                    }
                    else if (radioDoubleline.Checked)
                    {
                        if (radioPattern1.Checked)
                        {
                            datapackage[11] = 0x10;
                            datapackage[12] = setdisplaypattern(firstLinecombo);
                            for (int i = 13; i < 23; i++)
                            {
                                datapackage[i] = 0x20;
                            }
                        }
                        else
                        {
                            byte[] doubleline1 = new byte[12];
                            doubleline1 = setDisplayLine1();
                            for (int i = 0; i < doubleline1.Length; i++)
                            {
                                datapackage[i + 11] = doubleline1[i];
                            } 
                        }
                        if (radioPattern2.Checked)
                        {
                            datapackage[23] = 0x10;
                            datapackage[24] = setdisplaypattern(secondLinecombo);
                            for (int i = 25; i < 35; i++)
                            {
                                datapackage[i] = 0x20;
                            }
                        }
                        else 
                        {
                            byte[] doubleline2 = new byte[12];
                            doubleline2 = setDisplayLine2();
                            for (int i = 0; i < doubleline2.Length; i++)
                            {
                                datapackage[i + 23] = doubleline2[i];
                            }
                        } 
                    }
                    /*string debug = "";
                    for (int i = 0; i < 35; i++)
                    {
                        debug += "byte" + i.ToString() +" : "+datapackage[i].ToString("X2") + "  ";
                    }
                    MessageBox.Show(debug);*/
                    datapackage[35] = checkSum(datapackage, 35);
                    datapackage[36] = 0x03;
                    break;
                case "LTL Command":
                    datapackage[1] = 0x82;
                    setLTL();
                    datapackage[3] = checkSum(datapackage,3);
                    datapackage[4] = 0x03;
                    break;

            }
        }
        void checkReply(ComboBox select, byte[] reply)
        {
            switch (select.Text)
            {
                case "Version Check Command":
                    string version = "";
                    if (reply[0] == 0x02 && reply[1] == 0x09)
                    {
                        if (reply[5] == checkSum(reply, 5))
                        {
                            version = Encoding.ASCII.GetString(
                                reply, 2, 3);
                            replyTextbox.Text = version;
                        }
                    }
                    break;
                case "LED Check Command":
                    if (reply[0] == 0x02 && reply[1] == 0x06)
                    {
                        if (reply[2] == checkSum(reply, 2))
                        {
                            replyTextbox.Text = "รูปแบบถูกต้อง";
                        }
                    }
                    else if (reply[0] == 0x02 && reply[1] == 0x15)
                    {
                        if (reply[2] == checkSum(reply, 2))
                        {
                            replyTextbox.Text = "รูปแบบไม่ถูกต้อง";
                        }
                    }
                    break;
            }   
        }
        byte checkSum(byte[] dataArray,int len)
        {
            byte checkSumBuffer = 0x00;
            for(int i=0; i<len;i++ )
            {
                checkSumBuffer ^= dataArray[i];
            }
            return checkSumBuffer;
        }      
        private void Form1_Load(object sender, EventArgs e)
        {           
            intitialWindows();
            displayGroupBox.Enabled = false;
            groupboxEthernet.Enabled = true;
            groupBox1.Enabled = false;
            intitialPorts();
            getAvailablePorts();
            radioEthernet.Checked = true;
            clientCheck = false;
        }
  
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                 if (string.IsNullOrEmpty(portNameComboBox.Text))
                {
                    MessageBox.Show("Please Select Port Name or Check Seria port " );
                }
                else
                {
                    serialPort.PortName = portNameComboBox.Text;
                    serialPort.BaudRate = Convert.ToInt32(baudRateComboBox.Text);
                    serialPort.DataBits = Convert.ToInt32(dataBitsComboBox.Text);
                    switch (parityComboBox.Text)
                    {
                        case "None": serialPort.Parity = Parity.None; break;
                        case "Odd": serialPort.Parity = Parity.Odd; break;
                        case "Even": serialPort.Parity = Parity.Even; break;
                        case "Mark": serialPort.Parity = Parity.Mark; break;
                    }
                    switch (stopBitsComboBox.Text)
                    {
                        case "0": serialPort.StopBits = StopBits.None; break;
                        case "1": serialPort.StopBits = StopBits.One; break;
                        case "2": serialPort.StopBits = StopBits.Two; break;
                        default: break;
                    }
                    serialPort.Open();
                    progressBar1.Value = 100;
                }
               
                if (serialPort.IsOpen)
                {
                    instructionCombo.Enabled = true;
                    updateBtn.Enabled = true;
                    clearBtn.Enabled = true;
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;  
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Unauthorized Access");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            serialPort.Close();
            progressBar1.Value = 0;
            instructionCombo.Enabled = false;
            displayGroupBox.Enabled = false;
            brightnessGroup.Enabled = false;
            ReplyGroupbox.Enabled = false;
            updateBtn.Enabled = false;
            clearBtn.Enabled = false;
            btnStop.Enabled = false;
            btnStart.Enabled = true;

        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            setProtocol();
            if (radioEthernet.Checked)
            {
               client.Client.Close();
               client = null;
               client = new TcpClient();
               client.Connect(iPAddress, port);
               if (client.Connected == true)
               {
                  client.Client.Send(datapackage);                
               }
               else
               {
                  listBox1.Items.Add(client.Connected);
               }        
            }
            else
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Write(datapackage, 0, datapackage.Length);
                }
            }
            if (instructionCombo.Text == "Version Check Command")
            {
                byte[] replyVersion = new byte[6];
                if (radioSerial.Checked)
                {
                    serialPort.Read(replyVersion, 0, 6);         
                }
                else
                {
                    client.Client.Receive(replyVersion);
                }
                checkReply(instructionCombo, replyVersion);
            }
            else if (instructionCombo.Text == "LED Check Command")
            {
                byte[] replyLED = new byte[4];
                serialPort.Read(replyLED, 0, 4);
                checkReply(instructionCombo, replyLED);
            }

        
        }

        private void instructionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (instructionCombo.Items[instructionCombo.SelectedIndex].
                ToString() == "Reset Hardware Command")
            {
                displayGroupBox.Enabled = false;
                brightnessGroup.Enabled = false;
                ReplyGroupbox.Enabled = false;
            }
            else if (instructionCombo.Items[instructionCombo.SelectedIndex].
                ToString() == "Clear Display Command")
            {
                displayGroupBox.Enabled = false;
                brightnessGroup.Enabled = false;
                ReplyGroupbox.Enabled = false;
            }

            else if (instructionCombo.Items[instructionCombo.SelectedIndex].
                ToString() == "Alive Check Command")
            {
                displayGroupBox.Enabled = false;
                brightnessGroup.Enabled = false;
                ReplyGroupbox.Enabled = true;
            }
            else if (instructionCombo.Items[instructionCombo.SelectedIndex].
                ToString() == "LED Check Command")
            {
                displayGroupBox.Enabled = false;
                brightnessGroup.Enabled = false;
                ReplyGroupbox.Enabled = true;
            }
            else if (instructionCombo.Items[instructionCombo.SelectedIndex].
                ToString() == "Version Check Command")
            {
                displayGroupBox.Enabled = false;
                brightnessGroup.Enabled = false;
                ReplyGroupbox.Enabled = true;
            }
            else if (instructionCombo.Items[instructionCombo.SelectedIndex].
                ToString() == "Display Numeric Command")
            {
                displayGroupBox.Enabled = true;
                statusCombobox.Enabled = true;
                statusCombobox.SelectedIndex = 0;
                patternDisplayCombo.Enabled = false;
                classTextbox.Enabled = true;
                classTextbox.Clear();
                priceTextbox.Enabled = true;
                priceTextbox.Clear();
                customGroup.Enabled = false;
                doubleLineGroup.Enabled = true;
                doublegroupBox.Enabled = true;
                firstLinecombo.Visible = false;
                secondLinecombo.Visible = false;
                firstTextbox.Visible = true;
                firstTextbox.Enabled = true;
                firstTextbox.Clear();
                secondTextbox.Visible = true;
                secondTextbox.Enabled = true;
                secondTextbox.Clear();
                firstradio.Enabled = false;
                secondradio.Enabled = false;
                brightnessGroup.Enabled = false;
                ReplyGroupbox.Enabled = false;
            }
            else if (instructionCombo.Items[instructionCombo.SelectedIndex].
                ToString() == "Display Pattern Command")
            {
                displayGroupBox.Enabled = true;
                patternDisplayCombo.Enabled = true;
                doublegroupBox.Enabled = false;
                customGroup.Enabled = false;
                classTextbox.Enabled = true;
                classTextbox.Clear();
                priceTextbox.Enabled = true;
                priceTextbox.Clear();
                ReplyGroupbox.Enabled = false;
                firstradio.Enabled = false;
                secondradio.Enabled = false;
            }
            else if (instructionCombo.Items[instructionCombo.
                SelectedIndex].ToString() == "Display Custom Command")
            {
                displayGroupBox.Enabled = true;
                statusCombobox.Enabled = true;
                statusCombobox.SelectedIndex = 0;
                patternDisplayCombo.Enabled = true;
                patternDisplayCombo.SelectedIndex = 0;
                classTextbox.Enabled = true;
                classTextbox.Clear();
                priceTextbox.Enabled = true;
                priceTextbox.Clear();
                customGroup.Enabled = true;
                radioSingleLine.Checked = true;
                radioDoubleline.Checked = false;
                doubleLineGroup.Enabled = false;
                brightnessGroup.Enabled = false;
                ReplyGroupbox.Enabled = false;
            }
            else if (instructionCombo.Items[instructionCombo.
                SelectedIndex].ToString() == "LTL Command")
            {
                displayGroupBox.Enabled = true;
                statusCombobox.Enabled = true;
                statusCombobox.SelectedIndex = 0;
                patternDisplayCombo.Enabled = false;           
                classTextbox.Enabled = false;
                priceTextbox.Enabled = false;
                customGroup.Enabled = false;             
                doubleLineGroup.Enabled = false;
                brightnessGroup.Enabled = false;
                ReplyGroupbox.Enabled = false;
            }
            else if (instructionCombo.Items[instructionCombo.
                SelectedIndex].ToString() == "Brightness Adjust Command")
            {
                displayGroupBox.Enabled = false;                             
                brightnessGroup.Enabled = true;
                ReplyGroupbox.Enabled = false;
            }
        }

        private void radioDoubleline_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDoubleline.Checked)
            {
                doubleLineGroup.Enabled = true;
                doublegroupBox.Enabled = true;
                firstTextbox.Visible = true;
                firstTextbox.Enabled = true;
                firstTextbox.Clear();
                firstLinecombo.Visible = false;
                secondTextbox.Visible = true;
                secondTextbox.Enabled = true;
                secondTextbox.Clear();
                secondLinecombo.Visible = false;
                firstradio.Enabled = true;
                secondradio.Enabled = true;
                radioPattern1.Checked = false;
                radioNumber1.Checked = true;
                radioPattern2.Checked = false;
                radioNumber2.Checked = true;
                patternDisplayCombo.Enabled = false;
            }
            else
            {
                doubleLineGroup.Enabled = false;
                patternDisplayCombo.Enabled = true;
            }
        }
        private void radioPattern1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioPattern1.Checked)
            {
                firstTextbox.Visible = false;
                firstLinecombo.Visible = true;
                firstLinecombo.SelectedIndex = 0;
            }
            else
            {
                firstTextbox.Visible = true;
                firstLinecombo.Visible = false;
                firstTextbox.Clear();
            }
        }

        private void radioPattern2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioPattern2.Checked)
            {
                secondTextbox.Visible = false;
                secondLinecombo.Visible = true;
                secondLinecombo.SelectedIndex = 0;
            }
            else
            {
                secondTextbox.Visible = true;
                secondLinecombo.Visible = false;
                secondTextbox.Clear();
            }
        }

        private void radioSerial_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSerial.Checked)
            {
                /*if (clientCheck)
                {
                    client.Close();
                }*/
                groupboxEthernet.Enabled = false;
                groupBox1.Enabled = true;
                displayGroupBox.Enabled = false;
                instructionCombo.Enabled = false;
                updateBtn.Enabled = false;
                clearBtn.Enabled = false;
                btnStart.Enabled = true;
                btnStop.Enabled = false;    
            }
            else
            {
                button1.Enabled = true;
                serialPort.Close();
                progressBar1.Enabled = false;
                progressBar1.Value = 0;
                instructionCombo.Enabled = false;
                updateBtn.Enabled = false;
                clearBtn.Enabled = false;
                groupboxEthernet.Enabled = true;
                groupBox1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            iPAddress = IPAddress.Parse(hostTextBox.Text);
            port = int.Parse(portTextBox.Text);
            client = null;
           // client.Client = null;
            client = new TcpClient();
            client.Connect(iPAddress, port);
            if (client.Connected)
            {
                clientCheck = true;
                MessageBox.Show("Connected");
                instructionCombo.Enabled = true;
                updateBtn.Enabled = true;
                clearBtn.Enabled = true;
                btnStart.Enabled = false;
                btnStop.Enabled = true;  
                button1.Enabled = false;
            }
            
        }
        private void button2_Click(object sender, EventArgs e)
        {
            clientCheck = false;
            client.Close();
            button2.Enabled = false;
            instructionCombo.Enabled = true;
            updateBtn.Enabled = true;
            clearBtn.Enabled = true;
            displayGroupBox.Enabled = false;
            button1.Enabled = true;

        }

        private void radioEthernet_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Close();
            client.Dispose();
        }
    }
}
