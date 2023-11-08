namespace UdemyAuthServer.Core.Configuration
{
    public class Client
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public List<string> Audiences { get; set; } // bu şuna denk geliyor bu client benim apılarımdan hangilerine erişecek bunu belirticez - gönderecegi tokenlarda hangi apı erişecegi bilgisi
    }
}
