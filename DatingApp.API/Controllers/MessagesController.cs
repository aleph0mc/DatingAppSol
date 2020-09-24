using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(UserActivityActionFilter))]
    [Route("api/[controller]/{userid}")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _Repo;
        private readonly IMapper _Mapper;

        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _Repo = repo;
            _Mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userid, int id)
        {
            if (userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _Repo.GetMessage(id);

            if (null == messageFromRepo)
                return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userid, [FromQuery] MessageParams messageParams)
        {
            if (userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserId = userid;

            var messagesFromRepo = await _Repo.GetMessagesForUser(messageParams);

            var messages = _Mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,
                messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);

        }

        [HttpGet("thread/{recipientid}")] // to differentiate the path with GetMessagesForUser method
        public async Task<IActionResult> GetMessageThread(int userid, int recipientid)
        {
            if (userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messagesFromRepo = await _Repo.GetMessageThread(userid, recipientid);

            var messageThread = _Mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userid, MessageForCreationDto messageForCreationDto)
        {
            //This is used automatically by Automapper to get the sender info for the object is in memory
            var sender = await _Repo.GetUser(userid, true);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userid;

            //This is used automatically by Automapper to get the sender info for the object is in memory
            var recipent = await _Repo.GetUser(messageForCreationDto.RecipientId, false);

            if (null == recipent)
                return BadRequest("Could not find user");

            var message = _Mapper.Map<Message>(messageForCreationDto);

            _Repo.Add(message);

            // Using the ReverseMap method of Automapper
            //var messageToReturn = _Mapper.Map<MessageForCreationDto>(message);

            if (await _Repo.SaveAll())
            {
                var messageToReturn = _Mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new { userid, id = message.Id }, messageToReturn);
            }
            throw new Exception("Creating the message failed on save");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int userid, int id)
        {
            if (userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _Repo.GetMessage(id);

            if (messageFromRepo.SenderId == userid)
                messageFromRepo.SenderDeleted = true;

            if (messageFromRepo.RecipientId == userid)
                messageFromRepo.RecipientDeleted = true;

            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                _Repo.Delete(messageFromRepo);

            if (await _Repo.SaveAll())
                return NoContent();

            throw new Exception("Error deleting the message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userid, int id)
        {
            if (userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var message = await _Repo.GetMessage(id);

            if (null == message)
                return BadRequest("Message not found");

            if (message.RecipientId != userid)
                return Unauthorized();

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await _Repo.SaveAll();

            return NoContent();
        }
    }
}
