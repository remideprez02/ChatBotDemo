using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatbotApp.Helpers
{
    public class SomeHelpers
    {
        public string GetImgByScore(Double? score)
        {
            if (score < 0.4)
            {
                return HttpContext.Current.Server.MapPath("~/Assets/decu.jpg");
            }

            if (score > 0.4 && score < 0.55)
            {
                return HttpContext.Current.Server.MapPath("~/Assets/neutre.jpg");
            }

            return HttpContext.Current.Server.MapPath("~/Assets/content.jpg");
        }
    }
}