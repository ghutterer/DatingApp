using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs;
using Api.Entities;
using Api.Helpers;

namespace Api.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);

        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByUsernameAsync(string username);

        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

        Task<MemberDto> GetMemberAsync(string username, bool isCurrentUser);

        Task<string> GetUserGender(string username);

        Task<AppUser> GetUserByPhotoId(int photoId);

    }
}