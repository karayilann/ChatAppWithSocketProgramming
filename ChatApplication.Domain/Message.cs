namespace ChatApplication.Domain
{
    public class Message
    {
        //public string Sender { get; set; }  
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public Message(string content)
        {
            //Sender = sender;
            Content = content;
            Timestamp = DateTime.Now;
        }

        // This will edit for receiving file
        public Message(string fileName, long fileSize)
        {
            Content = $"FILE: {fileName}|SIZE: {fileSize}";
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Content}";
            //return $"[{Timestamp}] {Sender}: {Content}";
        }
    }
}