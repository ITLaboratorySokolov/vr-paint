using NSubstitute;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;

namespace ZCU.TechnologyLab.Common.Unity.Tests.Behaviours.Connections.Client.Session
{
    public class SessionClientWrapperMock : SessionClientWrapper
    {
        public ISessionClient MockedClient { get; private set; }
        
        protected override ISessionClient CreateClient()
        {
            return MockedClient = Substitute.For<ISessionClient>();
        }
    }
}