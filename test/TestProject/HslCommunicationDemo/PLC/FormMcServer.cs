﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Profinet;
using HslCommunication;
using HslCommunication.ModBus;
using System.Threading;

namespace HslCommunicationDemo
{
    public partial class FormMcServer : Form
    {
        public FormMcServer( )
        {
            InitializeComponent( );
        }



        private void FormSiemens_Load( object sender, EventArgs e )
        {
            panel2.Enabled = false;


            if(Program.Language == 2)
            {
                Text = "MC Virtual Server [data support, bool: x,y,m   word: x,y,m,d,w]";
                label3.Text = "port:";
                button1.Text = "Start Server";
                button11.Text = "Close Server";
                label11.Text = "This server is not a strict mc protocol and only supports perfect communication with HSL components.";
                label19.Text = "Note: The string of values needs to be converted to the corresponding data type";
                button4.Text = "Connecting Alien client";
                groupBox1.Text = "Single Data Read test";
                label6.Text = "Adderss:";
                label7.Text = "Result";
                button_read_bool.Text = "Read Bit";
                button_read_short.Text = "r-short";
                button_read_ushort.Text = "r-ushort";
                button_read_int.Text = "r-int";
                button_read_uint.Text = "r-uint";
                button_read_long.Text = "r-long";
                button_read_ulong.Text = "r-ulong";
                button_read_float.Text = "r-float";
                button_read_double.Text = "r-double";
                button_read_string.Text = "r-string";
                label8.Text = "length";

                label10.Text = "Address:";
                label9.Text = "Value";
                groupBox2.Text = "Single Data Write test";
                button24.Text = "w-Bit";
                button22.Text = "w-short";
                button21.Text = "w-ushort";
                button20.Text = "w-int";
                button19.Text = "w-uint";
                button18.Text = "w-long";
                button17.Text = "w-ulong";
                button16.Text = "w-float";
                button15.Text = "w-double";
                button14.Text = "w-string";

                button8.Text = "Load";
                button9.Text = "Save";
                button10.Text = "Timed writing";
                label1.Text = "log:";
                checkBox1.Text = "Display received data";
                label16.Text = "Client-Online:";
            }
        }
        
        
        
        private System.Windows.Forms.Timer timerSecond;

        private void FormSiemens_FormClosing( object sender, FormClosingEventArgs e )
        {
            mcNetServer?.ServerClose( );
        }

        /// <summary>
        /// 统一的读取结果的数据解析，显示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="address"></param>
        /// <param name="textBox"></param>
        private void readResultRender<T>( OperateResult<T> result, string address, TextBox textBox )
        {
            if (result.IsSuccess)
            {
                textBox.AppendText( DateTime.Now.ToString( "[HH:mm:ss] " ) + $"[{address}] {result.Content}{Environment.NewLine}" );
            }
            else
            {
                MessageBox.Show( result.ToString( ) );
            }
        }

        /// <summary>
        /// 统一的数据写入的结果显示
        /// </summary>
        /// <param name="result"></param>
        /// <param name="address"></param>
        private void writeResultRender( string address )
        {
            MessageBox.Show( DateTime.Now.ToString( "[HH:mm:ss] " ) + $"[{address}] Write Success" );
        }


        #region Server Start


        private HslCommunication.Profinet.Melsec.MelsecMcServer mcNetServer;

        private void button1_Click( object sender, EventArgs e )
        {
            if (!int.TryParse( textBox2.Text, out int port ))
            {
                MessageBox.Show( DemoUtils.PortInputWrong );
                return;
            }


            try
            {

                mcNetServer = new HslCommunication.Profinet.Melsec.MelsecMcServer( );                       // 实例化对象
                //s7NetServer.LogNet = new HslCommunication.LogNet.LogNetSingle( "logs.txt" );                  // 配置日志信息
                //s7NetServer.LogNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
                mcNetServer.OnDataReceived += BusTcpServer_OnDataReceived;
                
                mcNetServer.ServerStart( port );

                button1.Enabled = false;
                panel2.Enabled = true;
                button4.Enabled = true;
                button11.Enabled = true;

                timerSecond?.Dispose( );
                timerSecond = new System.Windows.Forms.Timer( );
                timerSecond.Interval = 1000;
                timerSecond.Tick += TimerSecond_Tick;
                timerSecond.Start( );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }


        private void button11_Click( object sender, EventArgs e )
        {
            // 停止服务
            mcNetServer?.ServerClose( );
            button1.Enabled = true;
            button11.Enabled = false;
        }

        private void TimerSecond_Tick( object sender, EventArgs e )
        {
            label15.Text = mcNetServer.OnlineCount.ToString( ) ;
        }

        private void BusTcpServer_OnDataReceived( object sender, byte[] receive )
        {
            if (!checkBox1.Checked) return;

            if (InvokeRequired)
            {
                BeginInvoke( new Action<object,byte[]>( BusTcpServer_OnDataReceived ), sender, receive );
                return;
            }

            textBox1.AppendText( "Received：" + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( receive, ' ' ) + Environment.NewLine );
        }

        /// <summary>
        /// 当有日志记录的时候，触发，将日志信息也在主界面进行输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogNet_BeforeSaveToFile( object sender, HslCommunication.LogNet.HslEventArgs e )
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke( new Action<object, HslCommunication.LogNet.HslEventArgs>( LogNet_BeforeSaveToFile ), sender, e );
                    return;
                }

                textBox1.AppendText( e.HslMessage.ToString( ) + Environment.NewLine );
            }
            catch
            {
                return;
            }
        }



        #endregion

        #region 单数据读取测试


        private void button_read_bool_Click( object sender, EventArgs e )
        {
            // 读取bool变量
            readResultRender( mcNetServer.ReadBool( textBox3.Text  ), textBox3.Text, textBox4 );
        }

        private void button_read_short_Click( object sender, EventArgs e )
        {
            // 读取short变量
            readResultRender( mcNetServer.ReadInt16( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_ushort_Click( object sender, EventArgs e )
        {
            // 读取ushort变量
            readResultRender( mcNetServer.ReadUInt16( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_int_Click( object sender, EventArgs e )
        {
            // 读取int变量
            readResultRender( mcNetServer.ReadInt32( textBox3.Text ), textBox3.Text, textBox4 );
        }
        private void button_read_uint_Click( object sender, EventArgs e )
        {
            // 读取uint变量
            readResultRender( mcNetServer.ReadUInt32( textBox3.Text ), textBox3.Text, textBox4 );
        }
        private void button_read_long_Click( object sender, EventArgs e )
        {
            // 读取long变量
            readResultRender( mcNetServer.ReadInt64( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_ulong_Click( object sender, EventArgs e )
        {
            // 读取ulong变量
            readResultRender( mcNetServer.ReadUInt64( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_float_Click( object sender, EventArgs e )
        {
            // 读取float变量
            readResultRender( mcNetServer.ReadFloat( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_double_Click( object sender, EventArgs e )
        {
            // 读取double变量
            readResultRender( mcNetServer.ReadDouble( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_string_Click( object sender, EventArgs e )
        {
            // 读取字符串
            readResultRender( mcNetServer.ReadString( textBox3.Text, ushort.Parse( textBox5.Text ) ), textBox3.Text, textBox4 );
        }


        #endregion

        #region 单数据写入测试


        private void button24_Click( object sender, EventArgs e )
        {
            // bool写入
            try
            {
                mcNetServer.Write( textBox8.Text, bool.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button22_Click( object sender, EventArgs e )
        {
            // short写入
            try
            {
                mcNetServer.Write( textBox8.Text, short.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button21_Click( object sender, EventArgs e )
        {
            // ushort写入
            try
            {
                mcNetServer.Write(textBox8.Text, ushort.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }


        private void button20_Click( object sender, EventArgs e )
        {
            // int写入
            try
            {
                mcNetServer.Write( textBox8.Text, int.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button19_Click( object sender, EventArgs e )
        {
            // uint写入
            try
            {
                mcNetServer.Write( textBox8.Text , uint.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button18_Click( object sender, EventArgs e )
        {
            // long写入
            try
            {
                mcNetServer.Write( textBox8.Text, long.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button17_Click( object sender, EventArgs e )
        {
            // ulong写入
            try
            {
                mcNetServer.Write(textBox8.Text , ulong.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button16_Click( object sender, EventArgs e )
        {
            // float写入
            try
            {
                mcNetServer.Write( textBox8.Text, float.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button15_Click( object sender, EventArgs e )
        {
            // double写入
            try
            {
                mcNetServer.Write( textBox8.Text, double.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }


        private void button14_Click( object sender, EventArgs e )
        {
            // string写入
            try
            {
                mcNetServer.Write( textBox8.Text, textBox7.Text );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }



        #endregion
        

        private void linkLabel2_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            HslCommunication.BasicFramework.FormSupport form = new HslCommunication.BasicFramework.FormSupport( );
            form.ShowDialog( );
        }

        private void button4_Click( object sender, EventArgs e )
        {
            // 连接异形客户端
            using (FormInputAlien form = new FormInputAlien( ))
            {
                if (form.ShowDialog( ) == DialogResult.OK)
                {
                    OperateResult connect = mcNetServer.ConnectHslAlientClient( form.IpAddress, form.Port, form.DTU );
                    if (connect.IsSuccess)
                    {
                        MessageBox.Show( HslCommunication.StringResources.Language.ConnectedSuccess );
                    }
                    else
                    {
                        MessageBox.Show( HslCommunication.StringResources.Language.ConnectedFailed + connect.Message );
                    }
                }
            }
        }
        

        private void button9_Click( object sender, EventArgs e )
        {
            // 将服务器的数据池存储起来
            if (mcNetServer != null)
            {
                mcNetServer.SaveDataPool( "123.txt" );
                MessageBox.Show( "Save file finish" );
            }
        }

        private void button8_Click( object sender, EventArgs e )
        {
            // 从文件加载服务器的数据池
            if (mcNetServer != null)
            {
                if (System.IO.File.Exists( "123.txt" ))
                {
                    mcNetServer.LoadDataPool( "123.txt" );
                    MessageBox.Show( "Load data finish" );
                }
                else
                {
                    MessageBox.Show( "File is not exist！" );
                }
            }
        }


        private Random random = new Random( );
        private string timerAddress = string.Empty;
        private long timerValue = 0;
        private System.Windows.Forms.Timer timerWrite = null;
        private void button10_Click( object sender, EventArgs e )
        {
            // 定时写
            timerWrite = new System.Windows.Forms.Timer( );
            timerWrite.Interval = 300;
            timerWrite.Tick += TimerWrite_Tick;
            timerWrite.Start( );
            timerAddress = textBox8.Text;
            button10.Enabled = false;
        }

        private void TimerWrite_Tick( object sender, EventArgs e )
        {
            ushort value = (ushort)(Math.Sin( 2 * Math.PI * timerValue / 100 ) * 100 + 100);
            mcNetServer.Write( timerAddress, value );
            timerValue++;
        }

    }
}
