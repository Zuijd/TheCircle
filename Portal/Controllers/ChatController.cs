using Domain;
using DomainServices.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;

namespace Portal.Controllers
{
    public class ChatController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        public ChatController(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Message([FromBody] ChatViewModel chatViewModel)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    //Console.WriteLine(chatViewModel.);
                    Message message = new()
                    {
                        User = _userService.GetUserByName(User.Identity?.Name!).Result,
                        MessageBody = chatViewModel.Message!
                    };


                    await _messageService.CreateMessage(message);

                }
            }
            catch (Exception e)
            {

                ModelState.AddModelError(e.Message, e.Message);
            }

            Console.WriteLine(chatViewModel.ViewName);

            if (chatViewModel.ViewName == "Stream Index")
            {
                return RedirectToAction("Index", "Stream");
            }
            else {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
