using System.Net;
using System.Net.Sockets;
using System.Text;

namespace com_client
{
    public class Communication_client
    {
        Socket client = null;
        int binary = 0;
        /// <summary>
        /// Overloadad konstruktör
        /// 
        /// Tar input från användaren
        /// Inputen ska vara IP-addressen hen vill koppla upp sig till
        /// Tillsist kallar konstruktören på Communitcate(Sträng) metoden
        /// som startar clienten
        /// </summary>
        public Communication_client()
        {
            Console.WriteLine("Put in IP");
            String input = Console.ReadLine();
            this.Communicate(input).Wait();

        }
        /// <summary>
        /// Samma som ovan fast den tar ett argument istället
        /// Denna konstruktör används inte för tillfället
        /// </summary>
        /// <param name="adress">
        /// Sträng, Ska helst vara IP-addressen
        /// händer inte så mycket annars
        /// </param>
        public Communication_client(string adress)
        {
            this.Communicate(adress).Wait();
        }
        /// <summary>
        /// Kopplar upp sig mot servern med hjälp av IP-addressen
        /// 
        /// Skapar en IPendpoint som används i socket
        /// för att koppla upp sig till servern
        /// </summary>
        /// <param name="adress">
        /// Sträng, Serverns Ipadress
        /// </param>
        /// <returns>
        /// Task<bool>, Returnerar ett löfte om en bool
        /// </returns>
        private async Task<bool> Communicate(string adress)
        {
            //var hostName = Dns.GetHostName();
            //IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(hostName);
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(adress); //maps adress to an ip-adress
            IPAddress ipAddress = ipHostInfo.AddressList[0]; //saves the ip-adress
            //IPAddress ipAddress = IPAddress.Any;

            IPEndPoint ipEndPoint = new(ipAddress, 1234); //Adds ip-adress to a port
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
        /// <summary>
        /// Overloadad metod
        /// 
        /// Används för att skicka meddelanden till clienten
        /// skickar konfirmation
        /// 
        /// tar meddelandet och lägger till ett nummer i slutet
        /// Anledningen till detta är att servern ska förstå att det är ett nytt meddelande
        /// då det skiljer sig från det gamla
        /// Enkrypterar sedan meddelandet och skickar det vidare
        /// </summary>
        /// <param name="_message">
        /// Sträng, Meddelandet som man vill skicka till servern
        /// Är i det här fallet konfirmation
        /// </param>
        public async void Send_message(string _message)
        {

            // Send message.
            var message = _message + $"{binary}"; //Message
            binary++;
            var messageBytes = Encoding.UTF8.GetBytes(message); //maps message to bytes
            _ = await this.client.SendAsync(messageBytes, SocketFlags.None); //Sends message over and waits for signal that it is done
                                                                             //Console.WriteLine($"Socket client sent message: \"{message}\""); //prints to console
            /*while (true)//While for communicating with other computer
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
            }*/
        }
        /// <summary>
        /// Kör samma sak som ovan nämnda
        /// med skillnaden att användaren själv får skriva in meddelandet som ska skickas till servern
        /// </summary>
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
        /// <summary>
        /// Lystnar efter meddelanden från servern
        /// 
        /// Väntar på meddelanden från server
        /// Om den får ett meddelanden kollar den om det är kort/input/vinnare
        /// Den avgör detta genom att se om det står c i början
        /// om det står input
        /// eller om det står w
        /// Om det står c i början printar den korten för ena spelaren
        /// beroende på om player_print är sann/falsk
        /// om den är sann printar den players kort
        /// falsk, printar den ai:s kort
        /// 
        /// Detta gör den genom att dekryptera koden i bokstav-siffer par
        /// och sedan printar svaret.
        /// Metoden som gör detta är Code_to_card(Char, Char)
        /// 
        /// Om det istället ska vara input
        /// kallar den på send_message()
        /// 
        /// Om det blir w
        /// Printar den ut namnet som står i meddelandet
        /// </summary>
        /// <returns>
        /// Task<bool>, så att man kan vänta på den
        /// </returns>
        public async Task<bool> Receiver()
        {
            string ai_hand = "";
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
                        Console.WriteLine("\nPlayer:");
                        for (int i = 0; i < str_player[1].Length; i += 2)
                        {
                            Console.WriteLine(Code_to_card(str_player[1][i], str_player[1][i + 1]));
                        }
                        Console.WriteLine("Sum " + str_player[2]);
                        player_print = false;
                    }
                    else
                    {
                        Console.WriteLine("\nAi:");
                        Console.WriteLine("Unknown card");
                        for (int i = 2; i < str_player[1].Length; i += 2)//Check for arithmetic miss
                        {
                            Console.WriteLine(Code_to_card(str_player[1][i], str_player[1][i + 1]));
                        }
                        //Console.WriteLine("Sum " + str_player[2]);
                        player_print = true;
                        ai_hand = response;
                    }
                    this.Send_message("received");
                }
                else if (response[0] == 'w')
                {
                    string[] str_player = ai_hand.Split(" ");
                    Console.WriteLine("\nAi:");
                    for (int i = 0; i < str_player[1].Length; i += 2)//Check for arithmetic miss
                    {
                        Console.WriteLine(Code_to_card(str_player[1][i], str_player[1][i + 1]));
                    }
                    Console.WriteLine("Sum " + str_player[2]);
                    String winner = response.Substring(1);
                    Console.WriteLine("\nThe winner is: " + winner);
                }
            }
            Console.WriteLine("loop done");
            return true;
        }
        /// <summary>
        /// Tar två chars och omvandlar dem till information
        /// som objectet sedan kan printa ut
        /// 
        /// Den gör detta genom att ha en sträng där den lägger till olika strängar
        /// beroende på vilken case det blir i switchen
        /// </summary>
        /// <param name="suit">
        /// char, en bokstav (s/d/h/c)
        /// symboliserar kortets färg
        /// </param>
        /// <param name="value">
        /// char, Symboliserar kortets värde
        /// </param>
        /// <returns>
        /// Sträng, T.ex Spades Quenn
        /// </returns>
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
