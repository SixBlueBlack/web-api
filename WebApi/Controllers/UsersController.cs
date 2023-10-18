﻿using System;
using System.Linq;
using AutoMapper;
using Game.Domain;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserRepository userRepository;

        private readonly IMapper mapper;

        // Чтобы ASP.NET положил что-то в userRepository требуется конфигурация
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        [HttpGet("{userId}", Name = nameof(GetUserById))]
        [Produces("application/json", "application/xml")]
        public ActionResult<UserDto> GetUserById([FromRoute] Guid userId)
        {
            var userEntity = userRepository.FindById(userId);
            return userEntity is null ? NotFound() : Ok(mapper.Map<UserDto>(userEntity));
        }

        [HttpPost]
        [Produces("application/json", "application/xml")]
        public IActionResult CreateUser([FromBody] CreateDto userCreateDto)
        {
            if (userCreateDto is null) return BadRequest();
            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
            if (!userCreateDto.Login.All(char.IsLetterOrDigit))
            {
                ModelState.AddModelError("Login", "Login should contain only letters or digits");
                return UnprocessableEntity(ModelState);
            }

            var userEntity = mapper.Map<UserEntity>(userCreateDto);
            userEntity = userRepository.Insert(userEntity);
            return CreatedAtRoute(
                nameof(GetUserById),
                new {userId = userEntity.Id},
                userEntity.Id);
        }

        [HttpPut("{userId}")]
        [Produces("application/json", "application/xml")]
        public IActionResult UpdateUser([FromRoute] Guid userId, [FromBody] UpdateDto userUpdateDto)
        {
            if (userUpdateDto is null || userId == Guid.Empty)
                return BadRequest();
            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
            
            var userEntity = new UserEntity(userId);
            mapper.Map(userUpdateDto, userEntity);
            userRepository.UpdateOrInsert(userEntity, out var isInserted);
            return !isInserted
                ? NoContent()
                : CreatedAtRoute(
                    nameof(GetUserById),
                    new {userId = userEntity.Id},
                    userEntity.Id);
        }
    }
}