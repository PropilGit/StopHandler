using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StopHandler.Models.POST
{
    class POSTServer
    {
        #region Singleton

        private static POSTServer instance;
        public static POSTServer GetInstance(int port = 48654)
        {
            if (instance == null) instance = new POSTServer(port);
            return instance;
        }

        #endregion

        public POSTServer(int port)
        {
            if (port < 48654 || port > 48999)
            {
                this.port = 48654;
                AddLog("ОШИБКА: Не удалось запустить сервер на порту " + port + "! Использован порт по умолчанию (48654).");
            }
            else this.port = port;

            listening = new Thread(new ThreadStart(Listen));
            listener = new TcpListener(IPAddress.Any, port);
        }
        public void Start()
        {
            isListening = true;
            listening.Start();
        }
        public void Stop()
        {
            isListening = false;
            listener.Stop();
        }

        #region Event UpdateLog

        public delegate void UpdateLog(string line);
        public event UpdateLog onLogUpdate;
        void AddLog(string msg)
        {
            if (onLogUpdate != null)
            {
                onLogUpdate(msg);
            }
        }

        #endregion

        #region Event POSTRequest

        public delegate void RequestPOST(IPOSTCommand command);
        public event RequestPOST onPOSTRequest;
        void POSTRequest(IPOSTCommand cmd)
        {
            if (onPOSTRequest != null)
            {
                onPOSTRequest(cmd);
            }
        }

        #endregion

        #region Listening

        TcpListener listener;
        Thread listening;
        private bool isListening = true;
        private int port = 48654;

        void Listen()
        {
            try
            {
                AddLog("Запуск сервера обработки POST-запросов...");
                Thread.Sleep(3000);
                listener.Start();
                AddLog("Сервер обработки POST-запросов запущен. Ожидание запросов...");

                while (isListening)
                {
                    HandleRequestAsync(listener.AcceptTcpClient());
                }
            }
            catch (Exception ex)
            {
                AddLog("ОШИБКА: Непредвиденная ошибка в POSTServer.cs/Listen(): " + ex.Message);
            }
        }

        #endregion

        #region HandleRequest

        bool debug = false;

        public async void HandleRequestAsync(TcpClient client)
        {
            await Task.Run(() => HandleRequest(client));
        }
        void HandleRequest(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            string post = RecieveDataToString_SR(stream);

            //debug
            if(debug) AddLog(post);
            try
            {
                if (post.Substring(0, 4) == "POST")
                {
                    IPOSTCommand cmd = POSTCommand.Parse(post);
                    if (cmd != null)
                    {
                        POSTRequest(cmd);
                    }
                    else
                    {
                        AddLog("Полученный POST-запрос не соответствует критериям");
                        POSTRequest(new ErrorCommand(post));
                    }
                }  
            }
            catch (Exception ex)
            {
                AddLog("ОШИБКА: Ошибка при чтении POST-запроса: " + ex.Message);
                POSTRequest(new ErrorCommand(post));
            }
            finally
            {
                Thread.Sleep(500);
                SMTP_OK(stream);
                stream.Close();
                client.Close();
            }
        }
        string RecieveDataToString_SB(NetworkStream stream)
        {
            byte[] data = new byte[256];
            StringBuilder strBuilder = new StringBuilder();
            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                strBuilder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            return strBuilder.ToString();
        }
        string RecieveDataToString_SR(NetworkStream stream)
        {
            string result = "";
            StreamReader reader = new StreamReader(stream);
            do
            {
                string message = reader.ReadLine();
                if (!string.IsNullOrEmpty(message)) result += message;
            }
            while (!reader.EndOfStream);

            return result;
        }
        #endregion

        public void SMTP_OK(NetworkStream stream)
        {
            Byte[] data = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK");
            stream.Write(data, 0, data.Length);
        }
    }
}


/*  
        public void SMTP_AlertReqest(int taskNum, int time)
        {
            WebRequest request = WebRequest.Create("https://bankrotforum.planfix.ru/webhook/json/timerAlert");
            request.Method = "POST";
            string sName = "{'taskNum':'" + taskNum + "','time':'" + time + "'}";

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(sName);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            using (System.IO.Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
        }
*/