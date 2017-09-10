using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;

namespace AuxiliaryLibrary.MSMQ
{
    /// <summary>
    /// MSMQ辅助类
    /// </summary>
    public class MsmqHelper
    {
        /// <summary>
        /// 创建MSMQ队列
        /// </summary>
        /// <param name="queuePath">队列路径</param>
        /// <param name="transactional">是否事务队列</param>
        public static bool Createqueue(string queuePath, bool transactional = false)
        {
            if (string.IsNullOrEmpty(queuePath))
            {
                throw new ArgumentNullException(nameof(queuePath));
            }
            try
            {
                //判断队列是否存在
                if (!MessageQueue.Exists(queuePath))
                {
                    MessageQueue messageQueue = MessageQueue.Create(queuePath, transactional);
                    //对队列赋予权限
                    messageQueue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                }
            }
            catch (MessageQueueException e)
            {
                throw e;
            }
            return true;
        }

        /// <summary>
        /// 删除队列
        /// </summary>
        /// <param name="queuePath">队列名称：格式.\private$\myQueue</param>
        public static bool Deletequeue(string queuePath)
        {
            if (string.IsNullOrEmpty(queuePath))
            {
                throw new ArgumentNullException(nameof(queuePath));
            }
            try
            {
                //判断队列是否存在
                if (MessageQueue.Exists(queuePath))
                {
                    MessageQueue.Delete(queuePath);
                }

            }
            catch (MessageQueueException e)
            {
                throw e;
            }
            return true;
        }

        /// <summary>
        /// 批量发送消息
        /// </summary>
        /// <typeparam name="T">消息实体</typeparam>
        /// <param name="target">消息集合</param>
        /// <param name="queuePath">队列路径</param>
        /// <returns></returns>
        public static bool BatchSendMessage<T>(List<T> target, string queuePath)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (string.IsNullOrEmpty(queuePath))
            {
                throw new ArgumentNullException(nameof(queuePath));
            }
            MessageQueueTransaction tran = new MessageQueueTransaction();
            MessageQueue myQueue = new MessageQueue();
            try
            {
                //判断队列是否存在
                if (!MessageQueue.Exists(queuePath))
                {
                    return false;
                }
                //连接到本地的队列
                using (myQueue = new MessageQueue(queuePath, QueueAccessMode.SendAndReceive))
                {
                    var myMessage = new Message
                    {
                        //消息内容
                        Body = target,
                        //消息序列化的格式为XML（XML，Binary，ActiveX）
                        Formatter = new XmlMessageFormatter(new[] { typeof(T) }),
                        // 获取或设置接收消息队列生成的确认消息的队列
                        AdministrationQueue = myQueue,
                        //获取或设置确认消息返回给发送应用程序的类型
                        AcknowledgeType = AcknowledgeTypes.PositiveReceive | AcknowledgeTypes.PositiveArrival
                    };
                    tran.Begin();

                    foreach (var mess in target)
                    {
                        bool state = SendMessage(mess, myQueue, myMessage, tran);
                        if (state == false)
                        {
                            return false;
                        }
                    }
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                tran.Abort();
                throw ex;
            }
            finally
            {
                myQueue.Close();
            }
            return true;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T">用户数据类型</typeparam>
        /// <param name="target">用户数据(数据实体类型)</param>
        /// <param name="myMessage"></param>
        /// <param name="tran">消息队列内部事务</param>
        /// <param name="myQueue"></param>
        /// <returns></returns>
        public static bool SendMessage<T>(T target, MessageQueue myQueue, Message myMessage, MessageQueueTransaction tran = null)
        {
            try
            {
                if (tran == null)
                {
                    myQueue.Send(myMessage);
                }
                else
                {
                    myQueue.Send(myMessage, tran);
                }
                return true;
            }
            catch (ArgumentException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <typeparam name="T">用户的数据类型</typeparam>
        /// <param name="queuePath">消息路径</param>
        /// <param name="tran"></param>
        /// <returns>用户填充在消息当中的数据</returns>
        public static T ReceiveMessage<T>(string queuePath, MessageQueueTransaction tran)
        {
            if (string.IsNullOrEmpty(queuePath))
            {
                throw new ArgumentNullException(nameof(queuePath));
            }
            try
            {
                //连接到本地队列
                MessageQueue myQueue = new MessageQueue(queuePath)
                {
                    Formatter = new XmlMessageFormatter(new[] { typeof(T) })
                };
                Message myMessage;
                //从队列中接收消息
                if (tran == null)
                {
                    myMessage = myQueue.Receive(MessageQueueTransactionType.Automatic);
                }
                else
                {
                    myMessage = myQueue.Receive(tran);
                }
                //获取消息的内容
                if (myMessage != null)
                {
                    return (T)myMessage.Body;
                }
            }
            catch (MessageQueueException e)
            {
                throw e;
            }
            catch (InvalidCastException e)
            {
                throw e;
            }
            return default(T);
        }

        /// <summary>
        /// 获取所有消息
        /// </summary>
        /// <param name="queuePath">队列路径</param>
        /// <returns></returns>
        public static List<string> GetAllMessage(string queuePath)
        {
            MessageQueue myQueue = null;
            try
            {
                myQueue = new MessageQueue(queuePath);
                Message[] message = myQueue.GetAllMessages();
                XmlMessageFormatter formatter = new XmlMessageFormatter(new[] { typeof(string) });
                List<string> msg = new List<string>(message.Length);
                foreach (Message mess in message)
                {
                    mess.Formatter = formatter;
                    msg.Add(mess.Body.ToString());
                }
                return msg;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                if (myQueue != null)
                {
                    myQueue.Dispose();
                }
            }
        }

        /// <summary>
        /// 通过枚举器获取所有消息。
        /// </summary>
        /// <param name="queuePath">队列路径</param>
        /// <returns></returns>
        public static List<string> GetAllMessageByEnumerator(string queuePath)
        {
            List<string> msgs;
            MessageQueue myQueue = null;
            try
            {
                myQueue = new MessageQueue(queuePath);
                XmlMessageFormatter formatter = new XmlMessageFormatter(new[] { typeof(string) });
#pragma warning disable 618
                MessageEnumerator enumerator = myQueue.GetMessageEnumerator();
#pragma warning restore 618
                msgs = new List<string>();
                while (enumerator.MoveNext())
                {
                    Message content = enumerator.Current;
                    if (content != null)
                    {
                        content.Formatter = formatter;
                        msgs.Add(content.Body.ToString());
                    }
                    enumerator.RemoveCurrent();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                if (myQueue != null)
                {
                    myQueue.Dispose();
                }
            }
            return msgs;
        }

        /// <summary>
        /// 采用Peek方法接收消息
        /// </summary>
        /// <typeparam name="T">用户数据类型</typeparam>
        /// <param name="queuePath">队列路径</param>
        /// <returns>用户数据</returns>
        public static T ReceiveMessageByPeek<T>(string queuePath)
        {
            if (string.IsNullOrEmpty(queuePath))
            {
                throw new ArgumentNullException(nameof(queuePath));
            }
            try
            {
                //连接到本地队列
                MessageQueue myQueue = new MessageQueue(queuePath)
                {
                    Formatter = new XmlMessageFormatter(new[] { typeof(T) })
                };
                //从队列中接收消息
                Message myMessage = myQueue.Peek();
                //获取消息的内容
                if (myMessage != null) return (T)myMessage.Body;
            }
            catch (MessageQueueException e)
            {
                throw e;
            }
            catch (InvalidCastException e)
            {
                throw e;
            }
            return default(T);
        }

        /// <summary>
        /// 获取队列中的所有消息
        /// </summary>
        /// <typeparam name="T">用户数据类型</typeparam>
        /// <param name="queuePath">队列路径</param>
        /// <returns>用户数据集合</returns>
        public static List<T> GetAllMessage<T>(string queuePath)
        {
            if (string.IsNullOrEmpty(queuePath))
            {
                throw new ArgumentNullException(nameof(queuePath));
            }
            try
            {
                MessageQueue myQueue = new MessageQueue(queuePath)
                {
                    Formatter = new XmlMessageFormatter(new[] { typeof(T) })
                };
                Message[] msgArr = myQueue.GetAllMessages();
                List<T> list = new List<T>();
                msgArr.ToList().ForEach((o) =>
                {
                    list.Add((T)o.Body);
                });
                return list;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获取指定队列的消息
        /// </summary>
        /// <param name="path">要查找的队列的位置（参数格式：.\Private$\LeeMSMQ）</param>
        /// <returns></returns>
        public Dictionary<string, object> GetSpecifyQueues(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            Dictionary<string, object> queuesDictionary = new Dictionary<string, object>();
            //判断私有消息是否存在
            if (MessageQueue.Exists(path))
            {
                using (MessageQueue mq = new MessageQueue(path))
                {
                    //设置消息队列格式化器
                    mq.Formatter = new XmlMessageFormatter(new[] { "System.String" });
                    //接收消息
                    Message msg = mq.Receive();
                    if (msg != null)
                    {
                        queuesDictionary.Add(msg.Label, msg.Body);
                    }
                    MessageQueue.Delete(path);
                }
            }
            return queuesDictionary;
        }

        /// <summary>
        /// 获取所有私有队列
        /// </summary>
        /// <param name="machineName">专用队列计算机</param>
        /// <returns></returns>
        public Dictionary<string, object> GetAllPrivateQueues(string machineName)
        {
            if (string.IsNullOrEmpty(machineName))
            {
                throw new ArgumentNullException(nameof(machineName));
            }
            Dictionary<string, object> queuesDictionary = new Dictionary<string, object>();
            //检索指定的计算机上的所有专用队列
            foreach (MessageQueue mq in MessageQueue.GetPrivateQueuesByMachine(machineName))
            {
                //返回队列中的所有消息
                mq.Formatter = new XmlMessageFormatter(new[] { "System.String" });
                Message[] msg = mq.GetAllMessages();
                foreach (Message m in msg)
                {
                    queuesDictionary.Add(m.Label, m.Body);
                }
            }
            return queuesDictionary;
        }

        /// <summary>
        /// 清空指定队列的消息
        /// </summary>
        /// <param name="queuePath">队列名称</param>
        public static bool ClearMessage(string queuePath)
        {
            if (string.IsNullOrEmpty(queuePath))
            {
                throw new ArgumentNullException(nameof(queuePath));
            }
            bool state = false;
            try
            {
                if (!MessageQueue.Exists(queuePath))
                {
                    MessageQueue myQueue = new MessageQueue(queuePath);
                    myQueue.Purge();
                    state = true;
                }
            }
            catch (MessageQueueException e)
            {
                throw e;

            }
            return state;
        }
    }
}