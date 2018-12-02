using NetCoreTemplate.Application.Notifications.Models;
using System.Threading.Tasks;

namespace NetCoreTemplate.Application.Interfaces {
  public interface INotificationService {
    Task SendAsync(Message message);
  }
}
