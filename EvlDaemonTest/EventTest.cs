using EvlDaemon;
using EvlDaemon.Events;
using System;
using System.Collections.Generic;
using Xunit;

namespace EvlDaemonTest
{
    public class EventTest
    {

        EventManager eventManager;

        public EventTest()
        {
            eventManager = new EventManager(new Dictionary<string, string>(), new Dictionary<string, string>());
        }

        [Fact]
        public void LoginIncorrectPasswordDescriptionIsCorrect()
        {
            Command command = new Command() { Number = Command.Login };
            Event e = eventManager.NewEvent(command, Command.LoginType.IncorrectPassword.ToString("d"), DateTime.Now);

            Assert.Equal("Login: Incorrect Password", e.Description);
        }

        [Fact]
        public void LoginSuccessfulDescriptionIsCorrect()
        {
            Command command = new Command() { Number = Command.Login };
            Event e = eventManager.NewEvent(command, Command.LoginType.LoginSuccessful.ToString("d"), DateTime.Now);

            Assert.Equal("Login: Login Successful", e.Description);
        }

        [Fact]
        public void UnknownCommandDescriptionIsCorrect()
        {
            Command command = new Command() { Number = "999" };
            Event e = eventManager.NewEvent(command, "", DateTime.Now);

            Assert.Equal("<Unknown: [999]>", e.Description);
        }
    }
}
