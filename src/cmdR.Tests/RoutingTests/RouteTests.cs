using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdR.Tests.RoutingTests
{
    [TestFixture]
    public class RouteTests
    {
        Dictionary<string, ParameterType> _singleRequired = new Dictionary<string, ParameterType>() { { "path", ParameterType.Required } };
        Dictionary<string, ParameterType> _singleOptional = new Dictionary<string, ParameterType>() { { "path", ParameterType.Optional } };

        Dictionary<string, ParameterType> _doubleRequired = new Dictionary<string, ParameterType>() 
            { 
                { "path", ParameterType.Required },
                { "filter", ParameterType.Required } 
            };

        Dictionary<string, ParameterType> _doubleRequiredOpt = new Dictionary<string, ParameterType>() 
            { 
                { "path", ParameterType.Required },
                { "filter", ParameterType.Optional } 
            };

        Dictionary<string, ParameterType> _doubleOptional = new Dictionary<string, ParameterType>() 
            { 
                { "path", ParameterType.Optional },
                { "filter", ParameterType.Optional } 
            };



        [Test]
        public void Match_CanMatchRouteWithoutParams()
        {
            var route = new Route("ls", new Dictionary<string, ParameterType>(), (p) => { var i = 1 + 1; });

            var match = route.Match(new List<string>());

            Assert.IsTrue(TryMatchRoute(new Dictionary<string, ParameterType>(), new List<string>()));
        }


        [Test]
        public void Match_CanMatchRouteWithSingleRequiredParam()
        {
            Assert.IsTrue(TryMatchRoute(_singleRequired, new List<string> { "path" }));
        }


        [Test]
        public void Match_DoesntMatchRouteWithSingleRequiredParam()
        {
            Assert.IsFalse(TryMatchRoute(_singleRequired, new List<string>()));
        }

        [Test]
        public void Match_DoesntMatchRouteWhereParamIsDifferent()
        {
            Assert.IsFalse(TryMatchRoute(_singleOptional, new List<string> { "bob" }));
        }


        [Test]
        public void Match_CanMatchRouteWithSingleOptionalParam()
        {
            Assert.IsTrue(TryMatchRoute(_singleOptional, new List<string> { "path" }));
        }

        [Test]
        public void Match_CanMatchRouteWithSingleOptionalParamWithoutTheParam()
        {
            Assert.IsTrue(TryMatchRoute(_singleOptional, new List<string>()));
        }

        [Test]
        public void Match_CanMatchRouteWithTwoRequiredParams()
        {
            Assert.IsTrue(TryMatchRoute(_doubleRequired, new List<string> { "path", "filter" }));
        }

        [Test]
        public void Match_DoesntMatchRouteWhereParamsAreDifferent_One()
        {
            Assert.IsFalse(TryMatchRoute(_doubleRequired, new List<string> { "path", "bob" }));
        }

        [Test]
        public void Match_DoesntMatchRouteWhereParamsAreDifferent_Two()
        {
            Assert.IsFalse(TryMatchRoute(_doubleRequired, new List<string> { "bob", "filter" }));
        }

        [Test]
        public void Match_DoesntMatchRouteWhereParamIsMissing()
        {
            Assert.IsFalse(TryMatchRoute(_doubleRequired, new List<string> { "filter" }));
        }

        [Test]
        public void Match_TwoOptinalParamsWithOneParam()
        {
            Assert.IsTrue(TryMatchRoute(_doubleOptional, new List<string> { "filter" }));
        }

        [Test]
        public void Match_TwoOptinalParamsWithTwoParam()
        {
            Assert.IsTrue(TryMatchRoute(_doubleOptional, new List<string> { "filter", "path" }));
        }

        [Test]
        public void Match_TwoOptinalParamsWithNoParams()
        {
            Assert.IsTrue(TryMatchRoute(_doubleOptional, new List<string>()));
        }



        private bool TryMatchRoute(IDictionary<string, ParameterType> routeParams, List<string> commandParams)
        {
            var route = new Route("ls", routeParams, (p) => { var i = 1 + 1; });

            return route.Match(commandParams);
        }
    }
}
