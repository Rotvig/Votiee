using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VotieeBackend.Models
{
    public class Vote
    {
        [Key]
        public int VoteId { get; set; }

        public virtual AnswerArchived Answer { get; set; }

        public string UserConnectionId { get; set; }
    }
}