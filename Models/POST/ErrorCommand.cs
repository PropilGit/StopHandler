using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.POST
{
    class ErrorCommand : IPOSTCommand
    {
        public string[] Tags { get => null; }
        public string Identifier { get => identifier; }
        public static readonly string identifier = "ERROR";
        public int TaskNum { get => 0; }
        public string Worker { get => "ERROR"; }
        public string Chat { get => "TEST"; }
        public string Message { get; private set; }

        public ErrorCommand(string message)
        {
            Message = message;
        }
        public static ErrorCommand Instantiate(string message)
        {
            try
            {
                return new ErrorCommand(message);
            }
            catch (Exception)
            {

            }
            return null;
        }
        
        public string GenerateMessage()
        {
            return Message;
        }
    }
}
