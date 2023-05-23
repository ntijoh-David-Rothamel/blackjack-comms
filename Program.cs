namespace com_client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Communication_client();
            while (true)
            {
                client.Receiver().Wait();
            }
        }
    }
}