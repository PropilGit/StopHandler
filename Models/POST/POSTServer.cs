using System;
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

        public delegate void UpdateLog(string msg, bool isError = false);
        public event UpdateLog onLogUpdate;
        void AddLog(string msg, bool isError = false)
        {
            if (onLogUpdate != null)
            {
                onLogUpdate(msg, isError);
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
            string post = RecieveDataToString_SB(stream);

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
            int buffer = 256;
            int maxCounter = 40;

            int counter = 0;
            byte[] data = new byte[buffer * maxCounter];
            
            do
            {
                int bytes = stream.Read(data, counter * buffer, buffer);
                counter++;
            }
            while (stream.DataAvailable && counter < maxCounter);

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(Encoding.UTF8.GetString(data, 0, buffer * counter));
            return strBuilder.ToString();
        }
        #endregion

        #region SendReqest

        public async void SendPOSTAsync(string url, string message)
        {
            await Task.Run(() => SendPOST(url, message));
        }

        void SendPOST(string url, string message)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";         

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(message);
            request.ContentLength = byteArray.Length;

            using (System.IO.Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
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
        
string RecieveDataToString(NetworkStream stream)
        {
            var bytes = GetBytes(stream, 1024);
            byte[] result = new byte[byt];
            foreach (var bytes in )
            {
                
            }
            return result;
        }

        IEnumerable<byte[]> GetBytes(NetworkStream stream, int bufferValue)
        {
            int counter = 1000;
            byte[] buffer = new byte[bufferValue];
            while (stream.DataAvailable && counter > 0)
            {
                counter--;
                int bytes = stream.Read(buffer, 0, buffer.Length);
                yield return buffer;
            }
        }
        
*/