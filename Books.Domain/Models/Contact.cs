using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tele.Domain.Models
{
    [Table("Contacts")]
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        public string Phone { get; set; }
        public string Username { get; set; }
        public long? AccessHash { get; set; }
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LangCode { get; set; }
        public bool Verified { get; set; }
        public bool BotNochats { get; set; }
        public bool BotChatHistory { get; set; }
        public bool Bot { get; set; }
        public bool Deleted { get; set; }
        public bool MutualContact { get; set; }
        public bool IsContact { get; set; }
        public bool Self { get; set; }
        public int Flags { get; set; }
        public string BotInlinePlaceholder { get; set; }
    }
}
