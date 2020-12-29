using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

class infoForm : Form
{

    protected Label receive { get; set; }
    protected Label send { get; set; }
    protected Label interfaceName { get; set; }
    protected NetworkInterface nic { get; set; }

    String title;
    int Width;
    int Height;
    int second;

    long receiveByteIPv4;
    long sendByteIPv4;

    Task updateTask;

    void changeReceiveValue( long value = 0 )
    {
        String suffix = String.Empty;
        String showValue = String.Empty;

        if( value != null ) 
        {
            if( value > 1000 && value < 1000000 )
            {
                
                suffix = "KB";
                showValue = (value / 1000).ToString();
            
            }else if( value > 1000000 ){
                
                suffix = "MB";
                showValue = (value / 1000000).ToString();
            
            }else if( value < 1000 ){

                suffix = "B";
                showValue = value.ToString();

            }
            this.receive.Text = showValue +  suffix + " (IPv4)/s (Download)";
        }
        else throw new Exception();
    }

    void changeSendValue( long value = 0 )
    {
        String suffix = String.Empty;
        String showValue = String.Empty;

        if( value != null){
            if( value > 1000 && value < 1000000 )
            {
                
                suffix = "KB";
                showValue = (value / 1000).ToString();
            
            }else if( value > 1000000 ){
                
                suffix = "MB";
                showValue = (value / 1000000).ToString();
            
            }else if( value < 1000 ){

                suffix = "B";
                showValue = value.ToString();

            }
            this.send.Text = showValue + suffix + " B(IPv4)/s (Upload)";
        } 
        else throw new Exception();
    }

    /// <summary>
    /// 一秒毎に呼ばれるメソッド
    /// call every second
    /// </summary>
    void onUpdate()
    {

        this.receiveByteIPv4 = this.nic.GetIPv4Statistics().BytesReceived;
        this.sendByteIPv4 = this.nic.GetIPv4Statistics().BytesSent;

        this.Text = this.title + " " + second.ToString() + "s";

        changeReceiveValue( (long)receiveByteIPv4 );
        changeSendValue( (long)sendByteIPv4 );

    }

    /// <summary>
    /// フォーム終了時の処理
    /// on exit processing
    /// </summary>
    /// <param name="s">object</param>
    /// <param name="e">eventargs</param>
    void onClose(object s, System.EventArgs e)
    {
        Environment.Exit(0);
    }

    #region constructor

    public infoForm( NetworkInterface nic )
    {

        this.title = "インターフェースの状況";
        this.Width = 400;
        this.Height = 400;
        this.nic = nic;

        this.receive = new Label() {
            Text = "B/(Download)" ,
            Location  = new Point( 10 , 10 ) ,            
            TabIndex = 2
        };

        this.send = new Label() {
            Text = "B/(Upload" ,
            Location = new Point( 10 , 40 ) ,
            TabIndex = 3
        };

        this.interfaceName = new Label() {
            Text =  nic.Description.ToString() ,
            Location = new Point( 110 ,10 ) ,
            AutoSize = true ,
            TabIndex = 4
        };

        base.Text = (String) this.title;
        base.Size = new Size( this.Width , this.Height );
        base.MaximizeBox  = false;
        base.MinimizeBox = true;
        base.MaximumSize = new System.Drawing.Size( this.Width , this.Height );
        base.MinimumSize = new System.Drawing.Size( this.Width , this.Height );
        base.Opacity = 0.80;
        base.Controls.Add(receive);
        base.Controls.Add(send);
        base.Controls.Add(interfaceName);
        base.Closed += new EventHandler(onClose);

        this.updateTask = Task.Run( async () => {
            while( true )
            {
                this.onUpdate();
                await Task.Delay(1000);
                this.second += 1;
            }
        });

    }

    public infoForm( NetworkInterface nic , String title , int width , int height )
    {

        this.title = title;
        this.Width = width;
        this.Height = height;
        this.nic = nic;

        this.receive = new Label() {
            Text = "B/(Download)" ,
            Location  = new Point( 10 , 10 ) ,            
            TabIndex = 2
        };

        this.send = new Label() {
            Text = "B/(Upload" ,
            Location = new Point( 10 , 40 ) ,
            TabIndex = 3
        };

        base.Text = (String) this.title;
        base.Size = new Size( this.Width , this.Height );
        base.MaximizeBox  = false;
        base.MinimizeBox = true;
        base.MaximumSize = new System.Drawing.Size( this.Width , this.Height );
        base.MinimumSize = new System.Drawing.Size( this.Width , this.Height );
        base.Opacity = 0.80;
        base.Controls.Add(receive);
        base.Controls.Add(send);
        base.Closed += new EventHandler(onClose);

        this.updateTask = Task.Run( async () => {
            while( true )
            {
                this.onUpdate();
                await Task.Delay(1000);
                this.second += 1;
            }
        });

    }

    #endregion

}

sealed class sysinfo {

    private static readonly NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
    private static Label receive;
    private static Label send;
    private static int counter = 0;
    
    #region staticmethods

    /// <summary>
    /// フォームの更新
    /// uodate form
    /// </summary>
    /// <returns></returns>    
    static void updateForm()
    {
        receive.Text = "helloWorld";
    }


    /// <summary>
    /// Windowsフォームの生成
    /// create windows form
    /// </summary>
    ///<param name="ishide"> true : hide form / false : show form </param> 
    static void createForm( bool ishide , NetworkInterface nic )
    {
        infoForm formObject = new infoForm(nic: nic);
        if(!ishide) Application.Run(formObject);
    }

    /// <summary>
    /// 受信データ数の取得
    /// get recieved packets
    /// </summary>
    /// <returns>受信データ数</returns>
    static long getReceivedBytes( NetworkInterface nic ){
        IPv4InterfaceStatistics ipv4 = nic.GetIPv4Statistics();
        return ipv4.BytesReceived;
    }

    /// <summary>
    ///  送信データ数の取得
    /// get sended packets
    /// </summary>
    /// <returns>送信データ数</returns>
    static long getSendBytes( NetworkInterface nic ){
        IPv4InterfaceStatistics ipv4 = nic.GetIPv4Statistics();
        return ipv4.BytesSent;
    }

    /// <summary>
    /// IPv4アドレスの取得
    /// </summary>
    /// <param name="nic">ネットワークインターフェース</param>
    /// <returns>IPv4アドレス : String</returns>
    static IPAddress[] getIPv4Address( NetworkInterface nic ){
        String dnsHostName = Dns.GetHostName();
        IPAddress[] adr = Dns.GetHostAddresses(dnsHostName);
        return adr;
    }

    /// <summary>
    ///  ネットワークインターフェースの一覧を表示する
    /// show network interfaces on console
    /// </summary>
    static void showNetworkInterfaces()
    {

        sbyte count = 0;

        Console.WriteLine("\nネットワークインターフェースの一覧\n");

        foreach ( NetworkInterface inf in nis )
        {
            String NicName = (String)inf.Description;
            Console.WriteLine( (++count).ToString() + "\t:\t" + NicName);
        }

    }

    /// <summary>
    /// 指定したネットワークインターフェースの詳細を表示する
    /// Show specified network interface description
    /// </summary>
    /// <param name="inc">ネットワークインターフェース</param>
    static void showInformation( NetworkInterface nic )
    {
        
        int count = 0;
        const int sec = 1000;
        
        Console.WriteLine("\non IPv4\n");
        createForm( false , nic );
        
        while(true){
            IPv4InterfaceStatistics ipv4 = nic.GetIPv4Statistics();
            
            Console.Write("         Received : " + ipv4.BytesReceived + "B/s Sended : " + ipv4.BytesSent + "B/s \r");
            Console.Write("     " + (++count) + "s \r");
            
            Thread.Sleep(sec);
        }

    }

    /// <summary>
    /// EXITが入力された場合終了
    /// Exit if the string "exit" is entered
    /// </summary>
    ///     
    /// <param name="arg">入力対象文字列 : String</param>
    static void isExit( String arg )
    {
        if( arg == "exit" ) Environment.Exit(0);
    }

    #endregion
    
    private static void Main(String[] args)
    {
        /**
        *
        *   ネットワークインターフェース(NIC)の一覧をコンソール上に出力する
        *   Output the network interfaces on console
        *
        */
         showNetworkInterfaces();

        while(true) {
            Console.WriteLine("\nインターフェースを選択して下さい (1~" + (nis.Length).ToString() + ")\n");
            Console.Write(">");
            String get = Console.ReadLine();
            isExit(get);

            try {

                int get_int = Convert.ToInt32(get);
                
                if( get_int > 0 && get_int <= nis.Length )
                {

                    showInformation(nis[ get_int - 1 ]);

                }else{
                    Console.WriteLine("正しい文字列を入力してください");    
                }

            } catch( Exception error ){
                Console.WriteLine("正しい文字列を入力してください");
            }

        }
        
    }

}