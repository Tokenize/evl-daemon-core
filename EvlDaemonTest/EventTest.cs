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
        Dictionary<string, string> partitions;
        Dictionary<string, string> zones;

        public EventTest()
        {
            partitions = new Dictionary<string, string>()
            {
                { "1", "Main" }
            };
            zones = new Dictionary<string, string>()
            {
                { "001", "Front Door" },
                { "002", "Back Door" }
            };
            eventManager = new EventManager(partitions, zones);
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

        [Fact]
        public void ZoneDescriptionIsCorrect()
        {
            Command command = new Command() { Number = Command.ZoneOpen };
            Event e = eventManager.NewEvent(command, "001", DateTime.Now);

            Assert.Equal($"Zone Open: {zones[e.Data]}", e.Description);
        }

        [Fact]
        public void PartitionDescriptionIsCorrect()
        {
            Command command = new Command() { Number = Command.PartitionNotReady };
            Event e = eventManager.NewEvent(command, "1", DateTime.Now);

            Assert.Equal($"Partition Not Ready: {partitions[e.Data]}", e.Description);
        }
    }
}
