using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace AuxiliaryLibrary.Socket
{
    /// <summary>
    /// Socket服务端
    /// </summary>
    public class SocketServer
    {
        private System.Net.Sockets.Socket _socket;

        private readonly string _ip;

        private readonly int _port;

        private bool _isListen = true;

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">监听的IP地址</param>
        /// <param name="port">监听的端口</param>
        public SocketServer(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        /// <summary>
        /// 构造函数,监听IP地址默认为本机0.0.0.0
        /// </summary>
        /// <param name="port">监听的端口</param>
        public SocketServer(int port)
        {
            _ip = "0.0.0.0";
            _port = port;
        }

        #endregion

        #region 内部成员

        
        private void StartListen()
        {
            try
            {
                _socket.BeginAccept(asyncResult =>
                {
                    try
                    {
                        System.Net.Sockets.Socket newSocket = _socket.EndAccept(asyncResult);

                        //马上进行下一轮监听,增加吞吐量
                        if (_isListen)
                            StartListen();

                        SocketConnection newClient = new SocketConnection(newSocket, this)
                        {
                            HandleRecMsg = HandleRecMsg,
                            HandleClientClose = HandleClientClose,
                            HandleSendMsg = HandleSendMsg,
                            HandleException = HandleException
                        };

                        newClient.StartRecMsg();
                        ClientList.AddLast(newClient);

                        HandleNewClientConnected?.Invoke(this, newClient);
                    }
                    catch (Exception ex)
                    {
                        HandleException?.Invoke(ex);
                    }
                }, null);
            }
            catch (Exception ex)
            {
                HandleException?.Invoke(ex);
            }
        }

        #endregion

        #region 外部接口

        /// <summary>
        /// 开始服务，监听客户端
        /// </summary>
        public void StartServer()
        {
            try
            {
                //实例化套接字（ip4寻址协议，流式传输，TCP协议）
                _socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //创建ip对象
                IPAddress address = IPAddress.Parse(_ip);
                //创建网络节点对象包含ip和port
                IPEndPoint endpoint = new IPEndPoint(address, _port);
                //将 监听套接字绑定到 对应的IP和端口
                _socket.Bind(endpoint);
                //设置监听队列长度为Int32最大值(同时能够处理连接请求数量)
                _socket.Listen(int.MaxValue);
                //开始监听客户端
                StartListen();
                HandleServerStarted?.Invoke(this);
            }
            catch (Exception ex)
            {
                HandleException?.Invoke(ex);
            }
        }

        /// <summary>
        /// 所有连接的客户端列表
        /// </summary>
        public LinkedList<SocketConnection> ClientList { get; set; } = new LinkedList<SocketConnection>();

        /// <summary>
        /// 关闭指定客户端连接
        /// </summary>
        /// <param name="theClient">指定的客户端连接</param>
        public void CloseClient(SocketConnection theClient)
        {
            theClient.Close();
        }

        #endregion

        #region 公共事件

        /// <summary>
        /// 异常处理程序
        /// </summary>
        public Action<Exception> HandleException { get; set; }

        #endregion

        #region 服务端事件

        /// <summary>
        /// 服务启动后执行
        /// </summary>
        public Action<SocketServer> HandleServerStarted { get; set; }

        /// <summary>
        /// 当新客户端连接后执行
        /// </summary>
        public Action<SocketServer, SocketConnection> HandleNewClientConnected { get; set; }

        /// <summary>
        /// 服务端关闭客户端后执行
        /// </summary>
        public Action<SocketServer, SocketConnection> HandleCloseClient { get; set; }

        #endregion

        #region 客户端连接事件

        /// <summary>
        /// 客户端连接接受新的消息后调用
        /// </summary>
        public Action<byte[], SocketConnection, SocketServer> HandleRecMsg { get; set; }

        /// <summary>
        /// 客户端连接发送消息后回调
        /// </summary>
        public Action<byte[], SocketConnection, SocketServer> HandleSendMsg { get; set; }

        /// <summary>
        /// 客户端连接关闭后回调
        /// </summary>
        public Action<SocketConnection, SocketServer> HandleClientClose { get; set; }

        #endregion
    }
}