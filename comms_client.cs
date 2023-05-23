using System.Net;
using System.Net.Sockets;
using System.Text;

namespace com_client
{
    public class Communication_client
    {
        Socket client = null;
        int binary = 0;

        public Communication_client()
        {
            Console.WriteLine("Put in IP");
            String input = Console.ReadLine();
            this.Communicate(input).Wait();

        }
        public Communication_client(string adress)
        {
            this.Communicate(adress).Wait();
        }

        private async Task<bool> Communicate(string adress)
        {
            var hostName = Dns.GetHostName();
            //IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(hostName);
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(adress); //maps adress to an ip-adress
            IPAddress ipAddress = ipHostInfo.AddressList[0]; //saves the ip-adress

            IPEndPoint ipEndPoint = new(ipAddress, 8080); //Adds ip-adress to a port
            Console.WriteLine("client created");
            this.client = new(//Creates the client object, using these arguments: (Which constructor is this?)
                    ipEndPoint.AddressFamily, //Specify what kind of adress
                    SocketType.Stream, //Stream means reliable communication between two ports. Uses TCP. Whatever that means
                    ProtocolType.Tcp); //Specifies protocol? 

            await this.client.ConnectAsync(ipEndPoint);
            //If ConnectAsync isn't done the code will return to the calling method
            //Therefore it will not continue to the while loop
            //Until ConnectAsync is done

            //this.Send_message("Hello World!");
            return true;
        }

        public async void Send_message(string _message)
        {

            // Send message.
            var message = _message; //Message
            var messageBytes = Encoding.UTF8.GetBytes(message); //maps message to bytes
            _ = await this.client.SendAsync(messageBytes, SocketFlags.None); //Sends message over and waits for signal that it is done
                                                                             //Console.WriteLine($"Socket client sent message: \"{message}\""); //prints to console
            while (true)//While for communicating with other computer
            {
                // Receive ack. Acknowledgment
                var buffer = new byte[1_024]; //Don't know
                var received = await this.client.ReceiveAsync(buffer, SocketFlags.None);//preps for message?
                var response = Encoding.UTF8.GetString(buffer, 0, received);//maps bytes to something readable
                                                                            // if (response == "<|ACK|>") //If I get answer print answer to console
                                                                            // {
                                                                            //     Console.WriteLine(
                                                                            //         $"Socket client received acknowledgment: \"{response}\"");
                                                                            // }
                                                                            // Sample output:
                                                                            //     Socket client sent message: "Hi friends 👋!<|EOM|>"
                                                                            //     Socket client received acknowledgment: "<|ACK|>"
            }
        }

        public async void Send_message()
        {
            Console.WriteLine("\nInput");
            var _message = Console.ReadLine() + $"{binary}";
            binary++;
            // Send message.
            var message = _message; //Message
            var messageBytes = Encoding.UTF8.GetBytes(message); //maps message to bytes
            _ = await this.client.SendAsync(messageBytes, SocketFlags.None); //Sends message over and waits for signal that it is done
                                                                             //Console.WriteLine($"Socket client sent message: \"{message}\""); //prints to console

            // Receive ack. Acknowledgment
            /*var buffer = new byte[1_024]; //Don't know
            var received = await this.client.ReceiveAsync(buffer, SocketFlags.None);//preps for message?
            var response = Encoding.UTF8.GetString(buffer, 0, received);//maps bytes to something readable
            if (response == "<|ACK|>") //If I get answer print answer to console
            {
                //Console.WriteLine(
                //    $"Socket client received acknowledgment: \"{response}\"");
            }*/
            // Sample output:
            //     Socket client sent message: "Hi friends 👋!<|EOM|>"
            //     Socket client received acknowledgment: "<|ACK|>"
        }
        //Used for receiving messages from communication server
        public async Task<bool> Receiver()
        {
            bool player_print = true;
            var buffer = new byte[1_024]; //Don't know
            while (true)
            {
                //Console.WriteLine("Waiting for message");
                var received = await this.client.ReceiveAsync(buffer, SocketFlags.None);//preps for message?
                var response = Encoding.UTF8.GetString(buffer, 0, received);//maps bytes to something readable
                //Console.WriteLine(response);
                if (response == "input")
                {
                    this.Send_message();
                }
                else if (response[0] == 'c')
                {//Maybe put in other method to make it more structured and clean????
                    string[] str_player = response.Split(" ");

                    if (player_print)
                    {
                        Console.WriteLine("\nPlayer\n");
                        for (int i = 0; i < str_player[1].Length; i += 2)
                        {
                            Console.WriteLine(Code_to_card(str_player[1][i], str_player[1][i + 1]));
                        }
                        Console.WriteLine("\nSum " + str_player[2]);
                        player_print = false;
                    }
                    else
                    {
                        Console.WriteLine("\nAi\n");
                        for (int i = 0; i < str_player[1].Length; i += 2)//Check for arithmetic miss
                        {
                            Console.WriteLine(Code_to_card(str_player[1][i], str_player[1][i + 1]));
                        }
                        Console.WriteLine("\nSum " + str_player[2]);
                        player_print = true;
                    }
                }
                else if (response[0] == 'w')
                {
                    String winner = response.Substring(1);
                    Console.WriteLine("The winner is: " + winner);
                }
            }
            Console.WriteLine("loop done");
            return true;
        }

        private String Code_to_card(char suit, char value)
        {
            String card = "";
            switch (suit)
            {
                case 's':
                    card += "Spades ";
                    break;
                case 'h':
                    card += "Hearts ";
                    break;
                case 'd':
                    card += "Diamonds ";
                    break;
                case 'c':
                    card += "Clubs ";
                    break;
            }
            switch (value)
            {
                case 'a':
                    card += "Ace";
                    break;
                case 'k':
                    card += "King";
                    break;
                case 'q':
                    card += "Queen";
                    break;
                case 'j':
                    card += "Jack";
                    break;
                case 't':
                    card += "10";
                    break;
                default:
                    card += value;
                    break;
            }
            return card;
        }

        public void Shutdown()
        {
            this.client.Shutdown(SocketShutdown.Both);
        }

    }
}
