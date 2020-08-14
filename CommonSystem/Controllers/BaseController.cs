using ApplicationLayer.EntityDto.BaseDto;
using Microsoft.AspNetCore.Mvc;

namespace CommonSystem.Controllers
{
    public class BaseController : Controller
    {
        public CurrentUserDto GetCurrentUser()
        {
            return new CurrentUserDto
            {
                UserId = User.Identity.GetLoginUserId(),
                LoginName = User.Identity.Name
            };
        }
    }
}