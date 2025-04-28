using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using efilling_api.Models;
using Microsoft.VisualBasic;
using System.Security.Cryptography.X509Certificates;

namespace efilling_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserNotificationsController : ControllerBase
    {
        // DI 
        private readonly EFilling_DBContext _context;
        public UserNotificationsController(EFilling_DBContext context)
        {
            _context = context;
        }

        // Get all notifications in the DB (for Testing only)
        // GET: api/UserNotifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserNotification>>> GetUserNotifications()
        {
          if (_context.UserNotifications == null)
          {
              return NotFound();
          }
            return await _context.UserNotifications.ToListAsync();
        }

        //  ---------------------- Notifications Page ----------------------------
        // 1
        // Get list of all Notifications for a user
        // GET: api/UserNotifications/GetUserNotifications/{userId}
        [HttpGet("GetUserNotifications/{userId}")]
        public async Task<ActionResult<IEnumerable<UserNotification>>> GetUserNotifications(int? userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            // get all notifications related to that user
            var userNotificationsList = await( _context.UserNotifications
                                            .Where(n => n.UserId == userId)
                                            .Include( n => n.Case)
                                            .Select( n => new
                                            {
                                                notifId = n.Id,
                                                caseNo = n.CaseId,
                                                caseTitle = n.Case.CaseTitle,
                                                Subject = n.title,
                                                //filedBy = ,
                                                date = n.Created_at
                                            })
                                            .ToListAsync());
            
            if (userNotificationsList == null || userNotificationsList.Count == 0)
            {
                return NotFound();
            }

            // return a list of the notifications 
            return Ok(userNotificationsList);
        }

        // 2
        // Get details of a notification (get notification by id - to show details of a notification)
        // GET: api/UserNotifications/GetNotificationById/{Id}
        [HttpGet("GetNotificationById/{Id}")]
        public async Task<ActionResult<UserNotification>> GetNotificationById(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var notification = await _context.UserNotifications.FindAsync(Id);
            
            if (notification == null)
            {
                return NotFound();
            }

            return notification;
        }

        //  ---------------------- Dashboard Page ----------------------------
        // 3
        // List last 15 unread notifications on Dashboard page
        // GET: api/UserNotifications/GetUserDashboardNotifications/{userId}
        [HttpGet("GetUserDashboardNotifications/{userId}")]
        public ActionResult<IEnumerable<UserNotification>> GetUserDashboardNotifications (int? userId)
        {
            if (userId == null)
            {
                return NotFound("No user exist with this id");
            }

            var notifications = _context.UserNotifications
                                        .Where(n => (n.UserId == userId) && (n.isRead == false))
                                        .Take(15);

            if (notifications == null) 
            {
                return NotFound("No notifications found");
            }

            return notifications.ToList();
        }


    }
}
