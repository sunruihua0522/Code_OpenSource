using Irixi_Aligner_Common;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.UserScript;
using Irixi_Aligner_Common.UserScript.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IrixiAlignerTest
{
    [TestClass]
    public class UnitTest1
    {
        readonly string TestLogicalAxisHashString = "A85897191ACA8EACA5E67DB50E0AC9176C6CB2A6";
        readonly string TestIPMHashString = "A484BDB22FE8E3B09BA258FFCFCA31FA0F2D671E";

        [TestMethod]
        public void UserScriptListSaveTest()
        {
            SystemService service = new SystemService();
            UserScriptManager mgr = service.ListBasedScriptManager;
            mgr.Clear();
            mgr.Add(new UserScriptMove() { Axis = service.FindLogicalAxisByHashString(TestLogicalAxisHashString), Speed = 100, Distance = 200, Mode = MoveMode.REL });
            mgr.Add(new UserScriptMove() { Axis = service.FindLogicalAxisByHashString(TestLogicalAxisHashString), Speed = 20, Distance = 2000, Mode = MoveMode.REL });
            mgr.Add(new UserScriptSwitchIPMRange() { PowerMeter = service.InternalPowerMeter[0], Range = 3 });

            mgr.Save("Test.dat");

            mgr.Clear();
            mgr.Load("Test.dat");
        }
    }
}
  