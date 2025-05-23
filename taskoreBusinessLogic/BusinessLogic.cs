﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.BL_Struct;
using taskoreBusinessLogic.Interfaces;

namespace taskoreBusinessLogic
{
    public class BusinessLogic
    {
      public ISesion GetSesionBL()
        {
            return new SessionBL();
        }

        public IAuth GetAuthBL()
        {
            return new AuthBL();
        }
        
        public IProject GetProjectBL()
        {
            return new ProjectBL();
        }
        
        public INews GetNewsBL()
        {
            return new NewsBL();
        }
        
        public IReview GetReviewBL()
        {
            return new ReviewBL();
        }
        
        public IUser GetUserBL()
        {
            return new UserBL();
        }
        
        public IChat GetChatBL()
        {
            return new ChatBL();
        }
    }
}
