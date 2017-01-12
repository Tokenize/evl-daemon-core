using EvlDaemon;
using Xunit;

namespace EvlDaemonTest
{
    public class EventTest
    {
        [Fact]
        public void LoginIncorrectPasswordDescriptionIsCorrect()
        {
            Command command = Event.GetCommand(Event.Login);
            Assert.Equal("Login Interaction: Incorrect Password",
                Event.Describe(command, Event.LoginData.IncorrectPassword));
        }

        [Fact]
        public void LoginSuccessfulDescriptionIsCorrect()
        {
            Command command = Event.GetCommand(Event.Login);
            Assert.Equal("Login Interaction: Login Successful",
                Event.Describe(command, Event.LoginData.LoginSuccessful));
        }

        [Fact]
        public void UnknownCommandDescriptionIsCorrect()
        {
            Command command = new Command() { Number = "999" };
            Assert.Equal("Unknown command (999)", Event.Describe(command));
        }
    }
}
