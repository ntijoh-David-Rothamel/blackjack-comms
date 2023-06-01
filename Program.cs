namespace com_client
{
    class Program
    {
        //Instantierar clienten
        static void Main(string[] args)
        {
            var client = new Communication_client();
            while (true) //Gör så att clienten håller igång
            {
                client.Receiver().Wait(); 
            }
        }
    }
}