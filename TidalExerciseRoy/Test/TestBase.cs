using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using TidalExerciseRoy.BL;
using TidalExerciseRoy.Common;
using static NLog.LogManager;

namespace TidalExerciseRoy.Test
{
    public class TestBase
    {
        protected RequestMaker RequestMaker = new RequestMaker();
        protected string RandomId;
        [SetUp]
        public async Task Setup()
        {
            Debug.WriteLine("Starting test");
            RandomId = Guid.NewGuid().ToString();
            await RequestMaker.Login(ConfigHandler.Username, ConfigHandler.Password);
        }

        [TearDown]
        public async Task EndTest()
        {
            Debug.WriteLine("Ending test");
            Shutdown();
            await RequestMaker.OpenTidlSession.Logout();
        }

    }
}