using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public class Notice : INotice
    {
        public string Id { get; set; }
        public string AuthorName { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public List<NoticeTag> Tags { get; set; }
        public List<string> ReadBy { get; set; }

        public Notice()
        {
            ReadBy = new List<string>();
            Id = Guid.NewGuid().ToString();
        }
    }
}
