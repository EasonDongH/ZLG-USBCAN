using System;
using System.Collections.Generic;
using System.Windows.Forms;



namespace WindowsApplication1
{
    public partial class Form1 : Form
    {



        //static UInt32 m_devtype = 4;//USBCAN2
        static UInt32 m_devtype = 21;//USBCAN-2e-u
        //usb-e-u 波特率
        static UInt32[] GCanBrTab = new UInt32[10]{
                    0x060003, 0x060004, 0x060007,
                        0x1C0008, 0x1C0011, 0x160023,
                        0x1C002C, 0x1600B3, 0x1C00E0,
                        0x1C01C1
                };
        ////////////////////////////////////////
        const UInt32 STATUS_OK = 1;

        UInt32 m_bOpen = 0;
        UInt32 m_devind = 0;
        UInt32 m_canind = 0;

        VCI_CAN_OBJ[] m_recobj = new VCI_CAN_OBJ[50];

        UInt32[] m_arrdevtype = new UInt32[20];

        ControlCAN controlCAN;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox_DevIndex.SelectedIndex = 0;
            comboBox_CANIndex.SelectedIndex = 0;
            textBox_AccCode.Text = "00000000";
            textBox_AccMask.Text = "FFFFFFFF";
            textBox_Time0.Text = "00";
            textBox_Time1.Text = "14";
            comboBox_Filter.SelectedIndex = 1;
            comboBox_Mode.SelectedIndex = 0;
            comboBox_SendType.SelectedIndex = 2;
            comboBox_FrameFormat.SelectedIndex = 0;
            comboBox_FrameType.SelectedIndex = 0;
            textBox_ID.Text = "00000123";
            textBox_Data.Text = "00 01 02 03 04 05 06 07 ";

            comboBox_e_u_baud.SelectedIndex = 0;//1000Kbps
            comboBox_e_u_Filter.SelectedIndex = 2;//禁用
            textBox_e_u_startid.Text = "1";
            textBox_e_u_endid.Text = "ff";

            //
            Int32 curindex = 0;
            comboBox_devtype.Items.Clear();

            curindex = comboBox_devtype.Items.Add("USBCAN_E_U");
            m_arrdevtype[curindex] = (uint)CANDevice.VCI_USBCAN_E_U;

            curindex = comboBox_devtype.Items.Add("USBCAN_2E_U");
            m_arrdevtype[curindex] = (uint)CANDevice.VCI_USBCAN_2E_U;

            curindex = comboBox_devtype.Items.Add("PCI5010U");
            m_arrdevtype[curindex] = (uint)CANDevice.VCI_PCI5010U;

            curindex = comboBox_devtype.Items.Add("PCI5020U");
            m_arrdevtype[curindex] = (uint)CANDevice.VCI_PCI5020U;

            comboBox_devtype.SelectedIndex = 3;
            comboBox_devtype.MaxDropDownItems = comboBox_devtype.Items.Count;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_bOpen == 1)
            {
                controlCAN.CloseDevice();
            }
        }

        unsafe private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 1)
            {
                controlCAN.CloseDevice();
                m_bOpen = 0;
            }
            else
            {
                m_devtype = m_arrdevtype[comboBox_devtype.SelectedIndex];

                m_devind = (UInt32)comboBox_DevIndex.SelectedIndex;
                m_canind = (UInt32)comboBox_CANIndex.SelectedIndex;
                this.controlCAN = new ControlCAN(m_devtype, m_devind, m_canind);
                if (!controlCAN.OpenDevice())
                {
                    MessageBox.Show("打开设备失败,请检查设备类型和设备索引号是否正确", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                //USB-E-U 代码
                UInt32 baud;
                baud = GCanBrTab[comboBox_e_u_baud.SelectedIndex];
                if (controlCAN.SetBaud(baud) != STATUS_OK)
                {
                    MessageBox.Show("设置波特率错误，打开设备失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    controlCAN.CloseDevice();
                    return;
                }
                //滤波设置
                //////////////////////////////////////////////////////////////////////////
                m_bOpen = 1;
                VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
                config.AccCode = System.Convert.ToUInt32("0x" + textBox_AccCode.Text, 16);
                config.AccMask = System.Convert.ToUInt32("0x" + textBox_AccMask.Text, 16);
                config.Timing0 = System.Convert.ToByte("0x" + textBox_Time0.Text, 16);
                config.Timing1 = System.Convert.ToByte("0x" + textBox_Time1.Text, 16);
                config.Filter = (Byte)comboBox_Filter.SelectedIndex;
                config.Mode = (Byte)comboBox_Mode.SelectedIndex;
                controlCAN.InitCAN(config);

                //////////////////////////////////////////////////////////////////////////
                Int32 filterMode = comboBox_e_u_Filter.SelectedIndex;
                if (2 != filterMode)//不是禁用
                {
                    VCI_FILTER_RECORD filterRecord = new VCI_FILTER_RECORD();
                    filterRecord.ExtFrame = (UInt32)filterMode;
                    filterRecord.Start = System.Convert.ToUInt32("0x" + textBox_e_u_startid.Text, 16);
                    filterRecord.End = System.Convert.ToUInt32("0x" + textBox_e_u_endid.Text, 16);

                    if (controlCAN.SetFilter(filterRecord) != STATUS_OK)
                    {
                        MessageBox.Show("设置滤波失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        controlCAN.CloseDevice();
                        return;
                    }
                }
                //////////////////////////////////////////////////////////////////////////
            }
            buttonConnect.Text = m_bOpen == 1 ? "断开" : "连接";
            timer_rec.Enabled = m_bOpen == 1 ? true : false;
        }

        unsafe private void timer_rec_Tick(object sender, EventArgs e)
        {
            UInt32 res = new UInt32();
            res = controlCAN.GetReceiveNum();
            if (res == 0)
                return;
            //res = VCI_Receive(m_devtype, m_devind, m_canind, ref m_recobj[0],50, 100);

            /////////////////////////////////////
            //UInt32 con_maxlen = 50;
            List<VCI_CAN_OBJ> receive = controlCAN.Receive();
            ////////////////////////////////////////////////////////

            String str = "";
            foreach (var obj in receive)
            {
                str = "接收到数据: ";
                str += "  帧ID:0x" + System.Convert.ToString((Int32)obj.ID, 16);
                str += "  帧格式:";
                if (obj.RemoteFlag == 0)
                    str += "数据帧 ";
                else
                    str += "远程帧 ";
                if (obj.ExternFlag == 0)
                    str += "标准帧 ";
                else
                    str += "扩展帧 ";

                //////////////////////////////////////////
                if (obj.RemoteFlag == 0)
                {
                    str += "数据: ";
                    byte len = (byte)(obj.DataLen % 9);
                    byte j = 0;
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[0], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[1], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[2], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[3], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[4], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[5], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[6], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[7], 16);

                }
                listBox_Info.Items.Add(str);
                listBox_Info.SelectedIndex = listBox_Info.Items.Count - 1;
            }
        }

        private void button_StartCAN_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 0)
                return;
            controlCAN.StartCAN();
        }

        private void button_StopCAN_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 0)
                return;
           controlCAN.ResetCAN();
        }

        unsafe private void button_Send_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 0)
                return;

            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            //sendobj.Init();
            sendobj.SendType = (byte)comboBox_SendType.SelectedIndex;
            sendobj.RemoteFlag = (byte)comboBox_FrameFormat.SelectedIndex;
            sendobj.ExternFlag = (byte)comboBox_FrameType.SelectedIndex;
            sendobj.ID = System.Convert.ToUInt32("0x" + textBox_ID.Text, 16);
            int len = (textBox_Data.Text.Length + 1) / 3;
            sendobj.DataLen = System.Convert.ToByte(len);
            String strdata = textBox_Data.Text;
            int i = -1;
            if (i++ < len - 1)
                sendobj.Data[0] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[1] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[2] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[3] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[4] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[5] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[6] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[7] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);

            int nTimeOut = 3000;
            controlCAN.SetTimeOut(nTimeOut);
            if (controlCAN.Transmit(sendobj) != true)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


    }
}