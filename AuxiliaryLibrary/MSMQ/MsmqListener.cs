using System;
using System.Diagnostics;
using System.Messaging;

namespace AuxiliaryLibrary.MSMQ
{
    /// <summary>
    /// 监听MSMQ
    /// </summary>
    public class MsmqListener
    {
        /// <summary>
        /// 监听状态
        /// </summary>
        private bool _listen;

        /// <summary>
        /// 消息队列
        /// </summary>
        private readonly MessageQueue _queue;

        /// <summary>
        /// 消息接收事件
        /// </summary>
        public event MessageReceivedEventHandler MessageReceived;

        /// <summary>
        /// 消息接收委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void MessageReceivedEventHandler(object sender, MessageEventArgs args);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="queuePath">队列路径</param>
        /// <param name="xmlMessageFormatter">序列化和反序列化对象到或从使用基于 XSD 架构定义的 XML 格式的消息正文</param>
        public MsmqListener(string queuePath, XmlMessageFormatter xmlMessageFormatter)
        {
            _queue = new MessageQueue(queuePath) { Formatter = xmlMessageFormatter };
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        public void Start()
        {
            _listen = true;
            _queue.PeekCompleted += OnPeekCompleted;
            _queue.ReceiveCompleted += OnReceiveCompleted;
            StartListening();
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop()
        {
            _listen = false;
            //从队列中删除的情况下读取一条消息
            _queue.PeekCompleted -= OnPeekCompleted;
            _queue.ReceiveCompleted -= OnReceiveCompleted;
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        private void StartListening()
        {
            if (!_listen)
            {
                return;
            }
            // 异步接收BeginReceive()方法无MessageQueueTransaction重载(微软类库的Bug?)
            // 这里变通一下：先异步BeginPeek()，然后带事务异步接收Receive(MessageQueueTransaction)
            if (_queue.Transactional)
            {
                _queue.BeginPeek();
            }
            else
            {
                _queue.BeginReceive();
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            _queue.EndPeek(e.AsyncResult);
            var trans = new MessageQueueTransaction();
            try
            {
                trans.Begin();
                var msg = _queue.Receive(trans);
                trans.Commit();

                StartListening();

                if (msg != null) FireRecieveEvent(msg.Body);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                trans.Abort();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var msg = _queue.EndReceive(e.AsyncResult);

            StartListening();

            FireRecieveEvent(msg.Body);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        private void FireRecieveEvent(object body)
        {
            MessageReceived?.Invoke(this, new MessageEventArgs(body));
        }
    }


    /// <summary>
    /// 消息参数
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// 消息体
        /// </summary>
        public object MessageBody { get; }

        public MessageEventArgs(object body)
        {
            MessageBody = body;
        }
    }
}