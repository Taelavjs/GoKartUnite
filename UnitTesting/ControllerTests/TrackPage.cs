using AngleSharp;
using GoKartUnite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnitTesting.ControllerTests.Bases;

namespace UnitTesting.ControllerTests
{
    public class TrackPage : BaseControllerClass, IClassFixture<TestServer<Program>>
    {
        public TrackPage(TestServer<Program> server) : base(server) { }


    }
}
