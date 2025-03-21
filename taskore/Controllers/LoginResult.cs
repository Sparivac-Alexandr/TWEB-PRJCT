using System;

namespace taskore.Controllers
{
    internal class LoginResult
    {
        public bool Status { get; internal set; }
        public Exception StatusMsg { get; internal set; }
    }
}