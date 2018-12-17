
using System.Threading.Tasks;

namespace NetCoreTemplate.Infrastructure {
  public class NotificationService : INotificationService {
    public Task SendAsync(Message message) {
      return Task.CompletedTask;
    }
  }
}
