﻿using JiraCloneMVC.Web.Models;
using System.Collections.Generic;

namespace JiraCloneMVC.Web.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        IEnumerable<User> GetAllFromProject(int projectId);
        IEnumerable<User> GetUsersInProjectsByRole(string role, int projectId);
        IEnumerable<User> GetNonMembersOfProject(int projectId);
    }
}
