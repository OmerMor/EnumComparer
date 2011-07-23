using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using BenchmarkHelper;
//using v = System.Void;

namespace SitraUtils.Benchmarks
{
    public class EnumComparerBenchmarks
    {
        const int MAX_DAY_OF_WEEK = (int)DayOfWeek.Saturday;
        private static int iterations = 100000;

        public static void MainX()
        {
            var lcgEnumComparer =
                runFunc("building Generic LCG.EnumComparer ",
                        i => LCG.EnumComparer<DayOfWeek>.Instance, 1);
            var enumComparer =
                runFunc("building Generic EnumComparer     ",
                        i => EnumComparer<DayOfWeek>.Instance, 1);
            var dayOfWeekComparer =
                runFunc("building hand-written EnumComparer",
                        i => new DayOfWeekComparer(), 1);

            runAction("Dictionary with Generic LCG.EnumComparer      ",
                      i => populateDictionary(new Dictionary<DayOfWeek, int>(lcgEnumComparer), i), iterations);

            runAction("Dictionary with Generic EnumComparer          ",
                      i => populateDictionary(new Dictionary<DayOfWeek, int>(enumComparer), i), iterations);

            runAction("Dictionary with hand-written DayOfWeekComparer",
                      i => populateDictionary(new Dictionary<DayOfWeek, int>(dayOfWeekComparer), i), iterations);

            runAction("Dictionary with default comparer              ",
                      i => populateDictionary(new Dictionary<DayOfWeek, int>(), i), iterations);

            runAction("Dictionary of ints                            ",
                      i => populateDictionary(new Dictionary<int, int>(), i), iterations);

            Console.ReadLine();
        }
        public static void Main(string[] args)
        {
            try
            {
                iterations = int.Parse(args[0]);
            }
            catch
            {
            }

            Random rng = new Random();
            //goto ints;
            var dayOfWeekList = new List<DayOfWeek>(iterations);
            for (int i = 0; i < iterations; i++)
            {
                dayOfWeekList.Add((DayOfWeek) (rng.Next() % MAX_DAY_OF_WEEK));
            }
            var dayOfWeekArray = dayOfWeekList.ToArray();
            var buildComparerSuite = TestSuite.Create("build comparer", 0, 0)
                .Add(input =>
                {
                    //lcgComparer = LCG.EnumComparer.For<DayOfWeek>();
                    lcgComparer = new LCG.EnumComparer<DayOfWeek>();
                    return 0;
                }, "LCG EnumComparer")
                .Add(input =>
                {
                    //etComparer = EnumComparer.For<DayOfWeek>();
                    etComparer = new EnumComparer<DayOfWeek>();
                    return 0;
                }, "Expression Tree EnumComparer")
                .Add(input =>
                {
                    handWrittenComparer = new DayOfWeekComparer();
                    return 0;
                }, "Hand-Written EnumComparer")
                /*.RunTests()*/;
            Init();
            var addToDictSuite = TestSuite.Create("add to dict", dayOfWeekArray, 0)
                .Add(input =>
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        var dayOfWeek = input[i];
                        lcgDict[dayOfWeek] = i;
                    }
                    return 0;
                }, "LCG EnumComparer")
                .Add(input =>
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        var dayOfWeek = input[i];
                        lcg2Dict[dayOfWeek] = i;
                    }
                    return 0;
                }, "LCG2 EnumComparer")
                .Add(input =>
                         {
                             for (int i = 0; i < input.Length; i++)
                             {
                                 var dayOfWeek = input[i];
                                 etDict[dayOfWeek] = i;
                             }
                             return 0;
                         }, "Expression Tree EnumComparer")
                .Add(input =>
                         {
                             for (int i = 0; i < input.Length; i++)
                             {
                                 var dayOfWeek = input[i];
                                 handDict[dayOfWeek] = i;
                             }
                             return 0;
                         }, "Hand-Written EnumComparer")
                .Add(input =>
                         {
                             for (int i = 0; i < input.Length; i++)
                             {
                                 var dayOfWeek = input[i];
                                 defaultDict[dayOfWeek] = i;
                             }
                             return 0;
                         }, "Default Comparer")
                .Add(input =>
                         {
                             for (int i = 0; i < input.Length; i++)
                             {
                                 int dayOfWeek = (int) input[i];
                                 intDict[dayOfWeek] = i;
                             }
                             return 0;
                         }, "Int Dictionary")
                .RunTests();

            var best = BenchmarkResult.FindBest(/*buildComparerSuite, */addToDictSuite);

/*
            Console.WriteLine("-------------------- Build --------------------");
            buildComparerSuite.Display(ResultColumns.NameAndScore, best);

*/
            Console.WriteLine("-------------------- Add --------------------");
            addToDictSuite.Display(ResultColumns.NameAndScore, best);
        }
        private static EnumComparer<DayOfWeek> etComparer = EnumComparer.For<DayOfWeek>();
        private static LCG.EnumComparer<DayOfWeek> lcgComparer = LCG.EnumComparer.For<DayOfWeek>();
        private static LCG.EnumComparer2<DayOfWeek> lcg2Comparer = LCG.EnumComparer2<DayOfWeek>.Instance;
        private static DayOfWeekComparer handWrittenComparer = new DayOfWeekComparer();
        private static Dictionary<DayOfWeek, int> lcgDict;
        private static Dictionary<DayOfWeek, int> lcg2Dict;
        private static Dictionary<DayOfWeek, int> etDict;
        private static Dictionary<DayOfWeek, int> handDict;
        private static Dictionary<DayOfWeek, int> defaultDict;
        private static Dictionary<int, int> intDict;

        public static void Init()
        {
            etComparer = EnumComparer.For<DayOfWeek>();
            lcgComparer = LCG.EnumComparer.For<DayOfWeek>();
            lcg2Comparer = LCG.EnumComparer2<DayOfWeek>.Instance;
            handWrittenComparer = new DayOfWeekComparer();

            lcgDict = new Dictionary<DayOfWeek, int>(MAX_DAY_OF_WEEK, lcgComparer);
            lcg2Dict = new Dictionary<DayOfWeek, int>(MAX_DAY_OF_WEEK, lcg2Comparer);
            etDict = new Dictionary<DayOfWeek, int>(MAX_DAY_OF_WEEK, etComparer);
            handDict = new Dictionary<DayOfWeek, int>(MAX_DAY_OF_WEEK, handWrittenComparer);
            defaultDict = new Dictionary<DayOfWeek, int>(MAX_DAY_OF_WEEK);
            intDict = new Dictionary<int, int>(MAX_DAY_OF_WEEK);
        }

        //[Benchmark]
        public static void EtEnumComparer()
        {
            for (var i = 0; i < iterations; i++)
            {
                var key = (DayOfWeek)(i % MAX_DAY_OF_WEEK);
                etDict[key] = i;
            }
        }
        //[Benchmark]
        public static void LcgEnumComparer()
        {
            for (int i = 0; i < iterations; i++)
            {
                var key = (DayOfWeek)(i % MAX_DAY_OF_WEEK);
                lcgDict[key] = i;
            }
        }

        public void foo(Func<int> d)
        {
            
        }

        public void foo(Expression<Func<int>> e)
        {
            
        }

        public void foo()
        {
/*
            Expression<Func<int>> e = () => 10;
            foo(() => 10);
            foo(e);
            foo(e.Compile().Invoke);
*/
            Type t = typeof (Expression<>);
            //Debugger.Break();
            Type vtype = typeof (void);
            Type genericType = t.MakeGenericType(vtype);
        }

        //[Benchmark]
        public static void HandWrittenComparer()
        {
            for (int i = 0; i < iterations; i++)
            {
                var key = (DayOfWeek)(i % MAX_DAY_OF_WEEK);
                handDict[key] = i;
            }
        }
        //[Benchmark]
        public static void DefaultComparer()
        {
            for (int i = 0; i < iterations; i++)
            {
                var key = (DayOfWeek)(i % MAX_DAY_OF_WEEK);
                defaultDict[key] = i;
            }
        }
        //[Benchmark]
        public static void IntDictionary()
        {
            for (int i = 0; i < iterations; i++)
            {
                var key = (i % MAX_DAY_OF_WEEK);
                intDict[key] = i;
            }
        }

        private static void runAction(string desc, Action<int> action, int iters)
        {
            action(iters);
            var stopwatch = new Stopwatch();
            var elapsed = new TimeSpan();
            const int RUNS = 3;
            for (var i = 0; i < RUNS; i++ )
            {
                stopwatch.Start();
                action(iters);
                stopwatch.Stop();
                elapsed += stopwatch.Elapsed;
                stopwatch.Reset();
            }
            elapsed = TimeSpan.FromTicks(elapsed.Ticks / RUNS);
            double ticksPerSec = TimeSpan.FromSeconds(1).Ticks;
            Console.WriteLine(desc + ": {0:N1} ms ({1:N0} hz)",
                              elapsed.TotalMilliseconds, iters * ticksPerSec / elapsed.Ticks);
        }
        private static T runFunc<T>(string desc, Func<int, T> func, int iters)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = func(iters);
            stopwatch.Stop();
            double ticksPerSec = TimeSpan.FromSeconds(1).Ticks;
            Console.WriteLine(desc + ": {0:N1} ms ({1:N0} hz)",
                              stopwatch.Elapsed.TotalMilliseconds, iters * ticksPerSec / stopwatch.ElapsedTicks);
            return result;
        }

        private static void populateDictionary(Dictionary<DayOfWeek, int> map, int iters)
        {
            for (var i = 0; i < iters; i++)
            {
                var key = (DayOfWeek)(i % MAX_DAY_OF_WEEK);
                map[key] = i;
            }
        }
        private static void populateDictionary(Dictionary<int, int> map, int iters)
        {
            for (var i = 0; i < iters; i++)
            {
                var key = (i % MAX_DAY_OF_WEEK);
                map[key] = i;
            }
        }
    }
}
