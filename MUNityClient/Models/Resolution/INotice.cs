using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public interface INotice
    {
        string Id { get; set; }

        string AuthorName { get; set; }

        string AuthorId { get; set; }

        DateTime CreationDate { get; set; }

        string Title { get; set; }

        string Text { get; set; }

        List<NoticeTag> Tags { get; set; }

        List<string> ReadBy { get; set; }
    }
}
