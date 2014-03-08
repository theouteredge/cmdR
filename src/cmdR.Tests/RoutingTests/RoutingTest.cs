using cmdR.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdR.Tests
{
    [TestFixture]
    public class RoutingTest
    {
        private Route _simpleRoute = new Route("ls", new Dictionary<string, ParameterType>(), (p, c, s) => {  });
        private Route _singleParamRoute = new Route("ls", new Dictionary<string, ParameterType> { { "path", ParameterType.Required } }, (p, c, s) => { });


        [Test]
        public void RegisterRoute_CanRegisterASimpleRoute()
        {
            var routing = new Routing();
            var count = routing.Count;

            routing.RegisterRoute(_simpleRoute);

            Assert.AreEqual(count + 1, routing.Count);
        }

        [Test]
        public void RegisterRoute_CanRegisterMultipleRoutesWithTheSameNameButDifferentParameteres()
        {
            var routing = new Routing();
            var count = routing.Count;

            routing.RegisterRoute(_simpleRoute);
            routing.RegisterRoute(_singleParamRoute);

            Assert.AreEqual(count + 2, routing.Count);
        }

        [Test]
        [ExpectedException(typeof(InvalidRouteException))]
        public void RegisterRoute_ThorwsExceptionWithDuplicateRoutes()
        {
            var routing = new Routing();
            var count = routing.Count;

            routing.RegisterRoute(_simpleRoute);
            routing.RegisterRoute(_simpleRoute);
        }


        [Test]
        public void FindRoute_CanFindASimpleRoute()
        {
            var routing = new Routing();
            routing.RegisterRoute(_simpleRoute);

            var matchedRoute = routing.FindRoute("ls", new Dictionary<string, string>());

            Assert.AreEqual(_simpleRoute, matchedRoute);
        }

        [Test]
        public void FindRoute_CanFindARouteWithARequiredParam()
        {
            var routing = new Routing();
            
            routing.RegisterRoute(_simpleRoute);
            routing.RegisterRoute(_singleParamRoute);

            var matchedRoute = routing.FindRoute("ls", new Dictionary<string, string>() { { "path", @"c:\temp\" } });

            Assert.AreEqual(_singleParamRoute, matchedRoute);
        }

        [Test]
        [ExpectedException(typeof(NoRouteFoundException))]
        public void FindRoute_ThrowsAnExcpetionWhenNotRoutesMatched()
        {
            var routing = new Routing();

            routing.RegisterRoute(_simpleRoute);

            var matchedRoute = routing.FindRoute("ls", new Dictionary<string, string>() { { "path", @"c:\temp\" } });
        }
    }
}
