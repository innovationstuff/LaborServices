using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LaborServices.Entity;
using LaborServices.Entity.Identity;
using LaborServices.Model.Identity;
using Microsoft.Owin;

namespace LaborServices.Managers.Identity
{
    public class ApplicationGroupManager
    {
        #region variabes
        private readonly ApplicationGroupStore _groupStore;
        private readonly LaborServicesDbContext _db;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        #endregion

        public ApplicationGroupManager(IOwinContext context)
        {
            _db = context.Get<LaborServicesDbContext>();
            _userManager = context.GetUserManager<ApplicationUserManager>();
            _roleManager = context.Get<ApplicationRoleManager>();
            _groupStore = new ApplicationGroupStore(_db);
        }

        #region getters
        public IQueryable<ApplicationGroup> Groups => _groupStore.Groups;

        public IEnumerable<ApplicationUser> GetGroupUsers(string groupId)
        {
            var group = this.FindById(groupId);
            var users = new List<ApplicationUser>();
            var allUsers = _userManager.Users.ToList();
            foreach (var groupUser in group.ApplicationUsers)
            {
                var user = allUsers.FirstOrDefault(u => u.Id == groupUser.ApplicationUserId);
                if (user == null) continue;
                users.Add(user);
            }
            return users;
        }
        public async Task<IEnumerable<ApplicationUser>> GetGroupUsersAsync(string groupId)
        {
            var group = await this.FindByIdAsync(groupId);
            var allUsers = await _userManager.Users.ToListAsync();
            var users = new List<ApplicationUser>();
            foreach (var groupUser in group.ApplicationUsers)
            {
                var user = allUsers.FirstOrDefault(u => u.Id == groupUser.ApplicationUserId);
                if (user == null) continue;
                users.Add(user);
            }
            return users;
        }

        public KeyValuePair<int, List<ApplicationGroup>> SearchAllPaging(string keyword, int pageSize = 10, int pageNumber = 1)
        {
            int start = ((pageNumber - 1) * pageSize);
            if (start < 0) start = 0;

            var groups = Groups;

            if (string.IsNullOrEmpty(keyword) == false)
            {
                keyword = keyword.ToLower();
                groups = groups.Where(p => p.Name.ToLower().Contains(keyword)
                || p.Description.ToLower().Contains(keyword));
            }
            var filterdItems = groups.ToList();
            return new KeyValuePair<int, List<ApplicationGroup>>(filterdItems.Count(), pageSize == 0 ? filterdItems : filterdItems.Skip(start).Take(pageSize).ToList());
        }
        #endregion

        #region Setters
        public IdentityResult CreateGroup(ApplicationGroup group)
        {
            _groupStore.Create(group);
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> CreateGroupAsync(ApplicationGroup group)
        {
            await _groupStore.CreateAsync(group);
            return IdentityResult.Success;
        }

        public IdentityResult UpdateGroup(ApplicationGroup group)
        {
            _groupStore.Update(group);
            foreach (var groupUser in group.ApplicationUsers)
            {
                this.RefreshUserGroupRoles(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> UpdateGroupAsync(ApplicationGroup group)
        {
            await _groupStore.UpdateAsync(group);
            foreach (var groupUser in group.ApplicationUsers)
            {
                await this.RefreshUserGroupRolesAsync(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }

        public IdentityResult DeleteGroup(string groupId)
        {
            var group = this.FindById(groupId);
            if (group == null)
            {
                throw new ArgumentNullException("User");
            }

            var currentGroupMembers = this.GetGroupUsers(groupId).ToList();
            // remove the roles from the group:
            group.ApplicationRoles.Clear();

            // Remove all the users:
            group.ApplicationUsers.Clear();

            // Remove the group itself:
            _db.ApplicationGroups.Remove(group);

            _db.SaveChanges();

            // Reset all the user roles:
            foreach (var user in currentGroupMembers)
            {
                this.RefreshUserGroupRoles(user.Id);
            }
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> DeleteGroupAsync(string groupId)
        {
            var group = await this.FindByIdAsync(groupId);
            if (group == null)
            {
                throw new ArgumentNullException("User");
            }

            var currentGroupMembers = (await this.GetGroupUsersAsync(groupId)).ToList();
            // remove the roles from the group:
            group.ApplicationRoles.Clear();

            // Remove all the users:
            group.ApplicationUsers.Clear();

            // Remove the group itself:
            _db.ApplicationGroups.Remove(group);

            await _db.SaveChangesAsync();

            // Reset all the user roles:
            foreach (var user in currentGroupMembers)
            {
                await this.RefreshUserGroupRolesAsync(user.Id);
            }
            return IdentityResult.Success;
        }
        #endregion

        #region user

        public IEnumerable<ApplicationGroup> GetUserGroups(string userId)
        {
            var userGroups = (from g in this.Groups
                              where g.ApplicationUsers
                                  .Any(u => u.ApplicationUserId == userId)
                              select g).ToList();
            return userGroups;
        }
        public async Task<IEnumerable<ApplicationGroup>> GetUserGroupsAsync(string userId)
        {
            var userGroups = (from g in this.Groups
                              where g.ApplicationUsers
                                  .Any(u => u.ApplicationUserId == userId)
                              select g).ToListAsync();
            return await userGroups;
        }

        public bool IsUserBlongToGroup(string userId, string groupId)
        {
            var userGroups = this.GetUserGroups(userId).ToList();
            return userGroups.Any() && userGroups.Any(g => g.Id == groupId);
        }
        public async Task<bool> IsUserBlongToGroupAsync(string userId, string groupId)
        {
            var userGroups = await this.GetUserGroupsAsync(userId);
            var userGroupsArr = userGroups as ApplicationGroup[] ?? userGroups.ToArray();
            return userGroupsArr.Any() && userGroupsArr.Any(g => g.Id == groupId);
        }

        public IdentityResult ClearUserGroups(string userId)
        {
            return this.SetUserGroups(userId, new string[] { });
        }
        public async Task<IdentityResult> ClearUserGroupsAsync(string userId)
        {
            return await this.SetUserGroupsAsync(userId, new string[] { });
        }

        /// <summary>
        /// @TODO Count on for user specific role outside group
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IdentityResult RefreshUserGroupRoles(string userId)
        {
            var user = _userManager.FindById(userId);
            if (user == null)
            {
                throw new ArgumentNullException("User");
            }

            // Find teh roles this user is entitled to from group membership:
            var newGroupRoles = this.GetUserGroupRoles(userId);

            // Remove user from previous roles:
            var oldUserRoles = _userManager.GetRoles(userId);
            if (oldUserRoles.Count > 0 && newGroupRoles != null && newGroupRoles.Any())
            {
                _userManager.RemoveFromRoles(userId, oldUserRoles.ToArray());
            }

           

            // Get the damn role names:
            var allRoles = _roleManager.Roles.ToList();
            var addTheseRoles = allRoles.Where(r => newGroupRoles.Any(gr => gr.ApplicationRoleId == r.Id));

            var roleNames = addTheseRoles.Select(n => n.Name).ToArray();

            // Add the user to the proper roles
            _userManager.AddToRoles(userId, roleNames);

            return IdentityResult.Success;
        }
        public async Task<IdentityResult> RefreshUserGroupRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentNullException("User");
            }

            // Find the roles this user is entitled to from group membership:
            var newGroupRoles = await this.GetUserGroupRolesAsync(userId);

            // Remove user from previous roles:
            var oldUserRoles = await _userManager.GetRolesAsync(userId);
            if (oldUserRoles.Count > 0 && newGroupRoles != null && newGroupRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(userId, oldUserRoles.ToArray());
            }



            // Get the damn role names:
            var allRoles = await _roleManager.Roles.ToListAsync();

            var addTheseRoles = allRoles.Where(r => newGroupRoles.Any(gr => gr.ApplicationRoleId == r.Id));

            var roleNames = addTheseRoles.Select(n => n.Name).ToArray();

            // Add the user to the proper roles
            await _userManager.AddToRolesAsync(userId, roleNames);

            return IdentityResult.Success;
        }

        public IdentityResult SetUserGroups(string userId, params string[] groupIds)
        {
            // Clear current group membership:
            var currentGroups = this.GetUserGroups(userId);
            foreach (var group in currentGroups)
            {
                group.ApplicationUsers
                    .Remove(group.ApplicationUsers
                        .FirstOrDefault(gr => gr.ApplicationUserId == userId
                        ));
            }
            _db.SaveChanges();

            // Add the user to the new groups:
            foreach (string groupId in groupIds)
            {
                var newGroup = this.FindById(groupId);
                newGroup.ApplicationUsers.Add(new ApplicationUserGroup
                {
                    ApplicationUserId = userId,
                    ApplicationGroupId = groupId
                });
            }
            _db.SaveChanges();

            this.RefreshUserGroupRoles(userId);
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> SetUserGroupsAsync(string userId, params string[] groupIds)
        {
            // Clear current group membership:
            var currentGroups = await this.GetUserGroupsAsync(userId);
            foreach (var group in currentGroups)
            {
                group.ApplicationUsers
                    .Remove(group.ApplicationUsers
                        .FirstOrDefault(gr => gr.ApplicationUserId == userId
                        ));
            }
            await _db.SaveChangesAsync();

            // Add the user to the new groups:
            foreach (string groupId in groupIds)
            {
                var newGroup = await this.FindByIdAsync(groupId);
                newGroup.ApplicationUsers.Add(new ApplicationUserGroup
                {
                    ApplicationUserId = userId,
                    ApplicationGroupId = groupId
                });
            }
            await _db.SaveChangesAsync();

            await this.RefreshUserGroupRolesAsync(userId);
            return IdentityResult.Success;
        }

        public IdentityResult SetUsersGroup(string groupId, string[] usersIds)
        {
            ApplicationGroup currentGroup = this.FindById(groupId);
            currentGroup.ApplicationUsers.Clear();
            _db.SaveChangesAsync();

            currentGroup.ApplicationUsers = usersIds.Select(userId => new ApplicationUserGroup()
            {
                ApplicationGroupId = currentGroup.Id,
                ApplicationUserId = userId
            }).ToList();

            _db.SaveChanges();

            foreach (var userId in usersIds)
            {
                this.RefreshUserGroupRoles(userId);
            }
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> SetUsersGroupAsync(string groupId, string[] usersIds)
        {
            ApplicationGroup currentGroup = await this.FindByIdAsync(groupId);
            currentGroup.ApplicationUsers.Clear();
            await _db.SaveChangesAsync();

            currentGroup.ApplicationUsers = usersIds.Select(userId => new ApplicationUserGroup()
            {
                ApplicationGroupId = currentGroup.Id,
                ApplicationUserId = userId
            }).ToList();

            await _db.SaveChangesAsync();

            foreach (var userId in usersIds)
            {
                await this.RefreshUserGroupRolesAsync(userId);
            }
            return IdentityResult.Success;
        }

        #endregion

        #region roles

        #region User

        public IdentityResult SetUserRoles(string userId, string[] rolesIds)
        {
            // Clear all the roles associated with this group:
            var thisUser = this._userManager.FindById(userId);
            thisUser.Roles.Clear();
            _db.SaveChanges();

            foreach (var roleId in rolesIds)
            {
                thisUser.Roles.Add(new ApplicationUserRole()
                {
                    RoleId = roleId,
                    UserId = userId
                });
            }
            _db.SaveChanges();

            this.RefreshUserGroupRoles(userId);
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> SetUserRolesAsync(string userId, string[] rolesIds)
        {
            var thisUser = await this._userManager.FindByIdAsync(userId);
            thisUser.Roles.Clear();
            _db.SaveChanges();

            foreach (var roleId in rolesIds)
            {
                thisUser.Roles.Add(new ApplicationUserRole()
                {
                    RoleId = roleId,
                    UserId = userId
                });
            }
            await _db.SaveChangesAsync();

            await this.RefreshUserGroupRolesAsync(userId);
            return IdentityResult.Success;
        }
        #endregion

        #region groups

        public IdentityResult SetGroupRoles(string groupId, string[] roleIds)
        {
            // Clear all the roles associated with this group:
            var thisGroup = this.FindById(groupId);
            thisGroup.ApplicationRoles.Clear();
            _db.SaveChanges();

            // Add the new roles passed in:
            foreach (var roleId in roleIds)
            {
                thisGroup.ApplicationRoles.Add(new ApplicationGroupRole
                {
                    ApplicationGroupId = groupId,
                    ApplicationRoleId = roleId
                });
            }
            _db.SaveChanges();

            // Reset the roles for all affected users:
            foreach (var groupUser in thisGroup.ApplicationUsers)
            {
                this.RefreshUserGroupRoles(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> SetGroupRolesAsync(string groupId, string[] roleIds)
        {
            // Clear all the roles associated with this group:
            var thisGroup = await this.FindByIdAsync(groupId);
            thisGroup.ApplicationRoles.Clear();
            await _db.SaveChangesAsync();

            // Add the new roles passed in:
            foreach (var roleId in roleIds)
            {
                thisGroup.ApplicationRoles.Add(new ApplicationGroupRole
                {
                    ApplicationGroupId = groupId,
                    ApplicationRoleId = roleId
                });
            }
            await _db.SaveChangesAsync();

            // Reset the roles for all affected users:
            foreach (var groupUser in thisGroup.ApplicationUsers)
            {
                await this.RefreshUserGroupRolesAsync(groupUser.ApplicationUserId);
            }
            return IdentityResult.Success;
        }

        public IEnumerable<ApplicationRole> GetGroupRoles(string groupId)
        {
            var grp = _db.ApplicationGroups.FirstOrDefault(g => g.Id == groupId);
            var roles = _roleManager.Roles.ToList();

            var groupRoles = from r in roles
                             where grp != null &&
                                   grp.ApplicationRoles.Any(ap => ap.ApplicationRoleId == r.Id)
                             select r;
            return groupRoles;
        }
        public async Task<IEnumerable<ApplicationRole>> GetGroupRolesAsync(string groupId)
        {
            var grp = await _db.ApplicationGroups.FirstOrDefaultAsync(g => g.Id == groupId);
            var roles = await _roleManager.Roles.ToListAsync();
            var groupRoles = (from r in roles
                              where grp != null &&
                                    grp.ApplicationRoles.Any(ap => ap.ApplicationRoleId == r.Id)
                              select r).ToList();
            return groupRoles;
        }

        public bool IsGroupHasRole(string groupId, string roleId)
        {
            var groupRoles = this.GetGroupRoles(groupId).ToList();
            return groupRoles.Any() && groupRoles.Any(g => g.Id == roleId);
        }
        public async Task<bool> IsGroupHasRoleAsync(string groupId, string roleId)
        {
            var groupRoles = await this.GetGroupRolesAsync(groupId);
            var groupRolesArr = groupRoles as ApplicationRole[] ?? groupRoles.ToArray();
            return groupRolesArr.Any() && groupRolesArr.Any(g => g.Id == roleId);
        }

        public IEnumerable<ApplicationGroup> GetRoleGroups(string roleId)
        {
            var roleGroups = (from g in this.Groups
                              where g.ApplicationRoles
                                   .Any(u => u.ApplicationRoleId == roleId)
                              select g).ToList();
            return roleGroups;
        }
        public async Task<IEnumerable<ApplicationGroup>> GetRoleGroupsAsync(string roleId)
        {
            var roleGroups = (from g in this.Groups
                              where g.ApplicationRoles
                                  .Any(u => u.ApplicationRoleId == roleId)
                              select g).ToListAsync();
            return await roleGroups;
        }

        #endregion

        #endregion

        #region User group Roles

        public IEnumerable<ApplicationGroupRole> GetUserGroupRoles(string userId)
        {
            var userGroups = this.GetUserGroups(userId);
            var userGroupRoles = new List<ApplicationGroupRole>();
            foreach (var group in userGroups)
            {
                if (group == null || group.ApplicationRoles == null) continue;
                userGroupRoles.AddRange(group.ApplicationRoles.ToArray());
            }
            return userGroupRoles;
        }

        public async Task<IEnumerable<ApplicationGroupRole>> GetUserGroupRolesAsync(string userId)
        {
            var userGroups = await this.GetUserGroupsAsync(userId);
            var userGroupRoles = new List<ApplicationGroupRole>();
            foreach (var group in userGroups)
            {
                if (group == null || group.ApplicationRoles == null) continue;
                userGroupRoles.AddRange(group.ApplicationRoles.ToArray());
            }
            return userGroupRoles;
        }

        #endregion

        #region  Find Group
        public ApplicationGroup FindById(string id)
        {
            return _groupStore.FindById(id);
        }

        public async Task<ApplicationGroup> FindByIdAsync(string id)
        {
            return await _groupStore.FindByIdAsync(id);
        }


        public ApplicationGroup FindByName(string name)
        {
            return _groupStore.FindByName(name);
        }

        public async Task<ApplicationGroup> FindByNameAsync(string name)
        {
            return await _groupStore.FindByNameAsync(name);
        }

        #endregion
    }
}
