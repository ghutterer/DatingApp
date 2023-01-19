using System.Security.Claims;
using Api.Data;
using Api.DTOs;
using Api.Entities;
using Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {

        public IUserRepository UserRepository { get; }
        private readonly IMapper mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)

        {
            this.mapper = mapper;
            this.UserRepository = userRepository;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = (await this.UserRepository.GetMembersAsync());

            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await this.UserRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await this.UserRepository.GetUserByUsernameAsync(username);

            if (user == null) return NotFound();

            this.mapper.Map(memberUpdateDto, user);

            if (await this.UserRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");

        }


    }
}
