using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBDesk.WebUtils.UrlShortner
{
    public class GoogleUrlReply
    {
        public string kind { get; set; }

        //is the short URL
        public string id { get; set; }

        //is the long URL to which it expands. 
        //longUrl may not be present in the response, for example, if status is "REMOVED".
        public string longUrl { get; set; }

        //is "OK" for most URLs. 
        //If Google believes that the URL is fishy, 
        //status may be something else, such as "MALWARE".
        public string status { get; set; }
    }
}
