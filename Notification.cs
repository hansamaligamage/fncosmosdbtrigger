namespace cosmosdbtrigger
{
    class Notification
    {
        public string user_id { get; set; }
        public string api_key { get; set; }
        public string sender_id { get; set; }
        public string to { get; set; }
        public string message { get; set; }
    }
}
