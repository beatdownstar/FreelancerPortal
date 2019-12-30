using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProsjektoppgaveITE1811Gruppe7.Models;
using ProsjektoppgaveITE1811Gruppe7.Models.Entities;

namespace ProsjektoppgaveITE1811Gruppe7.Controllers
{
    [Route("api/Chat")]
    [ApiController]
    [Authorize]
    public class ChatAPIController : ControllerBase
    {
        private readonly IChatRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public ChatAPIController(IChatRepository repository, UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // GET: api/MessageApi
        [HttpGet("{id}")]
        public IActionResult GetMessages([FromRoute] int id)
        {
            var messages = _repository.GetMessagesByOrderId(id);
            return Ok(messages);
        }

        // POST: api/MessageApi
        [HttpPost]
        public async Task<IActionResult> Message([FromBody] ChatMessageModel message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _repository.CreateMessage(message, User);

            return Ok(201);
        }


    }
}