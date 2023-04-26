using MrAnnouncerBot;
using NUnit;

namespace MrAnnounceBotTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.True(FredGpt.IsTalkingToFred("Fred, how are you?"));
            Assert.True(FredGpt.IsTalkingToFred("I am intrigued by you, Fred!!!"));
            Assert.True(FredGpt.IsTalkingToFred("We know that Fred is a lie!!!"));
            Assert.False(FredGpt.IsTalkingToFred("Mark, how are you?"));
            Assert.False(FredGpt.IsTalkingToFred("I am intrigued by you, Mark!!!"));
            Assert.False(FredGpt.IsTalkingToFred("We know that Mark is a lie!!!"));
        }
    }
}