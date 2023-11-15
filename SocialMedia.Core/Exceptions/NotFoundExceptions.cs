using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Core.Exceptions
{
    public class NotFoundExceptions: Exception
    {
        public NotFoundExceptions()
        {

        }
        public NotFoundExceptions(string message) : base(message)
        {

        }
    }
}
