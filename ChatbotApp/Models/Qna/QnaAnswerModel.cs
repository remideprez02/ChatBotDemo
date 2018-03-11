using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatbotApp.Models.Qna
{
    public class QnaAnswerModel
    {
        public string Answer { get; set; }
        public List<string> Questions { get; set; }
        public double Score { get; set; }
    }
}