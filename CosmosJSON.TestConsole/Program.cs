using CosmosJSON;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        Stopwatch sw = new();
        sw.Start();
        AssertNoCrash("{\"sus\": 1}");
        AssertNoCrash("{\"sus\": 1, \"notsosus\": false}");
        AssertNoCrash("{\"sus\": 1, \"notsosus\": false, \"array\": [1, 2, 3]}");
        AssertNoCrash("{\"sus\": 1, \"notsosus\": false, \"array\": [1, 2, 3], \"subobj\": {\"idk\": null}}");

        TestComplicated();
        TestDeserializeAndSerialize();


        StressTest();
        sw.Stop();

        Console.WriteLine("All tests completed within " + sw.Elapsed.TotalMilliseconds + "ms");
    }

    private static void StressTest()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        for(int i = 0; i < 100000; i++) {
            AssertNoCrash("{\"sus\": 1, \"notsosus\": false, \"array\": [1, 2, 3], \"subobj\": {\"idk\": null}}", false);
        }
        sw.Stop();

        Console.WriteLine("Stress test (100000x parse) completed within " + sw.Elapsed.TotalMilliseconds + "ms");
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

        if(log) Console.WriteLine($"Test AssertNoCrash with json '{json}' passed successfully.");
    }
}