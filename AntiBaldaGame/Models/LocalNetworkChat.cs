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
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);

                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    string? data = null;

                    Settings.Instance.Connected = true;
                    // Мы дождались клиента, пытающегося с нами соединиться

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    // Показываем данные на консоли
                    Console.Write("Полученный текст: " + data + "\n\n");

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
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

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
                }
            }


            //Console.Write("Введите сообщение: ");
            string message = Console.ReadLine()!;

            Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint!.ToString());
            byte[] msg = Encoding.UTF8.GetBytes(message);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            //Console.WriteLine("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));

            // Используем рекурсию для неоднократного вызова SendMessageFromSocket()
            if (message.IndexOf("<TheEnd>") == -1)
                SendMessageFromSocket(address, port); //goto start;

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }).Start();
    }


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
                    if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
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
