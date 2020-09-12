using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels
{
    public class SheetsSignupException:Exception
    {
        public SheetsSignupException(string message, Exception innerException=null):base($"Error in signup: {message}", innerException)
        {            
        }
    }
}
