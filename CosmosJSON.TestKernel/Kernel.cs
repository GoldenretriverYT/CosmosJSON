using Cosmos.HAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Sys = Cosmos.System;

namespace CosmosJSON.TestKernel
{
    public class Kernel : Sys.Kernel
    {

        protected override void BeforeRun()
        {
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
        }

        protected override void Run()
        {
            //Stopwatch sw = new();
            //sw.Start();
            AssertNoCrash("{\"sus\": 1}");
            AssertNoCrash("{\"sus\": 1, \"notsosus\": false}");
            AssertNoCrash("{\"sus\": 1, \"notsosus\": false, \"array\": [1, 2, 3]}");
            AssertNoCrash("{\"sus\": 1, \"notsosus\": false, \"array\": [1, 2, 3], \"subobj\": {\"idk\": null}}");

            TestComplicated();
            TestDeserializeAndSerialize();


            StressTest();
            //sw.Stop();

            //Console.WriteLine("All tests completed within " + sw.Elapsed.TotalMilliseconds + "ms");

            while (true) { }
        }

        private static void StressTest()
        {
            var startSec = RTC.Minute * 60 + RTC.Second;
            // Stopwatch sw = new Stopwatch();
            //sw.Start();
            for (int i = 0; i < 1000; i++) { // .net took 500ms at 100x the iterations
                AssertNoCrash("{\"sus\": 1, \"notsosus\": false, \"array\": [1, 2, 3], \"subobj\": {\"idk\": null}}", false);
            }
            //sw.Stop();

            var endSec = RTC.Minute * 60 + RTC.Second;
            Console.WriteLine("Stress test (1000x parse) completed within " + (endSec - startSec) + "s");
        }

        private static void TestComplicated()
        {
            var json = "{\"sus\": 1, \"notsosus\": false, \"array\": [1, 2, 3], \"subobj\": {\"idk\": null}}";

            var parser = new Parser(json);
            var obj = parser.Parse();

            if ((int)obj["sus"] != 1) throw new Exception("Assertion failed: obj['sus'] != 1");
            if ((bool)obj["notsosus"] != false) throw new Exception("Assertion failed: obj['notsosus'] != false");
            if ((int)((object[])obj["array"])[0] != 1) throw new Exception("Assertion failed: obj['array'][0] != 1");
            if ((int)((object[])obj["array"])[1] != 2) throw new Exception("Assertion failed: obj['array'][1] != 2");
            if ((int)((object[])obj["array"])[2] != 3) throw new Exception("Assertion failed: obj['array'][2] != 3");
            if (((Dictionary<string, object>)obj["subobj"])["idk"] != null) throw new Exception("Assertion failed: obj['subobj']['idk'] != null");

            Console.WriteLine("Test TestComplicated passed successfully.");
        }

        private static void TestDeserializeAndSerialize()
        {
            var json = "{\"sus\":1, \"notsosus\": false}";
            var expected = "{\"sus\": 30, \"notsosus\": false, \"okbro\": {\"bro\": {\"deeplyNested\": true}, \"ye\": 123.23}}";

            var parser = new Parser(json);
            var obj = parser.Parse();

            if ((int)obj["sus"] != 1) throw new Exception("Assertion failed: obj['sus'] != 1");
            if ((bool)obj["notsosus"] != false) throw new Exception("Assertion failed: obj['notsosus'] != false");

            obj["sus"] = 30;
            obj["okbro"] = new Dictionary<string, object>()
        {
            { "bro", new Dictionary<string, object>
                {
                    { "deeplyNested", true }
                }
            },
            { "ye", 123.23f }
        };

            var serialized = Serializer.Serialize(obj);

            if (serialized != expected) throw new Exception("Assertion failed: serialized != expected");

            Console.WriteLine("Test TestDeserializeAndSerialize passed successfully.");
        }

        private static void AssertNoCrash(string json, bool log = true)
        {
            try {
                var parser = new Parser(json);
                parser.Parse();
            } catch (Exception e) {
                Console.WriteLine($"Assertion of '{json}' failed: Failed to parse JSON: {e.Message}");
                throw;
            }

            if (log) Console.WriteLine($"Test AssertNoCrash with json '{json}' passed successfully.");
        }
    }
}
