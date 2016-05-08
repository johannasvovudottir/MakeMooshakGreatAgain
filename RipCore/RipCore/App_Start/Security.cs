using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore
{
    #region Security Declerations
    public enum SecurityState { USER = 0, STUDENT, TEACHER, ADMIN };
    public enum SecurityMessage { NOTLOGGED = 0, NOENTRY, ENTRY };
    public struct SecurityRedirect
    {
        public bool Redirect { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
    };
    #endregion
    public class Security
    {

    }
}