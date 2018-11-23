using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WindowsApplication1
{
    //1.ZLGCAN系列接口卡信息的数据类型。
    public struct VCI_BOARD_INFO
    {
        public UInt16 hw_Version;
        public UInt16 fw_Version;
        public UInt16 dr_Version;
        public UInt16 in_Version;
        public UInt16 irq_Num;
        public byte can_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] public byte[] str_Serial_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] str_hw_Type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved;
    }


    /////////////////////////////////////////////////////
    //2.定义CAN信息帧的数据类型。
    unsafe public struct VCI_CAN_OBJ  //使用不安全代码
    {
        public uint ID;
        public uint TimeStamp;
        public byte TimeFlag;
        public byte SendType;
        public byte RemoteFlag;//是否是远程帧
        public byte ExternFlag;//是否是扩展帧
        public byte DataLen;

        public fixed byte Data[8];

        public fixed byte Reserved[3];

    }
    ////2.定义CAN信息帧的数据类型。
    //public struct VCI_CAN_OBJ 
    //{
    //    public UInt32 ID;
    //    public UInt32 TimeStamp;
    //    public byte TimeFlag;
    //    public byte SendType;
    //    public byte RemoteFlag;//是否是远程帧
    //    public byte ExternFlag;//是否是扩展帧
    //    public byte DataLen;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    //    public byte[] Data;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    //    public byte[] Reserved;

    //    public void Init()
    //    {
    //        Data = new byte[8];
    //        Reserved = new byte[3];
    //    }
    //}

    //3.定义CAN控制器状态的数据类型。
    public struct VCI_CAN_STATUS
    {
        public byte ErrInterrupt;
        public byte regMode;
        public byte regStatus;
        public byte regALCapture;
        public byte regECCapture;
        public byte regEWLimit;
        public byte regRECounter;
        public byte regTECounter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved;
    }

    //4.定义错误信息的数据类型。
    public struct VCI_ERR_INFO
    {
        public UInt32 ErrCode;
        public byte Passive_ErrData1;
        public byte Passive_ErrData2;
        public byte Passive_ErrData3;
        public byte ArLost_ErrData;
    }

    //5.定义初始化CAN的数据类型
    public struct VCI_INIT_CONFIG
    {
        public UInt32 AccCode;
        public UInt32 AccMask;
        public UInt32 Reserved;
        public byte Filter;
        public byte Timing0;
        public byte Timing1;
        public byte Mode;
    }

    public struct CHGDESIPANDPORT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] szpwd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] szdesip;
        public Int32 desport;

        public void Init()
        {
            szpwd = new byte[10];
            szdesip = new byte[20];
        }
    }

    ///////// new add struct for filter /////////
    //typedef struct _VCI_FILTER_RECORD{
    //    DWORD ExtFrame;	//是否为扩展帧
    //    DWORD Start;
    //    DWORD End;
    //}VCI_FILTER_RECORD,*PVCI_FILTER_RECORD;
    public struct VCI_FILTER_RECORD
    {
        public UInt32 ExtFrame;
        public UInt32 Start;
        public UInt32 End;
    }

    public enum CANDevice
    {
        VCI_PCI5121 = 1,
        VCI_PCI9810 = 2,
        VCI_USBCAN1 = 3,
        VCI_USBCAN2 = 4,
        VCI_USBCAN2A = 4,
        VCI_PCI9820 = 5,
        VCI_CAN232 = 6,
        VCI_PCI5110 = 7,
        VCI_CANLITE = 8,
        VCI_ISA9620 = 9,
        VCI_ISA5420 = 10,
        VCI_PC104CAN = 11,
        VCI_CANETUDP = 12,
        VCI_CANETE = 12,
        VCI_DNP9810 = 13,
        VCI_PCI9840 = 14,
        VCI_PC104CAN2 = 15,
        VCI_PCI9820I = 16,
        VCI_CANETTCP = 17,
        VCI_PEC9920 = 18,
        VCI_PCI5010U = 19,
        VCI_USBCAN_E_U = 20,
        VCI_USBCAN_2E_U = 21,
        VCI_PCI5020U = 22,
        VCI_EG20T_CAN = 23
    }

    public enum Status
    {
        OK = 1
    }

    public class ControlCAN
    {
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadErrInfo(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_ERR_INFO pErrInfo);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadCANStatus(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_STATUS pCANStatus);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        [DllImport("controlcan.dll")]
        //static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        unsafe static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, byte* pData);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);
        //[DllImport("controlcan.dll")]
        //static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);
        [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);

        public uint DevType { get; set; }
        public uint DevId { get; set; }
        public uint CANIndex { get; set; }

        public ControlCAN(uint devType,uint devId,uint canIndex)
        {
            this.DevType = devType;
            this.DevId = devId;
            this.CANIndex = canIndex;
        }

        public void StartCAN()
        {
            VCI_StartCAN(this.DevType, this.DevId, this.CANIndex);
        }

        public void CloseDevice()
        {
            VCI_CloseDevice(this.DevType, this.DevId);
        }

        public void ResetCAN()
        {
            VCI_ResetCAN(this.DevType, this.DevId, this.CANIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devType"></param>
        /// <param name="devId"></param>
        /// <returns> true为成功，false为失败</returns>
        public bool OpenDevice()
        {
            return VCI_OpenDevice(this.DevType, this.DevId, 0) != 0;
        }

        unsafe public uint SetBaud(UInt32 baud)
        {
            return VCI_SetReference(this.DevType, this.DevId, this.CANIndex, 0, (byte*)&baud);
        }

        unsafe public uint SetFilter(VCI_FILTER_RECORD filterRecord)
        {
            //填充滤波表格
            VCI_SetReference(this.DevType, this.DevId, this.CANIndex, 1, (byte*)&filterRecord);
            //使滤波表格生效
            byte tm = 0;
            return VCI_SetReference(this.DevType, this.DevId, this.CANIndex, 2, &tm);
        }

        unsafe public void SetTimeOut(int nTimeOut)
        {
            VCI_SetReference(this.DevType, this.DevId, this.CANIndex, 4, (byte*)&nTimeOut);
        }

        public void InitCAN(VCI_INIT_CONFIG cfg)
        {
            VCI_InitCAN(this.DevType, this.DevId, this.CANIndex, ref cfg);
        }

        public UInt32 GetReceiveNum()
        {
            return VCI_GetReceiveNum(this.DevType, this.DevId, this.CANIndex);
        }

        public List<VCI_CAN_OBJ> Receive()
        {
            UInt32 res = new uint();
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)res);

            res = VCI_Receive(this.DevType, this.DevId, this.CANIndex, pt, res, 100);
            List<VCI_CAN_OBJ> ans = new List<VCI_CAN_OBJ>();
            for (UInt32 i = 0; i < res; i++)
            {
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                ans.Add(obj);
            }
            Marshal.FreeHGlobal(pt);

            return ans;
        }

        public bool Transmit(VCI_CAN_OBJ sendobj)
        {
            return VCI_Transmit(this.DevType, this.DevId, this.CANIndex, ref sendobj, 1) != 0;
        }
    }
}
