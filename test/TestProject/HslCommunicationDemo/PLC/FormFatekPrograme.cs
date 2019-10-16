﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Profinet;
using System.Threading;
using HslCommunication.Profinet.FATEK;
using HslCommunication;

namespace HslCommunicationDemo
{
    public partial class FormFatekPrograme : Form
    {
        public FormFatekPrograme( )
        {
            InitializeComponent( );
            fatekProgram = new FatekProgram( );
        }


        private FatekProgram fatekProgram = null;

        private void FormSiemens_Load( object sender, EventArgs e )
        {
            panel2.Enabled = false;
            comboBox1.SelectedIndex = 2;

            Language( Program.Language );
        }


        private void Language( int language )
        {
            if (language == 2)
            {
                Text = "FATEK Read PLC Demo";

                label1.Text = "parity:";
                label3.Text = "Stop bits";
                label27.Text = "Com:";
                label26.Text = "BaudRate";
                label25.Text = "Data bits";
                button1.Text = "Connect";
                button2.Text = "Disconnect";
                label21.Text = "Address:";
                label6.Text = "address:";
                label7.Text = "result:";

                button_read_bool.Text = "Read Bit";
                label23.Text = "X,Y,M,L,V,B";
                button_read_short.Text = "r-short";
                button_read_ushort.Text = "r-ushort";
                button_read_int.Text = "r-int";
                button_read_uint.Text = "r-uint";
                button_read_long.Text = "r-long";
                button_read_ulong.Text = "r-ulong";
                button_read_float.Text = "r-float";
                button_read_double.Text = "r-double";
                button_read_string.Text = "r-string";
                label8.Text = "length:";
                label11.Text = "Address:";
                label12.Text = "length:";
                button25.Text = "Bulk Read";
                label13.Text = "Results:";
                label16.Text = "Message:";
                label14.Text = "Results:";
                button26.Text = "Read";

                label10.Text = "Address:";
                label9.Text = "Value:";
                label19.Text = "Note: The value of the string needs to be converted";
                button24.Text = "Write Bit";
                button22.Text = "w-short";
                button21.Text = "w-ushort";
                button20.Text = "w-int";
                button19.Text = "w-uint";
                button18.Text = "w-long";
                button17.Text = "w-ulong";
                button16.Text = "w-float";
                button15.Text = "w-double";
                button14.Text = "w-string";

                groupBox1.Text = "Single Data Read test";
                groupBox2.Text = "Single Data Write test";
                groupBox3.Text = "Bulk Read test";
                groupBox4.Text = "Message reading test, hex string needs to be filled in";

                label24.Text = "X,Y,M,L,V,B";
                comboBox1.DataSource = new string[] { "None", "Odd", "Even" };
            }
        }

        private void FormSiemens_FormClosing( object sender, FormClosingEventArgs e )
        {

        }
        

        #region Connect And Close



        private void button1_Click( object sender, EventArgs e )
        {
            if (!int.TryParse( textBox2.Text, out int baudRate ))
            {
                MessageBox.Show( DemoUtils.BaudRateInputWrong );
                return;
            }

            if (!int.TryParse( textBox16.Text, out int dataBits ))
            {
                MessageBox.Show( DemoUtils.DataBitsInputWrong );
                return;
            }

            if (!int.TryParse( textBox17.Text, out int stopBits ))
            {
                MessageBox.Show( DemoUtils.StopBitInputWrong );
                return;
            }
            

            fatekProgram?.Close( );
            fatekProgram = new FatekProgram( );
            
            try
            {
                fatekProgram.SerialPortInni( sp =>
                {
                    sp.PortName = textBox1.Text;
                    sp.BaudRate = baudRate;
                    sp.DataBits = dataBits;
                    sp.StopBits = stopBits == 0 ? System.IO.Ports.StopBits.None : (stopBits == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.Two);
                    sp.Parity = comboBox1.SelectedIndex == 0 ? System.IO.Ports.Parity.None : (comboBox1.SelectedIndex == 1 ? System.IO.Ports.Parity.Odd : System.IO.Ports.Parity.Even);
                } );
                fatekProgram.Station = byte.Parse( textBox15.Text );


                fatekProgram.Open( );
                button2.Enabled = true;
                button1.Enabled = false;
                panel2.Enabled = true;

                userControlCurve1.ReadWriteNet = fatekProgram;
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button2_Click( object sender, EventArgs e )
        {
            // 断开连接
            fatekProgram.Close( );
            button2.Enabled = false;
            button1.Enabled = true;
            panel2.Enabled = false;
        }

        

        #endregion

        #region 单数据读取测试


        private void button_read_bool_Click( object sender, EventArgs e )
        {
            // 读取bool变量
            DemoUtils.ReadResultRender( fatekProgram.ReadBool( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_short_Click( object sender, EventArgs e )
        {
            // 读取short变量
            DemoUtils.ReadResultRender( fatekProgram.ReadInt16( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_ushort_Click( object sender, EventArgs e )
        {
            // 读取ushort变量
            DemoUtils.ReadResultRender( fatekProgram.ReadUInt16( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_int_Click( object sender, EventArgs e )
        {
            // 读取int变量
            DemoUtils.ReadResultRender( fatekProgram.ReadInt32( textBox3.Text ), textBox3.Text, textBox4 );
        }
        private void button_read_uint_Click( object sender, EventArgs e )
        {
            // 读取uint变量
            DemoUtils.ReadResultRender( fatekProgram.ReadUInt32( textBox3.Text ), textBox3.Text, textBox4 );
        }
        private void button_read_long_Click( object sender, EventArgs e )
        {
            // 读取long变量
            DemoUtils.ReadResultRender( fatekProgram.ReadInt64( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_ulong_Click( object sender, EventArgs e )
        {
            // 读取ulong变量
            DemoUtils.ReadResultRender( fatekProgram.ReadUInt64( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_float_Click( object sender, EventArgs e )
        {
            // 读取float变量
            DemoUtils.ReadResultRender( fatekProgram.ReadFloat( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_double_Click( object sender, EventArgs e )
        {
            // 读取double变量
            DemoUtils.ReadResultRender( fatekProgram.ReadDouble( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_string_Click( object sender, EventArgs e )
        {
            // 读取字符串
            DemoUtils.ReadResultRender( fatekProgram.ReadString( textBox3.Text, ushort.Parse( textBox5.Text ) ), textBox3.Text, textBox4 );
        }


        #endregion

        #region 单数据写入测试


        private void button24_Click( object sender, EventArgs e )
        {
            // bool写入
            DemoUtils.WriteResultRender( () => fatekProgram.Write( textBox8.Text,bool.Parse( textBox7.Text )), textBox8.Text );
        }

        private void button22_Click( object sender, EventArgs e )
        {
            // short写入
            DemoUtils.WriteResultRender( () => fatekProgram.Write( textBox8.Text, short.Parse( textBox7.Text ) ), textBox8.Text );
        }

        private void button21_Click( object sender, EventArgs e )
        {
            // ushort写入
            DemoUtils.WriteResultRender( () => fatekProgram.Write( textBox8.Text, ushort.Parse( textBox7.Text ) ), textBox8.Text );
        }


        private void button20_Click( object sender, EventArgs e )
        {
            // int写入
            DemoUtils.WriteResultRender( () => fatekProgram.Write( textBox8.Text, int.Parse( textBox7.Text ) ), textBox8.Text );
        }

        private void button19_Click( object sender, EventArgs e )
        {
            // uint写入
            DemoUtils.WriteResultRender( () => fatekProgram.Write( textBox8.Text, uint.Parse( textBox7.Text ) ), textBox8.Text );
        }

        private void button18_Click( object sender, EventArgs e )
        {
            // long写入
            DemoUtils.WriteResultRender( () => fatekProgram.Write( textBox8.Text, long.Parse( textBox7.Text ) ), textBox8.Text );
        }

        private void button17_Click( object sender, EventArgs e )
        {
            // ulong写入
            DemoUtils.WriteResultRender( () => fatekProgram.Write( textBox8.Text, ulong.Parse( textBox7.Text ) ), textBox8.Text );
        }

        private void button16_Click( object sender, EventArgs e )
        {
            // float写入
            DemoUtils.WriteResultRender( () => fatekProgram.Write( textBox8.Text, float.Parse( textBox7.Text ) ), textBox8.Text );
        }

        private void button15_Click( object sender, EventArgs e )
        {
            // double写入
            DemoUtils.WriteResultRender( () => fatekProgram.Write( textBox8.Text, double.Parse( textBox7.Text ) ), textBox8.Text );
        }


        private void button14_Click( object sender, EventArgs e )
        {
            // string写入
            DemoUtils.WriteResultRender( () => fatekProgram.Write( textBox8.Text, textBox7.Text ), textBox8.Text );
        }
        

        #endregion

        #region 批量读取测试

        private void button25_Click( object sender, EventArgs e )
        {
            DemoUtils.BulkReadRenderResult( fatekProgram, textBox6, textBox9, textBox10 );
        }



        #endregion

        #region 报文读取测试


        private void button26_Click( object sender, EventArgs e )
        {
            OperateResult<byte[]> read = fatekProgram.ReadBase( HslCommunication.BasicFramework.SoftBasic.HexStringToBytes( textBox13.Text ) );
            if (read.IsSuccess)
            {
                textBox11.Text = "Result：" + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( read.Content );
            }
            else
            {
                MessageBox.Show( "Read Failed：" + read.ToMessageShowString( ) );
            }
        }


        #endregion
        
        #region 测试使用

        private void test1()
        {
            OperateResult<bool[]> read = fatekProgram.ReadBool( "M100", 10 );
            if(read.IsSuccess)
            {
                bool m100 = read.Content[0];
                // and so on
                bool m109 = read.Content[9];
            }
            else
            {
                // failed
            }
        }
        

        private void test3( )
        {
            short d100_short = fatekProgram.ReadInt16( "D100" ).Content;
            ushort d100_ushort = fatekProgram.ReadUInt16( "D100" ).Content;
            int d100_int = fatekProgram.ReadInt32( "D100" ).Content;
            uint d100_uint = fatekProgram.ReadUInt32( "D100" ).Content;
            long d100_long = fatekProgram.ReadInt64( "D100" ).Content;
            ulong d100_ulong = fatekProgram.ReadUInt64( "D100" ).Content;
            float d100_float = fatekProgram.ReadFloat( "D100" ).Content;
            double d100_double = fatekProgram.ReadDouble( "D100" ).Content;
            // need to specify the text length
            string d100_string = fatekProgram.ReadString( "D100", 10 ).Content;
        }
        private void test4( )
        {
            fatekProgram.Write( "D100", (short)5 );
            fatekProgram.Write( "D100", (ushort)5 );
            fatekProgram.Write( "D100", 5 );
            fatekProgram.Write( "D100", (uint)5 );
            fatekProgram.Write( "D100", (long)5 );
            fatekProgram.Write( "D100", (ulong)5 );
            fatekProgram.Write( "D100", 5f );
            fatekProgram.Write( "D100", 5d );
            // length should Multiples of 2 
            fatekProgram.Write( "D100", "12345678" );
        }


        private void test5( )
        {
            OperateResult<byte[]> read = fatekProgram.Read( "D100", 10 );
            if(read.IsSuccess)
            {
                int count = fatekProgram.ByteTransform.TransInt32( read.Content, 0 );
                float temp = fatekProgram.ByteTransform.TransSingle( read.Content, 4 );
                short name1 = fatekProgram.ByteTransform.TransInt16( read.Content, 8 );
                string barcode = Encoding.ASCII.GetString( read.Content, 10, 10 );
            }
        }

        private void test6( )
        {
            OperateResult<UserType> read = fatekProgram.ReadCustomer<UserType>( "D100" );
            if (read.IsSuccess)
            {
                UserType value = read.Content;
            }
            // write value
            fatekProgram.WriteCustomer( "D100", new UserType( ) );

            fatekProgram.LogNet = new HslCommunication.LogNet.LogNetSingle( Application.StartupPath + "\\Logs.txt" );

        }


        #endregion

    }
}
