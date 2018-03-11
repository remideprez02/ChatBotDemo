using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatbotApp.Models.Qna
{
    public class QnaQueryModel
    {
        public string Question { get; set; }
        public int Top { get; set; }
    }
}