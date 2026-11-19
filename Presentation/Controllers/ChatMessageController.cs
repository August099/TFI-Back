using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Models.Message;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Cliente,Admin,Recepcionista")]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatMessageController : ControllerBase
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatMessageController(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        [HttpGet("chat/{chatId}")]
        public async Task<ActionResult<List<MessageDTO>>> GetByChatId([FromRoute] int chatId)
        {
            return Ok(await _chatMessageService.GetByChatIdAsync(chatId));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MessageDTO>> GetById([FromRoute] int id)
        {
            return Ok(await _chatMessageService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> Add([FromBody] CreateMessageDTO message)
        {
            var senderIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(senderIdClaim, out var senderId))
                return Forbid();

            var created = await _chatMessageService.AddAsync(message, senderId);

            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { id = created.Id },
                value: created
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MessageDTO>> Update(
            [FromRoute] int id,
            [FromBody] UpdateMessageDTO message)
        {
            return Ok(await _chatMessageService.UpdateAsync(id, message));
        }
    }
}
