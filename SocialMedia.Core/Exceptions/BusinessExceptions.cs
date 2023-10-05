using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Core.Exceptions
{
    public class BusinessExceptions: Exception
    {
        public BusinessExceptions()
        {

        }
        public BusinessExceptions(string message): base(message)
        {

        }
    }
}
