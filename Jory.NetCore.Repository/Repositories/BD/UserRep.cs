using System;
using System.Collections.Generic;
using System.Text;
using Jory.NetCore.Core.Enums;
using Jory.NetCore.Repository.IRepositories.BD;
using Microsoft.EntityFrameworkCore;

namespace Jory.NetCore.Repository.Repositories.BD
{
    public class UserRep : BaseRep, IUserRep
    {
        public UserRep(DbContext context, DbCategory dbCategory) : base(context, dbCategory)
        {

        }
    }
}
