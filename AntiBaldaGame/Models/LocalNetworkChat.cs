using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Avalonia.Threading;

namespace AntiBaldaGame.Models;

public class LocalNetworkChat(int recieverPort)
{
    int RecieverPort => recieverPort;

    public void StartServer()
    {
        //Console.OutputEncoding = Encoding.UTF8;
        //new Thread(
        //Dispatcher.UIThread.Invoke(
        new Thread(() =>
        {
            // Устанавливаем для сокета локальную конечную точку
            var ipAddr = IPAddress.Parse(GetLocalIPv4Addresses()[0]);

            IPEndPoint ipEndPoint = new(ipAddr, RecieverPort);

            // Создаем сокет Tcp/Ip
            Socket sListener = new(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                if (!sListener.IsBound)
                    sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                // Начинаем слушать соединения
                while (true)
                {
                    Console.WriteLine("Awaiting connection to {0}", ipEndPoint);

                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();

                    if (!Settings.Instance.GameRunning)
                        Dispatcher.UIThread.Post(() =>
                        {
                            MultiplayerHandler.Instance.Connected = true;
                            //Console.WriteLine(MultiplayerHandler.Instance.Connected);
                        }, DispatcherPriority.Normal);

                    string? data = null;

                    // Мы дождались клиента, пытающегося с нами соединиться

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    // Показываем данные на консоли
                    //Console.Write("Полученный текст: " + data + "\n\n");
                    //Console.WriteLine(data);
                    if (!Settings.Instance.GameRunning)
                    {
                        if (data.StartsWith("start"))
                        {
                            MultiplayerHandler.Instance.IsFirstPlayer = false;
                            var spl = data.Split();
                            Settings.Instance.GridSize = int.Parse(spl[1]);
                            Settings.Instance.StartWord = spl[2];
                            Settings.Instance.OtherPlayerName = spl[3];
                            SendAnswerStartCom();
                            OnGameStart.Invoke();
                        }
                        else
                        {
                            Dispatcher.UIThread.Post(
                                () => MultiplayerHandler.Instance.IncomeText = data,
                                DispatcherPriority.Normal
                            );
                        }
                    }
                    else
                    {
                        if (data.StartsWith("exit"))
                        {
                            OnGameExit.Invoke();
                        }
                        else if (data.StartsWith("startAns"))
                        {
                            Settings.Instance.OtherPlayerName = data.Split()[1];
                            OnGameStart.Invoke();
                        }
                        else if (data.StartsWith("skip"))
                        {
                            OnSkipRequested.Invoke();
                        }
                        else if (data.StartsWith("next"))
                        {
                            var spt = data.Split();
                            OnNextRoundRequested.Invoke(int.Parse(spt[1]), spt[2][0], int.Parse(spt[3]), spt[4]);
                        }
                    }

                    // Отправляем ответ клиенту\
                    string reply = "Спасибо за запрос в " + data.Length.ToString()
                            + " символов";
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                    if (data.IndexOf("<TheEnd>") > -1)
                    {
                        Console.WriteLine("Сервер завершил соединение с клиентом.");
                        break;
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                //Console.ReadLine();
            }
        }).Start();
    }

    public void SendMessageFromSocket(string address, int port)
    {
        new Thread(() =>
        {
        start:

            // Буфер для входящих данных
            byte[] bytes = new byte[1024];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            IPAddress ipAddr = IPAddress.Parse(address);
            IPEndPoint ipEndPoint = new(ipAddr, port);

            Socket sender = new(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            while (true)
            {
                try
                {
                    sender.Connect(ipEndPoint);
                    break;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
            string message = "";
            var mp = MultiplayerHandler.Instance;

            if (!Settings.Instance.GameRunning)
            {
                var prevText = mp.SendingText;
                while (!Settings.Instance.GameRunning &&
                    (mp.SendingText == null || mp.SendingText == " " || mp.SendingText == prevText))
                {
                    Thread.Sleep(100);
                }
                message = mp.SendingText?
                    .Replace("start", "x").Replace("exit", "")
                    ?? " ";
            }
            else
            {
                while (command == null)
                {
                    Thread.Sleep(100);
                }
                message = command;
                command = null;
            }

            //Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint!.ToString());
            byte[] msg = Encoding.UTF8.GetBytes(message);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            //Console.WriteLine("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));

            // Используем рекурсию для неоднократного вызова SendMessageFromSocket()
            if (message.IndexOf("exit") == -1)
                goto start;//SendMessageFromSocket(address, port); //goto start;

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }).Start();
    }

    private string? command = null;
    public void SendStartGameCom()
    {
        command = $"start {Settings.Instance.GridSize} {Settings.Instance.StartWord} {Settings.Instance.Name}";
    }

    public void SendExitGameCom()
    {
        command = "exit";
    }

    public void SendSkipCom()
    {
        command = "skip";
    }

    public void SendNextMoveCom(int cell, char letter, int scoreAdded, string word)
    {
        command = $"next {cell} {letter} {scoreAdded} {word}";
    }

    private void SendAnswerStartCom()
    {
        command = $"startAns {Settings.Instance.Name}";
    }

    public event Action OnGameStart = () => { };
    public event Action OnGameExit = () => { };
    public event Action OnSkipRequested = () => { };
    public event Action<int,char,int,string> OnNextRoundRequested = (_,_,_,_) => { };

    public static System.Collections.Generic.List<string> GetLocalIPv4Addresses()
    {
        var list = new System.Collections.Generic.List<string>();

        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            // Рассматриваем только поднятые интерфейсы (UP) и не loopback
            if (ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                IPInterfaceProperties ipProps = ni.GetIPProperties();
                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IPAddress ip = addr.Address;

                        // Исключаем loopback (127.0.0.1)
                        if (!IsPrivateIP(ip))
                            continue;

                        list.Add(ip.ToString());
                    }
                }
            }
        }

        return list;
    }

    /// <summary>
    /// Проверяет, является ли IP-адрес приватным (из диапазона локальной сети)
    /// </summary>
    static bool IsPrivateIP(IPAddress address)
    {
        if (address == null) return false;

        byte[] bytes = address.GetAddressBytes();

        // Проверяем по частным диапазонам:
        // 10.x.x.x
        if (bytes[0] == 10)
            return true;

        // 172.16.x.x - 172.31.x.x
        if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
            return true;

        // 192.168.x.x
        if (bytes[0] == 192 && bytes[1] == 168)
            return true;

        // 169.254.x.x (APIPA — link-local, тоже локальный, но не настроен DHCP)
        if (bytes[0] == 169 && bytes[1] == 254)
            return true;

        return false;
    }
}
