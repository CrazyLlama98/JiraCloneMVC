using System;

namespace JiraCloneMVC.Web.Errors
{
    public class EntryNotFoundException : Exception
    {
        public EntryNotFoundException()
        { }

        public EntryNotFoundException(string message) : base(message)
        { }
    }
}