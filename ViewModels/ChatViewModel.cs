using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chat.ViewModels
{
    class ChatViewModel : ViewModelBase
    {
        private string _ip;

        public string Ip
        {
            get { return _ip; }
            set
            {
                _ip = value;
                OnPropertyChanged(nameof(Ip));
            }
        }

        private string _port;

        public string Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        private string _nick;

        public string Nick
        {
            get { return _nick; }
            set
            {
                _nick = value;
                OnPropertyChanged(nameof(Nick));
            }
        }

        private string _chat;

        public string Chat
        {
            get { return _chat; }
            set
            {
                _chat = value;
                OnPropertyChanged(nameof(Chat));
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        TcpClient client;
        StreamReader streamReader;
        StreamWriter streamWriter;

        public event PropertyChangedEventHandler PropertyChanged;

        public ChatViewModel()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (client?.Connected == true)
                        {
                            var line = streamReader.ReadLine();

                            if (line != null)
                            {
                                Chat += line + "\n";
                            }
                            else
                            {
                                client.Close();
                                Chat += "Connected Error" + "\n";

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            });
        }

        public AsyncCommand Connect
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            client = new TcpClient();
                            client.Connect(Ip, Convert.ToInt32(Port));
                            streamReader = new StreamReader(client.GetStream());
                            streamWriter = new StreamWriter(client.GetStream());
                            streamWriter.AutoFlush = true;

                            streamWriter.WriteLine($"Login: {Nick}");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    });
                }, () => client == null || client?.Connected == false);
            }
        }

        public AsyncCommand Send
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            streamWriter.WriteLine($"{Nick}: {Message}");
                            Message = "";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                }, () => client?.Connected == true, !string.IsNullOrWhiteSpace(Message));
            }
        }

        [Obsolete]
        private void SetDefaultSettings()
        {
            String host = System.Net.Dns.GetHostName();
            Ip = System.Net.Dns.GetHostByName(host).AddressList[0].ToString();
            Port = "5050";
            Nick = "Artem";
        }

        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
