using AmbientContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;


//High - level modules(business logic / policy) should not depend on low-level modules (implementation details like database or IO). Both should depend on abstractions (interfaces).

//Abstractions should not depend on details. Details should depend on abstractions.

namespace DesignPatterns.Solid.DIP.BadExample
{
    public class EmailService
    {
        public void SendEmail(string message)
        {
            // Low-level detail: Specific logic to connect to SMTP server
            Console.WriteLine($"Sending email: {message}");
        }
    }


    /// <summary>
    /// Tight Coupling: NotificationManager is glued to EmailService. You cannot use this manager to send SMS or Push Notifications without rewriting it.
    /// 
    /// Hard to Test: You cannot test NotificationManager without actually sending real emails (because you can't swap in a fake/mock service).
    /// </summary>
    public class NotificationManager
    {
        private EmailService _emailService;

        public NotificationManager()
        {
            // VIOLATION: Hard-coded dependency on a concrete class.
            // If we want to switch to SMS, we must change this code.
            _emailService = new EmailService();
        }

        public void Notify(string message)
        {
            _emailService.SendEmail(message);
        }
    }
}

namespace DesignPatterns.Solid.DIP.GoodExample
{
    /// <summary>
    /// We introduce an interface (IMessageService). The NotificationManager now depends on this interface, and the specific services (EmailService, SmsService) also implement this interface. The dependency is "injected" via the constructor.
    /// 
    /// Loose Coupling: The NotificationManager doesn't care if it's sending an Email or an SMS; it just knows it can call Send().
    /// 
    /// Testability: For unit tests, you can inject a FakeMessageService that does nothing, so you don't send real emails during testing.

    ///Extensibility: You can add PushNotificationService later without touching the NotificationManager code at all.
    /// </summary>
    /// 
    // 1. The Abstraction (The "Socket")
    public interface IMessageService
    {
        void Send(string message);
    }

    // 2. Low-Level Modules (The "Appliances")
    public class EmailService : IMessageService
    {
        public void Send(string message) => Console.WriteLine($"Email sent: {message}");
    }

    public class SmsService : IMessageService
    {
        public void Send(string message) => Console.WriteLine($"SMS sent: {message}");
    }

    // 3. High-Level Module (The "House Wiring")
    public class NotificationManager
    {
        private readonly IMessageService _messageService;

        // DIP applied: We ask for the Interface, not the concrete class.
        public NotificationManager(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public void Notify(string message)
        {
            _messageService.Send(message);
        }
    }

    // Usage
    class DIPResult
    {
        static void Result01()
        {
            // We can now plug in ANY service we want without changing the Manager.
            IMessageService email = new EmailService();
            var manager = new NotificationManager(email);
            manager.Notify("Hello via Email!");

            IMessageService sms = new SmsService();
            var manager2 = new NotificationManager(sms);
            manager2.Notify("Hello via SMS!");
        }
    }
}